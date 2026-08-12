# Autolink.NET

Detect and linkify URLs, email addresses, mentions and hashtags in plain text. TLD-aware bare domain detection, HTML-safe output, and the same boundary handling common autolinkers use for trailing punctuation and parentheses. Zero external dependencies.

Every JavaScript framework has a text-linking library that gets URL boundaries right: trailing periods stripped, a Wikipedia-style `(programming_language)` kept intact, `report.txt` left alone because `.txt` is not a domain. On NuGet the options are either abandoned Twitter-era ports or full regex dumps with no tests around the actual hard part, which is deciding where a URL ends. Autolink.NET is a from-scratch, actively maintained implementation with a table of the exact inputs that break naive linkers, each asserted against an exact expected output.

Where you need it:

- Rendering user-generated text (comments, chat, bios) as safe HTML without a templating engine or JavaScript dependency
- Turning plain-text log lines or support tickets into clickable links server-side
- Extracting structured mentions and hashtags from freeform text without pulling in a social-media SDK

## Install

```
dotnet add package Autolink.Net
```

## Quickstart

```csharp
using Autolink;

string html = Autolinker.Linkify("Reach out to info@example.com or visit example.com.");

Console.WriteLine(html);
// Reach out to <a href="mailto:info@example.com">info@example.com</a> or
// visit <a href="https://example.com">example.com</a>.
```

`Linkify` always returns HTML that is safe to insert directly into a page: everything outside a match is escaped, and every attribute value it renders is escaped too.

## Finding matches without touching the text

```csharp
using Autolink;

var matches = Autolinker.FindMatches("Ping @maria about #launch, see docs.example.com/v2 for details.");

foreach (var match in matches)
{
    Console.WriteLine($"{match.Kind} '{match.Value}' at {match.Index} (len {match.Length})");
}
// Mention '@maria' at 5 (len 6)
// Hashtag '#launch' at 18 (len 7)
// Url 'docs.example.com/v2' at 31 (len 19)
```

`FindMatches` never modifies the input. Use it when you need the spans for highlighting, redaction, or building your own renderer.

## Controlling rel, target and where mentions link to

`Autolinker` has no opinion on where `@handle` or `#topic` should point, because that depends on your platform. The default `HrefResolver` links URLs and emails and leaves mentions and hashtags as plain escaped text; supply your own resolver to wire them up:

```csharp
using Autolink;

var options = new LinkifyOptions
{
    Rel = "nofollow noopener",
    Target = "_blank",
    HrefResolver = match => match.Kind switch
    {
        MatchKind.Mention => $"https://example.social/{match.Value[1..]}",
        MatchKind.Hashtag => $"https://example.social/tags/{match.Value[1..]}",
        _ => Autolinker.DefaultHrefResolver(match),
    },
};

string html = Autolinker.Linkify("@jack posted #dotnet", options);
// <a href="https://example.social/jack" rel="nofollow noopener" target="_blank">@jack</a>
// posted
// <a href="https://example.social/tags/dotnet" rel="nofollow noopener" target="_blank">#dotnet</a>
```

Return `null` from `HrefResolver` for any match you want left as plain text instead of an anchor.

## Boundary handling

This is the part naive regex linkers get wrong, and the part this package's fixture table is built around:

| Input | Linked span |
|---|---|
| `Visit https://example.com.` | `https://example.com` (trailing sentence period dropped) |
| `(https://example.com/foo)` | `https://example.com/foo` (unbalanced wrapping paren dropped) |
| `.../wiki/PHP_(programming_language)` | the parenthesis is kept, because it is balanced inside the URL |
| `report.txt` | not linked; `.txt` is not a known top-level domain |
| `example.com` | linked; `.com` is |

## Zero dependencies, AOT-friendly

No runtime NuGet dependencies. Matching uses source-generated regexes (`GeneratedRegexAttribute`), not runtime-compiled `Regex` instances, so the package works under Native AOT and trimming without reflection fallbacks. The built-in top-level domain table (`TopLevelDomains.Known`) is a plain in-memory set you can extend via `AutolinkOptions.AdditionalTopLevelDomains`.

## Notes and limitations

- Domain matching is ASCII-only; internationalized domain names (IDN/punycode) are not recognized as bare domains in this release. Mention and hashtag bodies do support Unicode letters (`@café`, `#naïve`).
- The built-in top-level domain set deliberately excludes newer generic domains that are also ordinary English words (`.email`, `.guru`, `.ninja`, and similar), to avoid false positives in prose. Add them yourself via `AutolinkOptions.AdditionalTopLevelDomains` if you need them.
- `Autolinker.FindMatches` never overlaps matches: at any position, the first recognized kind wins, in priority order URL, email, mention, hashtag.
- A `#` or `@` inside another match's word run is not re-examined, so Fediverse-style handles are not treated as a single unit: `@user@example.com` matches as the mention `@user` followed by the bare domain `example.com`, and `_john@example.com` (an invalid local part) matches only the bare domain `example.com` extracted from inside the address.
- `AdditionalAttributes` attribute names are validated against a conservative safe pattern (start with a letter, underscore or colon; only letters, digits, hyphens, underscores, colons and periods after that) and `Linkify` throws `ArgumentException` for a name outside it, since a name is rendered verbatim into the tag rather than as an escaped attribute value.

## License

MIT. See [LICENSE](LICENSE).
