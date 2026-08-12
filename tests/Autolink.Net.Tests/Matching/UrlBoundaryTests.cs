namespace Autolink.Net.Tests.Matching;

public class UrlBoundaryTests
{
    [Theory]
    [InlineData("Nested (parens (still) balance) https://example.com/(a(b)c) end", "https://example.com/(a(b)c)")]
    [InlineData("Doubly wrapped ((https://example.com)) here", "https://example.com")]
    [InlineData("Trailing semicolon https://example.com; next", "https://example.com")]
    [InlineData("Trailing colon https://example.com: next", "https://example.com")]
    [InlineData("Trailing asterisk https://example.com* next", "https://example.com")]
    [InlineData("Quoted \"https://example.com\" text", "https://example.com")]
    public void Url_boundary_is_trimmed_to_expected_value(string text, string expectedUrl)
    {
        var matches = Autolinker.FindMatches(text);

        var urlMatch = Assert.Single(matches, m => m.Kind == MatchKind.Url);
        Assert.Equal(expectedUrl, urlMatch.Value);
    }

    [Theory]
    [InlineData("http://example.com")]
    [InlineData("http://sub.example.co.uk/path")]
    [InlineData("https://192.168.1.1:8080/status")]
    [InlineData("https://localhost:5000/health")]
    public void Scheme_urls_do_not_require_a_known_top_level_domain(string text)
    {
        var matches = Autolinker.FindMatches(text);

        var urlMatch = Assert.Single(matches);
        Assert.Equal(MatchKind.Url, urlMatch.Kind);
        Assert.Equal(text, urlMatch.Value);
    }

    [Theory]
    [InlineData("config.yaml is not a url")]
    [InlineData("archive.zip download link")]
    [InlineData("photo.jpeg attached")]
    [InlineData("style.css and script.js")]
    public void Bare_filenames_with_unknown_extensions_are_not_matched(string text)
    {
        var matches = Autolinker.FindMatches(text);

        Assert.Empty(matches);
    }
}
