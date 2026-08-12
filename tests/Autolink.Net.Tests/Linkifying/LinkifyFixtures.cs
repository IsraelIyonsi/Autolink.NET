namespace Autolink.Net.Tests.Linkifying;

/// <summary>
/// ASCII-only inputs whose expected linkified HTML is safe to hardcode verbatim: none of their
/// plain-text segments contain characters outside the four WebUtility.HtmlEncode-escaped ASCII
/// symbols (&amp; &lt; &gt; ") or an apostrophe, so there is no ambiguity in the expected
/// literal. Unicode-bearing inputs are covered separately by span-only fixtures, since exact
/// HTML-entity output for non-ASCII text is an implementation detail of the base class library,
/// not of this package.
/// </summary>
public static class LinkifyFixtures
{
    public static IEnumerable<object[]> Cases()
    {
        yield return
        [
            "Visit https://example.com for details.",
            "Visit <a href=\"https://example.com\">https://example.com</a> for details.",
        ];

        yield return
        [
            "Contact info@example.com today",
            "Contact <a href=\"mailto:info@example.com\">info@example.com</a> today",
        ];

        yield return
        [
            "example.com has no scheme",
            "<a href=\"https://example.com\">example.com</a> has no scheme",
        ];

        yield return
        [
            "@jack posted #dotnet news",
            "@jack posted #dotnet news",
        ];

        yield return
        [
            "No links here at all.",
            "No links here at all.",
        ];

        yield return
        [
            "<script>alert(1)</script> visit https://example.com",
            "&lt;script&gt;alert(1)&lt;/script&gt; visit <a href=\"https://example.com\">https://example.com</a>",
        ];

        yield return
        [
            "Terms & Conditions: read them at https://example.com/terms&more",
            "Terms &amp; Conditions: read them at <a href=\"https://example.com/terms&amp;more\">https://example.com/terms&amp;more</a>",
        ];

        yield return
        [
            "Say \"hello\" then visit https://example.com",
            "Say &quot;hello&quot; then visit <a href=\"https://example.com\">https://example.com</a>",
        ];
    }
}
