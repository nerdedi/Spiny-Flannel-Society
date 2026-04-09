"""
NDIS Quote Generator

Generates Participant Summary of Supports style quotes using:
- NDIS price guide rates
- Claiming category validation
- Program/support line items and support period totals

The output format mirrors the structure of common provider quote templates.
"""

from __future__ import annotations

from dataclasses import dataclass, field
from datetime import date, datetime
from pathlib import Path
from typing import Dict, List, Optional
import argparse
import csv
import html
import json

from ndis_compliance_guidance import get_document_guidance


DATE_FMT = "%d/%m/%Y"
REGION_NATIONAL = "national"
REGION_REMOTE = "remote"
REGION_VERY_REMOTE = "very_remote"
VALID_REGIONS = {REGION_NATIONAL, REGION_REMOTE, REGION_VERY_REMOTE}


def normalize_region(region: str) -> str:
    """Normalize region aliases to one of: national, remote, very_remote."""
    value = region.strip().lower().replace("-", "_").replace(" ", "_")
    aliases = {
        "national": REGION_NATIONAL,
        "metro": REGION_NATIONAL,
        "remote": REGION_REMOTE,
        "very_remote": REGION_VERY_REMOTE,
        "veryremote": REGION_VERY_REMOTE,
    }
    resolved = aliases.get(value, value)
    if resolved not in VALID_REGIONS:
        raise ValueError(
            f"Invalid region '{region}'. Use one of: national, remote, very_remote."
        )
    return resolved


def parse_date(value: str) -> date:
    """Parse DD/MM/YYYY date."""
    return datetime.strptime(value, DATE_FMT).date()


@dataclass
class PriceGuideItem:
    """A billable support item in the NDIS price guide."""

    code: str
    name: str
    unit: str  # e.g. hour, each
    rate: float
    remote_rate: Optional[float] = None
    very_remote_rate: Optional[float] = None
    claiming_categories: List[str] = field(default_factory=list)


@dataclass
class ClaimingCategory:
    """A category under which support items may be claimed."""

    code: str
    name: str
    description: str = ""


@dataclass
class QuoteLine:
    """One quoted support row."""

    program: str
    support_item_code: str
    support_item_name: str
    service_type: str  # Face-to-Face, Non Face-to-Face, Centre Capital Costs, etc.
    claiming_category: str
    ratio: str = "N/A"
    quantity_per_day: float = 0.0
    days_per_week: int = 1
    payment_method: str = "NDIA Managed"
    override_rate: Optional[float] = None

    def rate(self, catalog: "NDISCatalog", region: str = REGION_NATIONAL) -> float:
        if self.override_rate is not None:
            return self.override_rate
        return catalog.get_rate(self.support_item_code, region=region)

    def daily_total(self, catalog: "NDISCatalog", region: str = REGION_NATIONAL) -> float:
        return round(self.quantity_per_day * self.rate(catalog, region=region), 2)

    def weekly_total(self, catalog: "NDISCatalog", region: str = REGION_NATIONAL) -> float:
        return round(self.daily_total(catalog, region=region) * self.days_per_week, 2)


@dataclass
class Participant:
    """Participant details shown in the quote header."""

    name: str
    ndis_number: str
    dob: str


@dataclass
class ParticipantQuote:
    """Participant Summary of Supports style quote model."""

    participant: Participant
    support_start: date
    support_end: date
    lines: List[QuoteLine] = field(default_factory=list)
    region: str = REGION_NATIONAL

    def support_weeks(self) -> float:
        days = (self.support_end - self.support_start).days + 1
        return max(days / 7.0, 0.0)

    def weekly_total(self, catalog: "NDISCatalog", region_override: Optional[str] = None) -> float:
        region = normalize_region(region_override or self.region)
        return round(sum(line.weekly_total(catalog, region=region) for line in self.lines), 2)

    def period_total(
        self,
        catalog: "NDISCatalog",
        weeks_override: Optional[float] = None,
        region_override: Optional[str] = None,
    ) -> float:
        weeks = self.support_weeks() if weeks_override is None else weeks_override
        return round(self.weekly_total(catalog, region_override=region_override) * weeks, 2)

    def validate(self, catalog: "NDISCatalog") -> List[str]:
        """Validate support item codes and claiming category mapping."""
        errors: List[str] = []

        try:
            normalize_region(self.region)
        except ValueError as e:
            errors.append(str(e))

        if self.support_end < self.support_start:
            errors.append("Support end date cannot be earlier than start date.")

        for idx, line in enumerate(self.lines, start=1):
            if not catalog.has_item(line.support_item_code):
                errors.append(
                    f"Line {idx}: Unknown support item code '{line.support_item_code}'."
                )
                continue

            if not catalog.has_category(line.claiming_category):
                errors.append(
                    f"Line {idx}: Unknown claiming category '{line.claiming_category}'."
                )
                continue

            if not catalog.is_item_allowed_in_category(
                line.support_item_code, line.claiming_category
            ):
                errors.append(
                    f"Line {idx}: Item {line.support_item_code} is not allowed in category {line.claiming_category}."
                )

            if line.quantity_per_day < 0:
                errors.append(f"Line {idx}: quantity_per_day must be non-negative.")

            if line.days_per_week < 0:
                errors.append(f"Line {idx}: days_per_week must be non-negative.")

        return errors

    def to_html(
        self,
        catalog: "NDISCatalog",
        weeks_override: Optional[float] = None,
        region_override: Optional[str] = None,
    ) -> str:
        """Render quote as a printable HTML document."""
        region = normalize_region(region_override or self.region)
        weekly = self.weekly_total(catalog, region_override=region)
        total = self.period_total(catalog, weeks_override=weeks_override, region_override=region)

        guidance = None
        try:
            guidance = get_document_guidance("quote", max_snippets_per_source=2)
        except Exception:
            guidance = None

        rows = []
        for line in self.lines:
            rate = line.rate(catalog, region=region)
            daily = line.daily_total(catalog, region=region)
            weekly_line = line.weekly_total(catalog, region=region)

            rows.append(
                f"""
                <tr>
                  <td>{html.escape(line.program)}</td>
                  <td>{html.escape(line.support_item_name)} - {html.escape(line.support_item_code)}<br><small>{html.escape(line.service_type)}</small></td>
                  <td style=\"text-align:center\">{html.escape(line.ratio)}</td>
                  <td style=\"text-align:right\">{line.quantity_per_day:.2f}</td>
                  <td style=\"text-align:right\">$ {rate:.2f}</td>
                  <td style=\"text-align:right\">$ {rate:.2f}</td>
                  <td style=\"text-align:right\">$ {daily:.2f}</td>
                  <td style=\"text-align:right\">{line.days_per_week}</td>
                  <td style=\"text-align:right\">$ {weekly_line:.2f}</td>
                  <td>{html.escape(line.payment_method)}</td>
                </tr>
                """
            )

        return f"""
<!doctype html>
<html>
<head>
  <meta charset=\"utf-8\">
  <title>Participant Summary of Supports</title>
  <style>
    body {{ font-family: Arial, sans-serif; margin: 24px; color: #111; }}
    h1 {{ text-align: center; font-size: 28px; margin-bottom: 20px; }}
    .header-grid {{ display: grid; grid-template-columns: 1fr 1fr; gap: 16px; margin-bottom: 14px; }}
    .card {{ border: 1px solid #bbb; }}
    .card table {{ width: 100%; border-collapse: collapse; }}
    .card td {{ border: 1px solid #ddd; padding: 6px 8px; font-size: 13px; }}
    .label {{ background: #b72d2f; color: #fff; font-weight: bold; width: 42%; }}
    .summary-title {{ margin: 18px 0 8px; font-weight: bold; }}
    table.quote {{ width: 100%; border-collapse: collapse; font-size: 12px; }}
    table.quote th, table.quote td {{ border: 1px solid #c7c7c7; padding: 6px; vertical-align: top; }}
    table.quote th {{ background: #66c1c4; text-align: left; }}
    .small {{ font-size: 11px; color: #444; margin-top: 16px; line-height: 1.4; }}
        .compliance {{ margin-top: 14px; font-size: 12px; }}
        .compliance h3 {{ margin: 10px 0 6px; font-size: 14px; }}
        .compliance ul {{ margin: 4px 0 8px 18px; }}
        .compliance .src {{ margin-top: 4px; color: #333; }}
  </style>
</head>
<body>
  <h1>Participant Summary of Supports</h1>

  <div class=\"header-grid\">
    <div class=\"card\">
      <table>
        <tr><td class=\"label\">PARTICIPANT NAME:</td><td>{html.escape(self.participant.name)}</td></tr>
                <tr><td class="label">RATE REGION:</td><td colspan="4" style="text-transform:uppercase">{html.escape(region.replace('_', ' '))}</td></tr>
        <tr><td class=\"label\">PARTICIPANT NDIS NUMBER:</td><td>{html.escape(self.participant.ndis_number)}</td></tr>
        <tr><td class=\"label\">PARTICIPANT DATE OF BIRTH:</td><td>{html.escape(self.participant.dob)}</td></tr>
      </table>
    </div>

    <div class=\"card\">
      <table>
        <tr><td class=\"label\">PERIOD OF SUPPORT</td><td>START DATE:</td><td>{self.support_start.strftime(DATE_FMT)}</td><td>END DATE:</td><td>{self.support_end.strftime(DATE_FMT)}</td></tr>
        <tr><td class=\"label\">WEEKLY TOTAL:</td><td colspan=\"4\" style=\"text-align:right\">$ {weekly:.2f}</td></tr>
        <tr><td class=\"label\">TOTAL:</td><td colspan=\"4\" style=\"text-align:right\">$ {total:.2f}</td></tr>
      </table>
    </div>
  </div>

  <div class=\"summary-title\">Details of Support per Week</div>
  <table class=\"quote\">
    <thead>
      <tr>
        <th>Program</th>
        <th>NDIS Support Item</th>
        <th>Ratio</th>
        <th>Hours / Units per Day</th>
        <th>Unit Cost</th>
        <th>Rate per hour/unit</th>
        <th>Daily Total</th>
        <th>Days per week</th>
        <th>Weekly total</th>
        <th>Payment Method</th>
      </tr>
    </thead>
    <tbody>
      {''.join(rows)}
    </tbody>
  </table>

  <div class=\"small\">
    Generated quote is based on configured NDIS price guide rates and claiming categories.
    Always verify final claimability against your current NDIS rules and participant plan.
  </div>

    {
        "" if guidance is None else f"""
    <div class=\"compliance\">
        <h3>Compliance Guidance (NDIS governance sources)</h3>
        <ul>
            {''.join(f'<li>{html.escape(item)}</li>' for item in guidance['checklist'])}
        </ul>
        <div class=\"src\"><strong>Sources:</strong></div>
        <ul>
            {''.join(f"<li>{html.escape(src['title'])} ({html.escape(src['pdf_file'])})</li>" for src in guidance['sources'])}
        </ul>
    </div>
        """
    }
</body>
</html>
"""


class NDISCatalog:
    """Holds price guide items and claiming categories."""

    def __init__(self, items: Dict[str, PriceGuideItem], categories: Dict[str, ClaimingCategory]):
        self.items = items
        self.categories = categories

    def has_item(self, item_code: str) -> bool:
        return item_code in self.items

    def has_category(self, category_code: str) -> bool:
        return category_code in self.categories

    def get_rate(self, item_code: str, region: str = REGION_NATIONAL) -> float:
        item = self.items.get(item_code)
        if item is None:
            raise KeyError(f"Unknown support item code: {item_code}")

        resolved_region = normalize_region(region)
        if resolved_region == REGION_REMOTE and item.remote_rate is not None:
            return item.remote_rate
        if resolved_region == REGION_VERY_REMOTE and item.very_remote_rate is not None:
            return item.very_remote_rate
        return item.rate

    def is_item_allowed_in_category(self, item_code: str, category_code: str) -> bool:
        item = self.items.get(item_code)
        if item is None:
            return False

        if not item.claiming_categories:
            return True

        return category_code in item.claiming_categories

    @classmethod
    def from_json(cls, price_guide_path: str, claiming_categories_path: str) -> "NDISCatalog":
        with open(price_guide_path, "r", encoding="utf-8") as f:
            price_data = json.load(f)
        with open(claiming_categories_path, "r", encoding="utf-8") as f:
            category_data = json.load(f)

        items: Dict[str, PriceGuideItem] = {}
        for item in price_data.get("items", []):
            code = item["code"]
            items[code] = PriceGuideItem(
                code=code,
                name=item.get("name", code),
                unit=item.get("unit", "hour"),
                rate=float(item.get("rate", 0.0)),
                remote_rate=(
                    float(item["remote_rate"])
                    if item.get("remote_rate") is not None
                    else None
                ),
                very_remote_rate=(
                    float(item["very_remote_rate"])
                    if item.get("very_remote_rate") is not None
                    else None
                ),
                claiming_categories=item.get("claiming_categories", []),
            )

        categories: Dict[str, ClaimingCategory] = {}
        for category in category_data.get("categories", []):
            code = category["code"]
            categories[code] = ClaimingCategory(
                code=code,
                name=category.get("name", code),
                description=category.get("description", ""),
            )

        return cls(items, categories)

    @classmethod
    def from_price_guide_csv(
        cls,
        csv_path: str,
        claiming_categories_path: str,
        code_field: str = "code",
        name_field: str = "name",
        rate_field: str = "rate",
        remote_rate_field: str = "remote_rate",
        very_remote_rate_field: str = "very_remote_rate",
        unit_field: str = "unit",
        categories_field: str = "claiming_categories",
    ) -> "NDISCatalog":
        """
        Build catalog from CSV + category JSON.

        Expected CSV headers (customizable):
        - code
        - name
        - rate
        - unit
        - claiming_categories (semicolon-delimited)
        """
        with open(csv_path, "r", encoding="utf-8") as f:
            reader = csv.DictReader(f)
            rows = list(reader)

        items: Dict[str, PriceGuideItem] = {}
        for row in rows:
            code = str(row.get(code_field, "")).strip()
            if not code:
                continue

            categories_raw = str(row.get(categories_field, "")).strip()
            item_categories = [c.strip() for c in categories_raw.split(";") if c.strip()]

            items[code] = PriceGuideItem(
                code=code,
                name=str(row.get(name_field, code)).strip(),
                unit=str(row.get(unit_field, "hour")).strip(),
                rate=float(row.get(rate_field, 0.0) or 0.0),
                remote_rate=(
                    float(row.get(remote_rate_field, 0.0) or 0.0)
                    if str(row.get(remote_rate_field, "")).strip()
                    else None
                ),
                very_remote_rate=(
                    float(row.get(very_remote_rate_field, 0.0) or 0.0)
                    if str(row.get(very_remote_rate_field, "")).strip()
                    else None
                ),
                claiming_categories=item_categories,
            )

        with open(claiming_categories_path, "r", encoding="utf-8") as f:
            category_data = json.load(f)

        categories: Dict[str, ClaimingCategory] = {}
        for category in category_data.get("categories", []):
            code = category["code"]
            categories[code] = ClaimingCategory(
                code=code,
                name=category.get("name", code),
                description=category.get("description", ""),
            )

        return cls(items, categories)


def load_default_catalog() -> NDISCatalog:
    """Load bundled catalog, preferring real 2025-26 extracted data when available."""
    base = Path(__file__).resolve().parent / "ndis_data"
    preferred = base / "price_guide.2025-26.json"
    fallback = base / "price_guide.sample.json"

    return NDISCatalog.from_json(
        str(preferred if preferred.exists() else fallback),
        str(base / "claiming_categories.sample.json"),
    )


def build_sample_quote() -> ParticipantQuote:
    """Build a quote similar to the provided quote screenshot structure."""
    participant = Participant(
        name="Sample Participant",
        ndis_number="4300 000 000",
        dob="01/01/2000",
    )

    quote = ParticipantQuote(
        participant=participant,
        support_start=parse_date("09/03/2026"),
        support_end=parse_date("27/04/2026"),
        lines=[
            QuoteLine(
                program="Core Supports",
                support_item_code="04_102_0136_6_1",
                support_item_name="Group Activities - Standard - Weekday Daytime",
                service_type="Face-to-Face",
                claiming_category="LLND_GROUP_ACTIVITIES",
                ratio="1:1",
                quantity_per_day=1.00,
                days_per_week=1,
            ),
            QuoteLine(
                program="Core Supports",
                support_item_code="04_102_0136_6_1",
                support_item_name="Group Activities - Standard - Weekday Daytime",
                service_type="Non Face-to-Face",
                claiming_category="LLND_GROUP_ACTIVITIES",
                ratio="N/A",
                quantity_per_day=0.03846,
                days_per_week=1,
            ),
            QuoteLine(
                program="Core Supports",
                support_item_code="04_599_0136_6_1",
                support_item_name="Centre Capital Cost Standard",
                service_type="Centre Capital Costs",
                claiming_category="LLND_CENTRE_CAPITAL",
                ratio="N/A",
                quantity_per_day=1.00,
                days_per_week=1,
            ),
        ],
    )
    return quote


def generate_quote_html(
    quote: ParticipantQuote,
    output_path: str,
    catalog: Optional[NDISCatalog] = None,
    weeks_override: Optional[float] = None,
    region_override: Optional[str] = None,
) -> str:
    """Validate and write quote HTML to disk."""
    catalog = catalog or load_default_catalog()
    errors = quote.validate(catalog)
    if errors:
        raise ValueError("Quote validation failed:\n- " + "\n- ".join(errors))

    output = Path(output_path)
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(
        quote.to_html(
            catalog,
            weeks_override=weeks_override,
            region_override=region_override,
        ),
        encoding="utf-8",
    )
    return str(output)


def _parse_args():
    parser = argparse.ArgumentParser(description="Generate Participant Summary of Supports quote")
    parser.add_argument(
        "--region",
        default=REGION_NATIONAL,
        choices=[REGION_NATIONAL, REGION_REMOTE, REGION_VERY_REMOTE],
        help="Rate region to use for item pricing.",
    )
    parser.add_argument(
        "--weeks",
        type=float,
        default=8.0,
        help="Override number of support weeks used for total calculation.",
    )
    return parser.parse_args()


def main() -> None:
    """Generate sample quote HTML for quick verification."""
    args = _parse_args()

    catalog = load_default_catalog()
    quote = build_sample_quote()
    quote.region = args.region

    errors = quote.validate(catalog)
    if errors:
        raise SystemExit("Validation errors:\n- " + "\n- ".join(errors))

    output = generate_quote_html(
        quote,
        "output/participant_summary_of_supports.html",
        catalog=catalog,
        weeks_override=args.weeks,
        region_override=args.region,
    )

    print("NDIS quote generated.")
    print(f"Rate region: {args.region}")
    print(f"Weekly total: ${quote.weekly_total(catalog, region_override=args.region):.2f}")
    print(
        f"Period total ({args.weeks:g} weeks): "
        f"${quote.period_total(catalog, weeks_override=args.weeks, region_override=args.region):.2f}"
    )
    print(f"File: {output}")


if __name__ == "__main__":
    main()
