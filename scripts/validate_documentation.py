#!/usr/bin/env python3
"""Fail-closed checks for the reader manuals and static Pages entry point."""
from __future__ import annotations

import re
import sys
import xml.etree.ElementTree as ET
from html.parser import HTMLParser
from pathlib import Path
from urllib.parse import unquote, urlsplit

ROOT = Path(__file__).resolve().parents[1]
DOCS = ROOT / "docs"

MANUAL = DOCS / "laboratory-manual.md"
COMPANION = DOCS / "laboratory-technical-companion.md"
CATALOGUE = DOCS / "books" / "index.md"
PAGE = DOCS / "index.html"
VISUALS = [
    DOCS / "assets" / "laboratory-manual-cover.svg",
    DOCS / "assets" / "laboratory-lifecycle.svg",
    DOCS / "assets" / "laboratory-domain-map.svg",
    DOCS / "assets" / "laboratory-staff-roles.svg",
    DOCS / "assets" / "laboratory-facility-system.svg",
]


def require(errors: list[str], condition: bool, message: str) -> None:
    if not condition:
        errors.append(message)


class PageParser(HTMLParser):
    def __init__(self) -> None:
        super().__init__()
        self.ids: set[str] = set()
        self.duplicates: set[str] = set()
        self.links: list[tuple[str, str]] = []

    def handle_starttag(self, tag: str, attrs: list[tuple[str, str | None]]) -> None:
        values = dict(attrs)
        element_id = values.get("id")
        if element_id:
            if element_id in self.ids:
                self.duplicates.add(element_id)
            self.ids.add(element_id)
        for attribute in ("href", "src"):
            value = values.get(attribute)
            if value:
                self.links.append((attribute, value))


def check_markdown_links(path: Path, errors: list[str]) -> int:
    source = path.read_text(encoding="utf-8")
    checked = 0
    pattern = re.compile(r"!??\[[^\]]*\]\(([^)]+)\")
    for raw in pattern.findall(source):
        target = raw.strip().split()[0].strip("<>")
        parts = urlsplit(target)
        if parts.scheme or parts.netloc or not parts.path:
            continue
        local = (path.parent / unquote(parts.path)).resolve()
        if ROOT.resolve() not in local.parents:
            errors.append(f"{path.relative_to(ROOT)}: local link escapes repository: {target}")
            continue
        checked += 1
        if not local.is_file():
            errors.append(f"{path.relative_to(ROOT)}: broken local link: {target}")
    return checked


def main() -> int:
    errors: list[str] = []
    for path in (MANUAL, COMPANION, CATALOGUE, PAGE, *VISUALS):
        require(errors, path.is_file(), f"required publication asset is missing: {path.relative_to(ROOT)}")
    if errors:
        print("\n".join(errors))
        return 1

    manual = MANUAL.read_text(encoding="utf-8")
    companion = COMPANION.read_text(encoding="utf-8")
    catalogue = CATALOGUE.read_text(encoding="utf-8")
    page = PAGE.read_text(encoding="utf-8")

    manual_requirements = [
        "# Singularity Laboratories",
        "## The Living Laboratory — Core Manual",
        "**Development manuscript · Edition 0.1**",
        "## How to use this book",
        "## The promise of this book",
        "### The researcher’s view",
        "### The laboratory author’s Workshop view",
        "### A living domain map",
        "### Adapter views: human inquiry and digital execution",
        "# Part I — The big idea",
        "# Part II — Conducting an investigation",
        "# Part III — Reading evidence responsibly",
        "# Part IV — The institution",
        "# Part V — Worked experiences",
        "# Part VI — Sourcebooks and deeper study",
        "# Part VII — Reference and appendices",
    ]
    for marker in manual_requirements:
        require(errors, marker in manual, f"core manual is missing required book structure: {marker}")

    companion_requirements = [
        "## How to use this companion",
        "## 01 — What: system boundary",
        "## 02 — Why: the engineering problem",
        "## 03 — How: implementation map",
        "## 04 — See it: verification and evidence",
        "## 05 — Choose your next step",
        "## Appendix A — Host obligations",
        "## Appendix B — Integration rules",
        "## Appendix C — Open engineering questions",
    ]
    for marker in companion_requirements:
        require(errors, marker in companion, f"technical companion is missing required structure: {marker}")

    sourcebooks = [
        (DOCS / "books" / "scientific-inquiry.md", "../assets/laboratory-lifecycle.svg", "## Methods in action"),
        (DOCS / "books" / "laboratory-npcs.md", "../assets/laboratory-staff-roles.svg", "## Responsibilities in action"),
        (DOCS / "books" / "flagship-laboratory-facilities.md", "../assets/laboratory-facility-system.svg", "## Facility composition in action"),
    ]
    for sourcebook_path, required_visual, worked_example in sourcebooks:
        sourcebook = sourcebook_path.read_text(encoding="utf-8")
        label = sourcebook_path.relative_to(ROOT)
        for marker in (
            "## How to use this book",
            "## Player /",
            "## Game Master / Workshop overview",
            "## Living domain map",
            "## Adapter views",
            "## Authoring and reference",
            worked_example,
            required_visual,
        ):
            require(errors, marker in sourcebook, f"{label}: missing SURPS living-book element: {marker}")

    require(errors, "Living Rulebook and Experience Standard" in catalogue,
            "sourcebook catalogue must link the canonical SURPS living-book standard")
    require(errors, "Behavior Model and Adapters" in catalogue,
            "sourcebook catalogue must link the canonical behavior/adapters contract")
    require(errors, "laboratory-manual.md" in catalogue and "laboratory-technical-companion.md" in catalogue,
            "sourcebook catalogue must route readers to both primary manuals")

    page_parser = PageParser()
    page_parser.feed(page)
    for duplicate in sorted(page_parser.duplicates):
        errors.append(f"docs/index.html: duplicate id attribute: {duplicate}")
    for attribute, value in page_parser.links:
        parts = urlsplit(value)
        if parts.scheme or parts.netloc:
            continue
        if value.startswith("#"):
            anchor = value[1:]
            if anchor and anchor not in page_parser.ids:
                errors.append(f"docs/index.html: missing in-page anchor: {value}")
            continue
        if parts.path:
            local = (PAGE.parent / unquote(parts.path)).resolve()
            if ROOT.resolve() not in local.parents or not local.is_file():
                errors.append(f"docs/index.html: broken local {attribute}: {value}")

    for visual in VISUALS:
        try:
            root = ET.parse(visual).getroot()
        except (ET.ParseError, OSError) as exc:
            errors.append(f"{visual.relative_to(ROOT)}: invalid SVG: {exc}")
            continue
        names = {element.tag.rsplit("}", 1)[-1] for element in root.iter()}
        require(errors, root.tag.rsplit("}", 1)[-1] == "svg", f"{visual.relative_to(ROOT)}: root must be svg")
        require(errors, "title" in names and "desc" in names,
                f"{visual.relative_to(ROOT)}: accessible title and description are required")

    link_count = 0
    for path in (MANUAL, COMPANION, CATALOGUE, *(item[0] for item in sourcebooks)):
        link_count += check_markdown_links(path, errors)

    require(errors, "laboratory-manual.md" in page and "laboratory-technical-companion.md" in page,
            "Pages entry point must link to both primary manuals")
    require(errors, "campaign workbench" in page.lower() and "evidence" in page.lower(),
            "Pages entry point must retain its interactive workbench and evidence surfaces")

    if errors:
        print("Singularity Laboratories documentation validation FAILED:")
        for error in errors:
            print(f" - {error}")
        return 1

    print("Singularity Laboratories documentation validation passed.")
    print(f"Validated the core manual, technical companion, sourcebook catalogue, Pages anchors, and {link_count} local Markdown links.")
    print(f"Validated {len(VISUALS)} accessible SVG publication assets.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
