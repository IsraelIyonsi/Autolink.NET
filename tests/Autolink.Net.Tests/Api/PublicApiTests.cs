namespace Autolink.Net.Tests.Api;

public class PublicApiTests
{
    [Fact]
    public void AutolinkMatch_has_value_equality()
    {
        var first = new AutolinkMatch(MatchKind.Url, "example.com", 0, 11);
        var second = new AutolinkMatch(MatchKind.Url, "example.com", 0, 11);

        Assert.Equal(first, second);
        Assert.True(first == second);
    }

    [Fact]
    public void AutolinkMatch_exposes_kind_value_index_and_length()
    {
        var match = new AutolinkMatch(MatchKind.Email, "a@b.com", 3, 7);

        Assert.Equal(MatchKind.Email, match.Kind);
        Assert.Equal("a@b.com", match.Value);
        Assert.Equal(3, match.Index);
        Assert.Equal(7, match.Length);
    }

    [Theory]
    [InlineData(MatchKind.Url)]
    [InlineData(MatchKind.Email)]
    [InlineData(MatchKind.Mention)]
    [InlineData(MatchKind.Hashtag)]
    public void MatchKind_defines_the_four_expected_values(MatchKind kind)
    {
        Assert.True(Enum.IsDefined(kind));
    }

    [Fact]
    public void DefaultHrefResolver_builds_mailto_href_for_email()
    {
        var match = new AutolinkMatch(MatchKind.Email, "a@example.com", 0, 13);

        Assert.Equal("mailto:a@example.com", Autolinker.DefaultHrefResolver(match));
    }

    [Fact]
    public void DefaultHrefResolver_prefixes_bare_domain_with_https()
    {
        var match = new AutolinkMatch(MatchKind.Url, "example.com", 0, 11);

        Assert.Equal("https://example.com", Autolinker.DefaultHrefResolver(match));
    }

    [Fact]
    public void DefaultHrefResolver_leaves_scheme_url_unchanged()
    {
        var match = new AutolinkMatch(MatchKind.Url, "http://example.com", 0, 19);

        Assert.Equal("http://example.com", Autolinker.DefaultHrefResolver(match));
    }

    [Theory]
    [InlineData(MatchKind.Mention)]
    [InlineData(MatchKind.Hashtag)]
    public void DefaultHrefResolver_returns_null_for_mention_and_hashtag(MatchKind kind)
    {
        var match = new AutolinkMatch(kind, "@x", 0, 2);

        Assert.Null(Autolinker.DefaultHrefResolver(match));
    }

    [Fact]
    public void AutolinkOptions_default_enables_every_match_kind()
    {
        var options = AutolinkOptions.Default;

        Assert.True(options.DetectUrls);
        Assert.True(options.DetectEmails);
        Assert.True(options.DetectMentions);
        Assert.True(options.DetectHashtags);
        Assert.Null(options.AdditionalTopLevelDomains);
    }

    [Fact]
    public void LinkifyOptions_default_has_no_rel_target_or_class()
    {
        var options = LinkifyOptions.Default;

        Assert.Null(options.Rel);
        Assert.Null(options.Target);
        Assert.Null(options.CssClass);
        Assert.Null(options.AdditionalAttributes);
        Assert.Same(AutolinkOptions.Default, options.MatchOptions);
    }
}
