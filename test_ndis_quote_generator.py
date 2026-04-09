"""Tests for NDIS quote generator."""

from ndis_quote_generator import (
    NDISCatalog,
    build_sample_quote,
    load_default_catalog,
    generate_quote_html,
)


def test_quote_totals():
    catalog = load_default_catalog()
    quote = build_sample_quote()

    # 70.23 + 2.70 + 2.59 = 75.52 weekly at 1 day/week
    assert abs(quote.weekly_total(catalog) - 75.52) < 0.01


def test_quote_validation_passes():
    catalog = load_default_catalog()
    quote = build_sample_quote()
    assert quote.validate(catalog) == []


def test_quote_totals_remote_region():
    catalog = load_default_catalog()
    quote = build_sample_quote()
    quote.region = "remote"

    # 98.32 + 3.78 + 3.63 = 105.73 weekly at 1 day/week
    assert abs(quote.weekly_total(catalog) - 105.73) < 0.01


def test_quote_totals_very_remote_region():
    catalog = load_default_catalog()
    quote = build_sample_quote()
    quote.region = "very_remote"

    # 105.35 + 4.05 + 3.89 = 113.29 weekly at 1 day/week
    assert abs(quote.weekly_total(catalog) - 113.29) < 0.01


def test_quote_html_generation():
    catalog = load_default_catalog()
    quote = build_sample_quote()

    path = generate_quote_html(
        quote,
        output_path="output/test_participant_summary_of_supports.html",
        catalog=catalog,
    )

    assert path.endswith(".html")


def run_all_tests():
    test_quote_totals()
    test_quote_validation_passes()
    test_quote_totals_remote_region()
    test_quote_totals_very_remote_region()
    test_quote_html_generation()
    print("✓ NDIS quote generator tests passed")


if __name__ == "__main__":
    run_all_tests()
