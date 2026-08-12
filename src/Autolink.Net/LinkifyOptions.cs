namespace Autolink;

/// <summary>
/// Controls how <see cref="Autolinker.Linkify"/> renders anchor tags for the matches it finds.
/// </summary>
public sealed class LinkifyOptions
{
    /// <summary>
    /// The options used to find matches before rendering them. Defaults to
    /// <see cref="AutolinkOptions.Default"/>.
    /// </summary>
    public AutolinkOptions MatchOptions { get; init; } = AutolinkOptions.Default;

    /// <summary>
    /// The value of the <c>rel</c> attribute to add to every rendered anchor, for example
    /// <c>"nofollow noopener"</c>. Defaults to <see langword="null"/> (attribute omitted).
    /// </summary>
    public string? Rel { get; init; }

    /// <summary>
    /// The value of the <c>target</c> attribute to add to every rendered anchor, for example
    /// <c>"_blank"</c>. Defaults to <see langword="null"/> (attribute omitted).
    /// </summary>
    public string? Target { get; init; }

    /// <summary>
    /// The value of the <c>class</c> attribute to add to every rendered anchor. Defaults to
    /// <see langword="null"/> (attribute omitted).
    /// </summary>
    public string? CssClass { get; init; }

    /// <summary>
    /// Resolves the <c>href</c> for a match. Returning <see langword="null"/> leaves the match
    /// as HTML-escaped plain text instead of wrapping it in an anchor. Defaults to
    /// <see cref="Autolinker.DefaultHrefResolver"/>, which links URLs and emails and leaves
    /// mentions and hashtags as plain text, since their target depends on the host platform.
    /// </summary>
    public Func<AutolinkMatch, string?> HrefResolver { get; init; } = Autolinker.DefaultHrefResolver;

    /// <summary>
    /// Produces extra attributes to add to a rendered anchor, keyed by attribute name. Called
    /// only for matches that will be linked (that is, <see cref="HrefResolver"/> returned a
    /// non-null value). Defaults to <see langword="null"/> (no extra attributes).
    /// </summary>
    public Func<AutolinkMatch, IReadOnlyDictionary<string, string>?>? AdditionalAttributes { get; init; }

    /// <summary>The default options: built-in match detection, no rel, target or class attributes.</summary>
    public static LinkifyOptions Default { get; } = new();
}
