namespace Autolink;

/// <summary>
/// Identifies the kind of entity an <see cref="AutolinkMatch"/> represents.
/// </summary>
public enum MatchKind
{
    /// <summary>
    /// A web URL: an http or https address, or a bare domain with a known top-level domain.
    /// </summary>
    Url,

    /// <summary>
    /// An email address whose domain has a known top-level domain.
    /// </summary>
    Email,

    /// <summary>
    /// An at-mention, such as <c>@handle</c>.
    /// </summary>
    Mention,

    /// <summary>
    /// A hashtag, such as <c>#topic</c>.
    /// </summary>
    Hashtag,
}
