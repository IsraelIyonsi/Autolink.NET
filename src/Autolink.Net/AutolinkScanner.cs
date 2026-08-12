using System.Text.RegularExpressions;

namespace Autolink;

/// <summary>
/// The matching engine behind <see cref="Autolinker.FindMatches"/>. Scans text left to right,
/// at each position trying the enabled match kinds in priority order: an http/https URL, then
/// an email address, then a bare-domain URL, then a mention, then a hashtag. The first kind that
/// matches wins and the scan resumes immediately after it, so matches never overlap.
/// </summary>
internal static partial class AutolinkScanner
{
    internal static IReadOnlyList<AutolinkMatch> Scan(string text, AutolinkOptions options)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(options);

        var matches = new List<AutolinkMatch>();
        var index = 0;

        while (index < text.Length)
        {
            if (!HasWordBoundaryBefore(text, index))
            {
                index++;
                continue;
            }

            if (TryMatchAt(text, index, options, out var match))
            {
                matches.Add(match);
                index = match.Index + match.Length;
                continue;
            }

            index++;
        }

        return matches;
    }

    private static bool TryMatchAt(string text, int index, AutolinkOptions options, out AutolinkMatch match)
    {
        if (options.DetectUrls && HasSchemeAt(text, index, out var scheme) &&
            TryMatchSchemeUrl(text, index, scheme, out match))
        {
            return true;
        }

        var c = text[index];

        if (char.IsLetterOrDigit(c))
        {
            if (options.DetectEmails && TryMatchEmail(text, index, options, out match))
            {
                return true;
            }

            if (options.DetectUrls && TryMatchBareDomainUrl(text, index, options, out match))
            {
                return true;
            }
        }
        else if (c == AutolinkConstants.MentionTrigger)
        {
            if (options.DetectMentions && TryMatchMention(text, index, out match))
            {
                return true;
            }
        }
        else if (c == AutolinkConstants.HashtagTrigger)
        {
            if (options.DetectHashtags && TryMatchHashtag(text, index, out match))
            {
                return true;
            }
        }

        match = default;
        return false;
    }

    private static bool HasSchemeAt(string text, int index, out string scheme)
    {
        var remaining = text.AsSpan(index);

        if (remaining.StartsWith(AutolinkConstants.HttpsScheme, StringComparison.OrdinalIgnoreCase))
        {
            scheme = AutolinkConstants.HttpsScheme;
            return true;
        }

        if (remaining.StartsWith(AutolinkConstants.HttpScheme, StringComparison.OrdinalIgnoreCase))
        {
            scheme = AutolinkConstants.HttpScheme;
            return true;
        }

        scheme = string.Empty;
        return false;
    }

    private static bool TryMatchSchemeUrl(string text, int index, string scheme, out AutolinkMatch match)
    {
        match = default;
        var hostStart = index + scheme.Length;

        if (!TryScanHost(text, hostStart, out var afterHost))
        {
            return false;
        }

        var pos = afterHost;
        pos += ScanPort(text, pos);
        pos += ScanPath(text, pos);

        var rawLength = pos - index;
        var trimmedLength = TrimTrailingBoundary(text, index, rawLength);

        if (trimmedLength <= scheme.Length)
        {
            return false;
        }

        match = new AutolinkMatch(MatchKind.Url, text.Substring(index, trimmedLength), index, trimmedLength);
        return true;
    }

    private static bool TryMatchBareDomainUrl(string text, int index, AutolinkOptions options, out AutolinkMatch match)
    {
        match = default;

        if (!TryScanKnownDomain(text, index, options, out var afterHost))
        {
            return false;
        }

        var pos = afterHost;
        pos += ScanPort(text, pos);
        pos += ScanPath(text, pos);

        var rawLength = pos - index;
        var trimmedLength = TrimTrailingBoundary(text, index, rawLength);

        if (trimmedLength <= 0)
        {
            return false;
        }

        match = new AutolinkMatch(MatchKind.Url, text.Substring(index, trimmedLength), index, trimmedLength);
        return true;
    }

    private static bool TryMatchEmail(string text, int index, AutolinkOptions options, out AutolinkMatch match)
    {
        match = default;

        var localPartMatch = LocalPartPattern().Match(text, index);
        if (!localPartMatch.Success || localPartMatch.Index != index)
        {
            return false;
        }

        var atIndex = index + localPartMatch.Length;
        if (atIndex >= text.Length || text[atIndex] != AutolinkConstants.MentionTrigger)
        {
            return false;
        }

        var domainStart = atIndex + 1;
        if (!TryScanKnownDomain(text, domainStart, options, out var domainEnd))
        {
            return false;
        }

        var rawLength = domainEnd - index;
        var trimmedLength = TrimTrailingBoundary(text, index, rawLength);
        var minimumLength = localPartMatch.Length + 1;

        if (trimmedLength <= minimumLength)
        {
            return false;
        }

        match = new AutolinkMatch(MatchKind.Email, text.Substring(index, trimmedLength), index, trimmedLength);
        return true;
    }

    private static bool TryMatchMention(string text, int index, out AutolinkMatch match)
    {
        match = default;
        var bodyStart = index + 1;
        var pos = bodyStart;

        while (pos < text.Length && IsWordChar(text[pos]))
        {
            pos++;
        }

        var bodyLength = pos - bodyStart;
        if (bodyLength < AutolinkConstants.MinMentionLength)
        {
            return false;
        }

        var length = pos - index;
        match = new AutolinkMatch(MatchKind.Mention, text.Substring(index, length), index, length);
        return true;
    }

    private static bool TryMatchHashtag(string text, int index, out AutolinkMatch match)
    {
        match = default;
        var bodyStart = index + 1;
        var pos = bodyStart;
        var hasLetter = false;

        while (pos < text.Length && IsWordChar(text[pos]))
        {
            if (char.IsLetter(text[pos]))
            {
                hasLetter = true;
            }

            pos++;
        }

        var bodyLength = pos - bodyStart;
        if (bodyLength < AutolinkConstants.MinHashtagLength || !hasLetter)
        {
            return false;
        }

        var length = pos - index;
        match = new AutolinkMatch(MatchKind.Hashtag, text.Substring(index, length), index, length);
        return true;
    }

    private static bool TryScanHost(string text, int start, out int end)
    {
        end = start;
        var hostMatch = HostPattern().Match(text, start);

        if (!hostMatch.Success || hostMatch.Index != start || hostMatch.Length == 0)
        {
            return false;
        }

        end = start + hostMatch.Length;
        return true;
    }

    private static bool TryScanKnownDomain(string text, int start, AutolinkOptions options, out int end)
    {
        end = start;

        if (!TryScanHost(text, start, out var hostEnd))
        {
            return false;
        }

        var host = text.Substring(start, hostEnd - start);
        var labels = host.Split(AutolinkConstants.DomainLabelSeparator);

        if (labels.Length < AutolinkConstants.MinDomainLabelCount)
        {
            return false;
        }

        // The host was scanned greedily and may have swallowed trailing labels that are not a
        // real top-level domain (e.g. "example.com.Zzzqq"). Back off one label at a time, from
        // the full greedy host down to the shortest two-label host, so a known TLD earlier in
        // the string still produces a match instead of rejecting the whole candidate outright.
        // This mirrors the backtracking host scan linkify-it style autolinkers use.
        var labelSpanLength = 0;
        for (var i = 0; i < labels.Length; i++)
        {
            labelSpanLength += labels[i].Length;
            if (i > 0)
            {
                labelSpanLength++;
            }
        }

        for (var labelCount = labels.Length; labelCount >= AutolinkConstants.MinDomainLabelCount; labelCount--)
        {
            var topLevelDomain = labels[labelCount - 1];

            if (topLevelDomain.Length >= AutolinkConstants.MinTopLevelDomainLength &&
                options.IsKnownTopLevelDomain(topLevelDomain))
            {
                end = start + labelSpanLength;
                return true;
            }

            labelSpanLength -= labels[labelCount - 1].Length + 1;
        }

        return false;
    }

    private static int ScanPort(string text, int start)
    {
        var portMatch = PortPattern().Match(text, start);
        if (!portMatch.Success || portMatch.Index != start)
        {
            return 0;
        }

        // A port must end cleanly: digits immediately followed by another word character (as in
        // "https://example.com:80x") are not a real port, so reject the whole port component
        // rather than gluing an unrelated suffix onto the end of the match.
        var afterPort = start + portMatch.Length;
        if (afterPort < text.Length && IsWordChar(text[afterPort]))
        {
            return 0;
        }

        return portMatch.Length;
    }

    private static int ScanPath(string text, int start)
    {
        var pathMatch = PathPattern().Match(text, start);
        return pathMatch.Success && pathMatch.Index == start ? pathMatch.Length : 0;
    }

    private static int TrimTrailingBoundary(string text, int start, int length)
    {
        var end = start + length;

        while (end > start)
        {
            var last = text[end - 1];

            if (last == AutolinkConstants.CloseParenthesis)
            {
                if (CountUnbalancedCloseParenthesis(text, start, end) > 0)
                {
                    end--;
                    continue;
                }

                break;
            }

            if (AutolinkConstants.TrailingTrimCharacters.IndexOf(last) >= 0)
            {
                end--;
                continue;
            }

            break;
        }

        return end - start;
    }

    private static int CountUnbalancedCloseParenthesis(string text, int start, int end)
    {
        var depth = 0;

        for (var i = start; i < end; i++)
        {
            if (text[i] == AutolinkConstants.OpenParenthesis)
            {
                depth++;
            }
            else if (text[i] == AutolinkConstants.CloseParenthesis)
            {
                depth--;
            }
        }

        return -depth;
    }

    private static bool HasWordBoundaryBefore(string text, int index) =>
        index == 0 || !IsWordChar(text[index - 1]);

    private static bool IsWordChar(char c) => char.IsLetterOrDigit(c) || c == '_';

    [GeneratedRegex(
        @"\G[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?(?:\.[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?)*",
        RegexOptions.CultureInvariant)]
    private static partial Regex HostPattern();

    [GeneratedRegex(
        @"\G[A-Za-z0-9](?:[A-Za-z0-9._%+-]*[A-Za-z0-9])?",
        RegexOptions.CultureInvariant)]
    private static partial Regex LocalPartPattern();

    [GeneratedRegex(@"\G:[0-9]+", RegexOptions.CultureInvariant)]
    private static partial Regex PortPattern();

    [GeneratedRegex(
        @"\G[/?#][A-Za-z0-9\-._~:/?#@!$&*+,;=%()']*",
        RegexOptions.CultureInvariant)]
    private static partial Regex PathPattern();
}
