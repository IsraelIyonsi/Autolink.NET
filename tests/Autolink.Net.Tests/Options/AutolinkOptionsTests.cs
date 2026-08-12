namespace Autolink.Net.Tests.Options;

public class AutolinkOptionsTests
{
    private const string SampleText = "Ping @alice, email bob@example.com, tag #dotnet, visit example.org";

    [Fact]
    public void DetectUrls_false_suppresses_url_matches_but_keeps_others()
    {
        var options = new AutolinkOptions { DetectUrls = false };

        var matches = Autolinker.FindMatches(SampleText, options);

        Assert.DoesNotContain(matches, m => m.Kind == MatchKind.Url);
        Assert.Contains(matches, m => m.Kind == MatchKind.Mention);
        Assert.Contains(matches, m => m.Kind == MatchKind.Email);
        Assert.Contains(matches, m => m.Kind == MatchKind.Hashtag);
    }

    [Fact]
    public void DetectEmails_false_suppresses_email_matches_but_keeps_others()
    {
        var options = new AutolinkOptions { DetectEmails = false };

        var matches = Autolinker.FindMatches(SampleText, options);

        Assert.DoesNotContain(matches, m => m.Kind == MatchKind.Email);
        Assert.Contains(matches, m => m.Kind == MatchKind.Url);
    }

    [Fact]
    public void DetectMentions_false_suppresses_mention_matches_but_keeps_others()
    {
        var options = new AutolinkOptions { DetectMentions = false };

        var matches = Autolinker.FindMatches(SampleText, options);

        Assert.DoesNotContain(matches, m => m.Kind == MatchKind.Mention);
        Assert.Contains(matches, m => m.Kind == MatchKind.Email);
    }

    [Fact]
    public void DetectHashtags_false_suppresses_hashtag_matches_but_keeps_others()
    {
        var options = new AutolinkOptions { DetectHashtags = false };

        var matches = Autolinker.FindMatches(SampleText, options);

        Assert.DoesNotContain(matches, m => m.Kind == MatchKind.Hashtag);
        Assert.Contains(matches, m => m.Kind == MatchKind.Url);
    }

    [Fact]
    public void All_kinds_disabled_returns_no_matches()
    {
        var options = new AutolinkOptions
        {
            DetectUrls = false,
            DetectEmails = false,
            DetectMentions = false,
            DetectHashtags = false,
        };

        var matches = Autolinker.FindMatches(SampleText, options);

        Assert.Empty(matches);
    }

    [Fact]
    public void Unknown_top_level_domain_is_rejected_by_default()
    {
        var matches = Autolinker.FindMatches("visit example.internalcorp today");

        Assert.Empty(matches);
    }

    [Fact]
    public void AdditionalTopLevelDomains_extends_recognition_case_insensitively()
    {
        var options = new AutolinkOptions { AdditionalTopLevelDomains = ["internalcorp"] };

        var matches = Autolinker.FindMatches("visit example.INTERNALCORP today", options);

        var match = Assert.Single(matches);
        Assert.Equal(MatchKind.Url, match.Kind);
        Assert.Equal("example.INTERNALCORP", match.Value);
    }

    [Fact]
    public void FindMatches_throws_on_null_text()
    {
        Assert.Throws<ArgumentNullException>(() => Autolinker.FindMatches(null!));
    }
}
