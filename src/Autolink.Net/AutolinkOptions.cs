namespace Autolink;

/// <summary>
/// Controls which kinds of matches <see cref="Autolinker.FindMatches"/> and
/// <see cref="Autolinker.Linkify"/> detect, and lets callers extend the built-in
/// top-level domain set used for bare-domain URL and email detection.
/// </summary>
public sealed class AutolinkOptions
{
    /// <summary>
    /// Whether to detect http and https URLs, and bare domains with a known top-level domain.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool DetectUrls { get; init; } = true;

    /// <summary>
    /// Whether to detect email addresses whose domain has a known top-level domain.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool DetectEmails { get; init; } = true;

    /// <summary>
    /// Whether to detect at-mentions such as <c>@handle</c>. Defaults to <see langword="true"/>.
    /// </summary>
    public bool DetectMentions { get; init; } = true;

    /// <summary>
    /// Whether to detect hashtags such as <c>#topic</c>. Defaults to <see langword="true"/>.
    /// </summary>
    public bool DetectHashtags { get; init; } = true;

    /// <summary>
    /// Extra top-level domains to recognize in addition to <see cref="TopLevelDomains.Known"/>.
    /// Compared case-insensitively. Defaults to <see langword="null"/> (no extras).
    /// </summary>
    public IReadOnlyCollection<string>? AdditionalTopLevelDomains { get; init; }

    /// <summary>The default options: every match kind enabled, using the built-in top-level domain set only.</summary>
    public static AutolinkOptions Default { get; } = new();

    /// <summary>
    /// Determines whether <paramref name="candidate"/> is a recognized top-level domain,
    /// checking both the built-in set and <see cref="AdditionalTopLevelDomains"/>.
    /// </summary>
    /// <param name="candidate">The candidate top-level domain label, without a leading dot.</param>
    internal bool IsKnownTopLevelDomain(string candidate)
    {
        if (TopLevelDomains.Known.Contains(candidate))
        {
            return true;
        }

        if (AdditionalTopLevelDomains is null)
        {
            return false;
        }

        foreach (var tld in AdditionalTopLevelDomains)
        {
            if (string.Equals(tld, candidate, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
