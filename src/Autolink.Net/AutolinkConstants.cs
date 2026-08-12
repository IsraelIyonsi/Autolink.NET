namespace Autolink;

/// <summary>
/// Named values shared by the matching engine. Kept internal because they describe
/// implementation choices, not part of the public contract.
/// </summary>
internal static class AutolinkConstants
{
    /// <summary>The literal scheme prefix that marks an http URL.</summary>
    internal const string HttpScheme = "http://";

    /// <summary>The literal scheme prefix that marks an https URL.</summary>
    internal const string HttpsScheme = "https://";

    /// <summary>The scheme prefix used to build a mailto href for an email match.</summary>
    internal const string MailtoScheme = "mailto:";

    /// <summary>The scheme prepended to a bare-domain URL when building its href.</summary>
    internal const string DefaultBareUrlScheme = "https://";

    /// <summary>The character that introduces a mention.</summary>
    internal const char MentionTrigger = '@';

    /// <summary>The character that introduces a hashtag.</summary>
    internal const char HashtagTrigger = '#';

    /// <summary>The separator between domain labels.</summary>
    internal const char DomainLabelSeparator = '.';

    /// <summary>An opening parenthesis, used when balancing trailing parentheses in a URL.</summary>
    internal const char OpenParenthesis = '(';

    /// <summary>A closing parenthesis, used when balancing trailing parentheses in a URL.</summary>
    internal const char CloseParenthesis = ')';

    /// <summary>The fewest dot-separated labels a domain must have to be considered a host.</summary>
    internal const int MinDomainLabelCount = 2;

    /// <summary>The shortest a top-level domain label may be.</summary>
    internal const int MinTopLevelDomainLength = 2;

    /// <summary>The fewest characters a mention handle must have after the trigger character.</summary>
    internal const int MinMentionLength = 1;

    /// <summary>The fewest characters a hashtag body must have after the trigger character.</summary>
    internal const int MinHashtagLength = 1;

    /// <summary>
    /// Trailing characters trimmed from the end of a URL or email match when they are not
    /// balanced by a matching opener within the match, mirroring common autolinker behavior.
    /// </summary>
    internal const string TrailingTrimCharacters = ".,;:!?\"'*";
}
