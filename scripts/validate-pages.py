#!/usr/bin/env python3
"""Validate the assembled, deployable Singularity Laboratories Pages site."""
from __future__ import annotations

import sys
from html.parser import HTMLParser
from pathlib import Path
from urllib.parse import unquote, urlsplit
import xml.etree.ElementTree as ET

ROOT = Path(sys.argv[sys.argv.index("--root") + 1]).resolve() if "--root" in sys.argv else Path("_site").resolve()
errors: list[str] = []

class Document(HTMLParser):
    def __init__(self) -> None:
        super().__init__(convert_charrefs=True)
        self.ids: set[str] = set()
        self.links: list[tuple[str, str]] = []
    def handle_starttag(self, tag: str, attrs: list[tuple[str, str | None]]) -> None:
        values = dict(attrs)
        element_id = values.get("id")
        if element_id:
            if element_id in self.ids:
                errors.append(f"duplicate id #{element_id}")
            self.ids.add(element_id)
        if tag == "a" and values.get("name"):
            self.ids.add(values["name"] or "")
        for attr in ("href", "src"):
            value = values.get(attr)
            if value:
                self.links.append((tag, value))

def fail(message: str) -> None:
    errors.append(message)

if not ROOT.is_dir():
    fail(f"site root does not exist: {ROOT}")
else:
    entry = ROOT / "index.html"
    if not entry.is_file() or entry.stat().st_size == 0:
        fail("assembled site is missing a non-empty root index.html")
    else:
        print(f"PASS root entry: {entry.relative_to(ROOT)}")

    html_files = sorted(ROOT.rglob("*.html"))
    if not html_files:
        fail("site contains no HTML documents")

    documents: dict[Path, Document] = {}
    for path in html_files:
        parser = Document()
        try:
            parser.feed(path.read_text(encoding="utf-8"))
            parser.close()
            documents[path.resolve()] = parser
        except (OSError, UnicodeError) as exc:
            fail(f"cannot parse {path.relative_to(ROOT)}: {exc}")

    for source, parser in documents.items():
        for _tag, raw in parser.links:
            parsed = urlsplit(raw)
            if parsed.scheme or parsed.netloc or raw.startswith(("#", "//")):
                continue
            target_text = unquote(parsed.path)
            if not target_text:
                target = source
            else:
                target = (source.parent / target_text).resolve()
            if target.is_dir():
                target = target / "index.html"
            if not target.is_file():
                fail(f"{source.relative_to(ROOT)}: missing local target {raw}")
                continue
            if parsed.fragment and target.suffix.lower() in {".html", ".htm"}:
                target_doc = documents.get(target.resolve())
                if target_doc and unquote(parsed.fragment) not in target_doc.ids:
                    fail(f"{source.relative_to(ROOT)}: missing fragment {raw}")

    svg_files = sorted(ROOT.rglob("*.svg"))
    for path in svg_files:
        try:
            ET.parse(path)
        except (ET.ParseError, OSError) as exc:
            fail(f"invalid SVG {path.relative_to(ROOT)}: {exc}")

    required_assets = (
        "assets/laboratory-lifecycle.svg",
        "assets/teaching-research-campus-map.svg",
        "assets/laboratory-domain-map.svg",
    )
    for relative in required_assets:
        asset = ROOT / relative
        if not asset.is_file() or asset.stat().st_size == 0:
            fail(f"required authored visual is missing: {relative}")

    if not (ROOT / ".nojekyll").is_file():
        fail("site is missing .nojekyll")

if errors:
    print("Pages validation failed:", file=sys.stderr)
    for error in errors:
        print(f" - {error}", file=sys.stderr)
    sys.exit(1)

print(f"PASS: {len(documents)} HTML documents and {len(svg_files)} SVG assets validated.")
