namespace Autolink.Net.Tests.Linkifying;

public class LinkifyAttributeTests
{
    [Fact]
    public void Rel_target_and_css_class_are_rendered_and_escaped()
    {
        var options = new LinkifyOptions
        {
            Rel = "nofollow noopener",
            Target = "_blank",
            CssClass = "auto-link \"fancy\"",
        };

        var html = Autolinker.Linkify("Visit https://example.com now", options);

        Assert.Equal(
            "Visit <a href=\"https://example.com\" class=\"auto-link &quot;fancy&quot;\" rel=\"nofollow noopener\" target=\"_blank\">https://example.com</a> now",
            html);
    }

    [Fact]
    public void Additional_attributes_are_appended_and_escaped()
    {
        var options = new LinkifyOptions
        {
            AdditionalAttributes = _ => new Dictionary<string, string> { ["data-kind"] = "\"tracked\"" },
        };

        var html = Autolinker.Linkify("https://example.com", options);

        Assert.Equal(
            "<a href=\"https://example.com\" data-kind=\"&quot;tracked&quot;\">https://example.com</a>",
            html);
    }

    [Fact]
    public void Additional_attributes_with_an_invalid_name_throws()
    {
        var options = new LinkifyOptions
        {
            AdditionalAttributes = _ => new Dictionary<string, string> { ["onclick=alert(1) x"] = "y" },
        };

        Assert.Throws<ArgumentException>(() => Autolinker.Linkify("https://example.com", options));
    }

    [Fact]
    public void Custom_href_resolver_can_enable_mention_and_hashtag_links()
    {
        var options = new LinkifyOptions
        {
            HrefResolver = match => match.Kind switch
            {
                MatchKind.Mention => $"https://example.social/{match.Value[1..]}",
                MatchKind.Hashtag => $"https://example.social/tags/{match.Value[1..]}",
                _ => Autolinker.DefaultHrefResolver(match),
            },
        };

        var html = Autolinker.Linkify("@jack posted #dotnet", options);

        Assert.Equal(
            "<a href=\"https://example.social/jack\">@jack</a> posted <a href=\"https://example.social/tags/dotnet\">#dotnet</a>",
            html);
    }

    [Fact]
    public void Href_resolver_returning_null_leaves_match_as_escaped_plain_text()
    {
        var options = new LinkifyOptions { HrefResolver = _ => null };

        var html = Autolinker.Linkify("Visit https://example.com <now>", options);

        Assert.Equal("Visit https://example.com &lt;now&gt;", html);
    }

    [Fact]
    public void Linkify_with_no_matches_returns_html_escaped_original_text()
    {
        var html = Autolinker.Linkify("Just <plain> & simple text");

        Assert.Equal("Just &lt;plain&gt; &amp; simple text", html);
    }

    [Fact]
    public void Linkify_throws_on_null_text()
    {
        Assert.Throws<ArgumentNullException>(() => Autolinker.Linkify(null!));
    }

    [Fact]
    public void FindMatches_throws_on_null_text()
    {
        Assert.Throws<ArgumentNullException>(() => Autolinker.FindMatches(null!));
    }
}
