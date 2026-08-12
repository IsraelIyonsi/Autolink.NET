using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Autolink;

/// <summary>
/// Renders a matched span of text into an HTML string, escaping surrounding plain text and
/// every attribute value so the output is always injection-safe.
/// </summary>
internal static partial class HtmlLinkRenderer
{
    private const char AttributeQuote = '"';
    private const string AnchorOpenTag = "<a href=\"";
    private const string AnchorCloseTag = "</a>";
    private const string ClassAttribute = " class=\"";
    private const string RelAttribute = " rel=\"";
    private const string TargetAttribute = " target=\"";

    internal static string Render(string text, IReadOnlyList<AutolinkMatch> matches, LinkifyOptions options)
    {
        if (matches.Count == 0)
        {
            return WebUtility.HtmlEncode(text);
        }

        var builder = new StringBuilder(text.Length);
        var cursor = 0;

        foreach (var match in matches)
        {
            if (match.Index > cursor)
            {
                builder.Append(WebUtility.HtmlEncode(text.Substring(cursor, match.Index - cursor)));
            }

            AppendMatch(builder, match, options);
            cursor = match.Index + match.Length;
        }

        if (cursor < text.Length)
        {
            builder.Append(WebUtility.HtmlEncode(text.Substring(cursor)));
        }

        return builder.ToString();
    }

    private static void AppendMatch(StringBuilder builder, AutolinkMatch match, LinkifyOptions options)
    {
        var encodedValue = WebUtility.HtmlEncode(match.Value);
        var href = options.HrefResolver(match);

        if (href is null)
        {
            builder.Append(encodedValue);
            return;
        }

        builder.Append(AnchorOpenTag).Append(WebUtility.HtmlEncode(href)).Append(AttributeQuote);
        AppendAttributeIfPresent(builder, ClassAttribute, options.CssClass);
        AppendAttributeIfPresent(builder, RelAttribute, options.Rel);
        AppendAttributeIfPresent(builder, TargetAttribute, options.Target);
        AppendAdditionalAttributes(builder, options.AdditionalAttributes?.Invoke(match));

        builder.Append('>').Append(encodedValue).Append(AnchorCloseTag);
    }

    private static void AppendAttributeIfPresent(StringBuilder builder, string attributePrefix, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        builder.Append(attributePrefix).Append(WebUtility.HtmlEncode(value)).Append(AttributeQuote);
    }

    private static void AppendAdditionalAttributes(StringBuilder builder, IReadOnlyDictionary<string, string>? attributes)
    {
        if (attributes is null)
        {
            return;
        }

        foreach (var (name, value) in attributes)
        {
            if (!AttributeNamePattern().IsMatch(name))
            {
                throw new ArgumentException(
                    $"'{name}' is not a valid HTML attribute name. Attribute names must start with a " +
                    "letter, underscore or colon, and contain only letters, digits, hyphens, underscores, " +
                    "colons and periods.",
                    nameof(attributes));
            }

            builder.Append(' ')
                .Append(name)
                .Append("=\"")
                .Append(WebUtility.HtmlEncode(value))
                .Append(AttributeQuote);
        }
    }

    // A conservative, safe subset of the HTML attribute name grammar: no spaces, '=', quotes or
    // other characters that could let a crafted name escape out of the attribute position (for
    // example a name like "onclick=alert(1) x" that would otherwise render a second, live
    // attribute). HtmlEncode alone does not touch spaces or '=', so the name is validated
    // outright instead of merely encoded.
    [GeneratedRegex(@"^[A-Za-z_:][A-Za-z0-9_:.-]*$", RegexOptions.CultureInvariant)]
    private static partial Regex AttributeNamePattern();
}
