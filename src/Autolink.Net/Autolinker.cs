namespace Autolink;

/// <summary>
/// Detects URLs, email addresses, mentions and hashtags in plain text, and renders HTML-safe
/// hyperlinks for them.
/// </summary>
public static class Autolinker
{
    /// <summary>
    /// Scans <paramref name="text"/> and returns every match it finds, in the order they occur.
    /// Matches never overlap: whichever kind is recognized first at a given position wins.
    /// </summary>
    /// <param name="text">The plain text to scan. Never modified.</param>
    /// <param name="options">
    /// Controls which match kinds are detected and which top-level domains are recognized.
    /// Defaults to <see cref="AutolinkOptions.Default"/> when omitted.
    /// </param>
    /// <returns>The matches found, in left-to-right order.</returns>
    public static IReadOnlyList<AutolinkMatch> FindMatches(string text, AutolinkOptions? options = null) =>
        AutolinkScanner.Scan(text, options ?? AutolinkOptions.Default);

    /// <summary>
    /// Scans <paramref name="text"/> and returns an HTML string in which every match is wrapped
    /// in an anchor tag, and all other text is HTML-escaped. The result is always safe to
    /// insert directly into an HTML document.
    /// </summary>
    /// <param name="text">The plain text to linkify. Never modified.</param>
    /// <param name="options">
    /// Controls which matches are detected and how their anchor tags are rendered. Defaults to
    /// <see cref="LinkifyOptions.Default"/> when omitted.
    /// </param>
    /// <returns>The HTML-safe, linkified text.</returns>
    public static string Linkify(string text, LinkifyOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(text);
        var effectiveOptions = options ?? LinkifyOptions.Default;
        var matches = FindMatches(text, effectiveOptions.MatchOptions);
        return HtmlLinkRenderer.Render(text, matches, effectiveOptions);
    }

    /// <summary>
    /// The default <see cref="LinkifyOptions.HrefResolver"/>. Builds an href for URL matches
    /// (prefixing bare domains with <c>https://</c>) and a <c>mailto:</c> href for email
    /// matches. Returns <see langword="null"/> for mentions and hashtags, since their target
    /// depends on the host platform and cannot be guessed generically.
    /// </summary>
    /// <param name="match">The match to resolve an href for.</param>
    /// <returns>The href to use, or <see langword="null"/> to leave the match as plain text.</returns>
    public static string? DefaultHrefResolver(AutolinkMatch match) => match.Kind switch
    {
        MatchKind.Url => BuildUrlHref(match.Value),
        MatchKind.Email => AutolinkConstants.MailtoScheme + match.Value,
        MatchKind.Mention => null,
        MatchKind.Hashtag => null,
        _ => null,
    };

    private static string BuildUrlHref(string value) => HasScheme(value)
        ? value
        : AutolinkConstants.DefaultBareUrlScheme + value;

    private static bool HasScheme(string value) =>
        value.StartsWith(AutolinkConstants.HttpScheme, StringComparison.OrdinalIgnoreCase) ||
        value.StartsWith(AutolinkConstants.HttpsScheme, StringComparison.OrdinalIgnoreCase);
}
