namespace Autolink;

/// <summary>
/// A single entity detected in plain text: its kind, the exact matched substring, and its
/// position within the original text.
/// </summary>
/// <param name="Kind">The kind of entity that was matched.</param>
/// <param name="Value">The exact substring of the source text that was matched.</param>
/// <param name="Index">The zero-based UTF-16 code unit offset where the match starts.</param>
/// <param name="Length">The length of the match, in UTF-16 code units.</param>
public readonly record struct AutolinkMatch(MatchKind Kind, string Value, int Index, int Length);
