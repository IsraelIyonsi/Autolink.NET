namespace Autolink.Net.Tests.Matching;

public class MatchFixtureTests
{
    [Theory]
    [MemberData(nameof(MatchFixtures.Cases), MemberType = typeof(MatchFixtures))]
    public void FindMatches_returns_exact_spans_for_fixture(string text, AutolinkMatch[] expected)
    {
        var actual = Autolinker.FindMatches(text);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(MatchFixtures.Cases), MemberType = typeof(MatchFixtures))]
    public void Every_match_value_equals_the_substring_at_its_reported_span(string text, AutolinkMatch[] expected)
    {
        foreach (var match in expected)
        {
            Assert.Equal(match.Value, text.Substring(match.Index, match.Length));
        }
    }
}
