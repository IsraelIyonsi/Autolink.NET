namespace Autolink.Net.Tests.Matching;

/// <summary>
/// A table of tricky inputs and their exact expected match spans, covering the correctness
/// hotspots called out in the package specification: URL boundary detection (trailing
/// punctuation, wrapping and balanced parentheses), TLD-aware bare-domain recognition versus
/// filenames, email-versus-mention disambiguation, and Unicode mention and hashtag bodies.
/// </summary>
public static class MatchFixtures
{
    public static IEnumerable<object[]> Cases()
    {
        yield return Case(
            "Visit https://example.com for details.",
            (MatchKind.Url, "https://example.com", 6, 19));

        yield return Case(
            "Contact us at info@example.com today",
            (MatchKind.Email, "info@example.com", 14, 16));

        yield return Case("The filename report.txt should not be linked");

        yield return Case(
            "example.com is a valid domain",
            (MatchKind.Url, "example.com", 0, 11));

        yield return Case(
            "www.example.com is also fine",
            (MatchKind.Url, "www.example.com", 0, 15));

        yield return Case("internal.corpnet is not linked");

        yield return Case("visit localhost for testing");

        yield return Case(
            "See https://en.wikipedia.org/wiki/PHP_(programming_language) for info",
            (MatchKind.Url, "https://en.wikipedia.org/wiki/PHP_(programming_language)", 4, 56));

        yield return Case(
            "The link (https://example.com/foo) works",
            (MatchKind.Url, "https://example.com/foo", 10, 23));

        yield return Case(
            "Ends with comma https://example.com, right?",
            (MatchKind.Url, "https://example.com", 16, 19));

        yield return Case(
            "Check out https://example.com! Amazing",
            (MatchKind.Url, "https://example.com", 10, 19));

        yield return Case(
            "See [https://example.com] for more",
            (MatchKind.Url, "https://example.com", 5, 19));

        yield return Case(
            "@jack mentioned #dotnet today",
            (MatchKind.Mention, "@jack", 0, 5),
            (MatchKind.Hashtag, "#dotnet", 16, 7));

        yield return Case(
            "@example.com is not an email",
            (MatchKind.Mention, "@example", 0, 8));

        yield return Case(
            "user@example.com vs @example mention",
            (MatchKind.Email, "user@example.com", 0, 16),
            (MatchKind.Mention, "@example", 20, 8));

        yield return Case(
            "Visit HTTPS://EXAMPLE.COM now",
            (MatchKind.Url, "HTTPS://EXAMPLE.COM", 6, 19));

        yield return Case("Room #123 is booked");

        yield return Case("word@nodothost has no dot");

        yield return Case("same#tag glued together");

        yield return Case(
            "API at https://example.com:8080/v1/users?active=true",
            (MatchKind.Url, "https://example.com:8080/v1/users?active=true", 7, 45));

        yield return Case(
            "Search https://example.com/search?q=test&sort=asc here",
            (MatchKind.Url, "https://example.com/search?q=test&sort=asc", 7, 42));

        yield return Case(
            "Download https://example.com/report.txt for details",
            (MatchKind.Url, "https://example.com/report.txt", 9, 30));

        yield return Case(
            "Ping @alice or email bob@example.com about #project or visit example.org",
            (MatchKind.Mention, "@alice", 5, 6),
            (MatchKind.Email, "bob@example.com", 21, 15),
            (MatchKind.Hashtag, "#project", 43, 8),
            (MatchKind.Url, "example.org", 61, 11));

        yield return Case(
            "Is https://example.com. the right site?",
            (MatchKind.Url, "https://example.com", 3, 19));

        yield return Case("");

        yield return Case("   ");

        yield return Case("Plain text without anything to link here.");

        yield return Case(
            "Meet @caf\u00e9 for coffee and talk about #na\u00efve today",
            (MatchKind.Mention, "@caf\u00e9", 5, 5),
            (MatchKind.Hashtag, "#na\u00efve", 37, 6));

        yield return Case(
            "Z\u00fcrich visitors love example.com for maps",
            (MatchKind.Url, "example.com", 21, 11));

        yield return Case(
            "Visit example.com.Zzzqq for details",
            (MatchKind.Url, "example.com", 6, 11));

        yield return Case(
            "See https://en.wikipedia.org/wiki/Murphy's_law here",
            (MatchKind.Url, "https://en.wikipedia.org/wiki/Murphy's_law", 4, 42));

        yield return Case(
            "Malformed port https://example.com:80x here",
            (MatchKind.Url, "https://example.com", 15, 19));

        yield return Case(
            "@user@example.com is two matches",
            (MatchKind.Mention, "@user", 0, 5),
            (MatchKind.Url, "example.com", 6, 11));
    }

    private static object[] Case(string text, params (MatchKind Kind, string Value, int Index, int Length)[] expected)
    {
        var matches = new AutolinkMatch[expected.Length];
        for (var i = 0; i < expected.Length; i++)
        {
            var (kind, value, index, length) = expected[i];
            matches[i] = new AutolinkMatch(kind, value, index, length);
        }

        return [text, matches];
    }
}

