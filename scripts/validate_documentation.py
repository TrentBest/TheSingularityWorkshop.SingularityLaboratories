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
CONTAINMENT_CAMPAIGN = DOCS / "specimens" / "night-signal-containment-campaign.md"
WORK_QUEUE = ROOT / "TODO.md"
README = ROOT / "README.md"
ROADMAP = ROOT / "ROADMAP.md"
DOCS_INDEX = DOCS / "index.md"
VISUALS = [
    DOCS / "assets" / "laboratory-manual-cover.svg",
    DOCS / "assets" / "laboratory-lifecycle.svg",
    DOCS / "assets" / "laboratory-domain-map.svg",
    DOCS / "assets" / "laboratory-staff-roles.svg",
    DOCS / "assets" / "laboratory-facility-system.svg",
    DOCS / "assets" / "teaching-research-campus-map.svg",
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
    pattern = re.compile(r"!?\[[^\]]*\]\(([^)]+)\)")
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
    for path in (MANUAL, COMPANION, CATALOGUE, PAGE, CONTAINMENT_CAMPAIGN, WORK_QUEUE, README, ROADMAP, DOCS_INDEX, *VISUALS):
        require(errors, path.is_file(), f"required publication asset is missing: {path.relative_to(ROOT)}")
    if errors:
        print("\n".join(errors))
        return 1

    manual = MANUAL.read_text(encoding="utf-8")
    companion = COMPANION.read_text(encoding="utf-8")
    catalogue = CATALOGUE.read_text(encoding="utf-8")
    containment_campaign = CONTAINMENT_CAMPAIGN.read_text(encoding="utf-8")
    work_queue = WORK_QUEUE.read_text(encoding="utf-8")
    readme = README.read_text(encoding="utf-8")
    roadmap = ROADMAP.read_text(encoding="utf-8")
    docs_index = DOCS_INDEX.read_text(encoding="utf-8")
    page = PAGE.read_text(encoding="utf-8")

    work_queue_requirements = [
        "## Status vocabulary",
        "## Current implementation",
        "## Active work — next in execution order",
        "## Planned platform work",
        "## Cross-repository coordination",
        "## Verification and release gates",
        "## Maintenance rule",
    ]
    for marker in work_queue_requirements:
        require(errors, marker in work_queue, f"repository work queue is missing required section: {marker}")
    require(errors, "Implemented" in work_queue and "Verified" in work_queue,
            "repository work queue must distinguish implementation from verification")
    require(errors, "[Repository work queue](TODO.md)" in readme,
            "README must link to the canonical repository work queue")
    require(errors, "[repository work queue](../TODO.md)" in docs_index,
            "documentation index must link to the canonical repository work queue")

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

    for marker in ("# NIGHT SIGNAL", "## 1. Campaign identity", "## 6. Safe-escape gate", "## 7. Death and safe-escape endings", "## 9. Acceptance tests", "not a real-world checklist"):
        require(errors, marker in containment_campaign, f"containment campaign is missing required scenario contract: {marker}")

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
    for path in (MANUAL, COMPANION, CATALOGUE, CONTAINMENT_CAMPAIGN, WORK_QUEUE, README, ROADMAP, DOCS_INDEX, *(item[0] for item in sourcebooks)):
        link_count += check_markdown_links(path, errors)

    require(errors, "laboratory-manual.md" in page and "laboratory-technical-companion.md" in page,
            "Pages entry point must link to both primary manuals")
    require(errors, "assets/laboratory-domain-map.svg" in page and "assets/laboratory-lifecycle.svg" in page,
            "Pages entry point must expose the living domain and research lifecycle diagrams")
    require(errors, "assets/teaching-research-campus-map.svg" in page and "data-facility=" in page,
            "Pages entry point must expose the campus map and interactive facility selector")
    require(errors, 'href="#campus">Campus explorer' in page,
            "Pages primary navigation must expose the campus explorer")
    facility_keys = set(re.findall(r'data-facility="([^"]+)"', page))
    facility_record_keys = set(re.findall(r'\b(welcome|teaching|physics|instruments|research|data|operations|archive):\{title:', page))
    require(errors, facility_keys == facility_record_keys and len(facility_keys) == 8,
            "campus selector buttons and facility detail records must have the same eight stable keys")
    require(errors, 'id="night-signal"' in page and "night-signal-containment-campaign.md" in page,
            "Pages entry point must expose and document the Night Signal campaign")
    require(errors, "data-night-action" in page and "safe-escape" in page and "terminal" in page,
            "Night Signal proof of concept must expose decision actions and terminal outcome recording")
    require(errors, "archivedAttempts" in page and "night.archive.push" in page and "night-export" in page,
            "Night Signal restart and export must preserve earlier attempts in the session archive")
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
