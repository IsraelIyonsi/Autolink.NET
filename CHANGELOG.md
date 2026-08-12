# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0] - 2026-08-12

### Added

- `Autolinker` static API: `FindMatches(string, AutolinkOptions?)` to scan text without modifying it, `Linkify(string, LinkifyOptions?)` to render HTML-safe anchor tags, and `DefaultHrefResolver` as the default `LinkifyOptions.HrefResolver`.
- Detection of four match kinds via `MatchKind`: `Url` (http/https, and bare domains with a known top-level domain), `Email`, `Mention` (`@handle`), and `Hashtag` (`#topic`).
- `AutolinkMatch` readonly record struct exposing `Kind`, `Value`, `Index` and `Length` for every match.
- URL boundary handling matching common autolinker behavior: trailing sentence punctuation is trimmed, an unbalanced wrapping closing parenthesis is excluded, and a balanced parenthesis pair inside the URL (as in Wikipedia article URLs) is kept.
- TLD-aware bare-domain and email recognition against a built-in top-level domain set (`TopLevelDomains.Known`), so a filename like `report.txt` is not linked while `example.com` is. Extendable via `AutolinkOptions.AdditionalTopLevelDomains`.
- `AutolinkOptions` to enable or disable each match kind independently (`DetectUrls`, `DetectEmails`, `DetectMentions`, `DetectHashtags`).
- `LinkifyOptions` for `Rel`, `Target`, `CssClass`, a per-match `HrefResolver` delegate, and an `AdditionalAttributes` delegate for arbitrary extra attributes. All rendered text and attribute values are HTML-escaped, making `Linkify` output injection-safe by construction.
- Unicode support in mention and hashtag bodies (for example `@café`, `#naïve`).
- Zero runtime dependencies. Matching is implemented with source-generated regexes (`GeneratedRegexAttribute`), so the package is Native AOT and trimming friendly.
