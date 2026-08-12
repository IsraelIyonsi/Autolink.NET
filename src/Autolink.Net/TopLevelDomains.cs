namespace Autolink;

/// <summary>
/// The built-in set of top-level domains that <see cref="Autolinker"/> recognizes when
/// deciding whether a bare domain or an email address is linkable.
/// </summary>
public static class TopLevelDomains
{
    private static readonly string[] SourceList =
    [
        // Original generic top-level domains.
        "com", "org", "net", "edu", "gov", "mil", "int", "info", "biz", "name",
        "pro", "coop", "museum", "aero", "jobs", "mobi", "travel", "tel", "asia", "cat", "xxx", "post",

        // Country-code top-level domains.
        "ac", "ad", "ae", "af", "ag", "ai", "al", "am", "ao", "aq", "ar", "as", "at", "au", "aw", "ax", "az",
        "ba", "bb", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bm", "bn", "bo", "br", "bs", "bt", "bw", "by", "bz",
        "ca", "cc", "cd", "cf", "cg", "ch", "ci", "ck", "cl", "cm", "cn", "co", "cr", "cu", "cv", "cw", "cx", "cy", "cz",
        "de", "dj", "dk", "dm", "do", "dz",
        "ec", "ee", "eg", "er", "es", "et", "eu",
        "fi", "fj", "fk", "fm", "fo", "fr",
        "ga", "gd", "ge", "gg", "gh", "gi", "gl", "gm", "gn", "gp", "gq", "gr", "gs", "gt", "gu", "gw", "gy",
        "hk", "hn", "hr", "ht", "hu",
        "id", "ie", "il", "im", "in", "io", "iq", "ir", "is", "it",
        "je", "jm", "jo", "jp",
        "ke", "kg", "kh", "ki", "km", "kn", "kp", "kr", "kw", "ky", "kz",
        "la", "lb", "lc", "li", "lk", "lr", "ls", "lt", "lu", "lv", "ly",
        "ma", "mc", "md", "me", "mg", "mh", "mk", "ml", "mm", "mn", "mo", "mp", "mq", "mr", "ms", "mt", "mu", "mv", "mw", "mx", "my", "mz",
        "na", "nc", "ne", "nf", "ng", "ni", "nl", "no", "np", "nr", "nu", "nz",
        "om",
        "pa", "pe", "pf", "pg", "ph", "pk", "pl", "pm", "pn", "pr", "ps", "pt", "pw", "py",
        "qa",
        "re", "ro", "rs", "ru", "rw",
        "sa", "sb", "sc", "sd", "se", "sg", "sh", "si", "sk", "sl", "sm", "sn", "so", "sr", "st", "sv", "sx", "sy", "sz",
        "tc", "td", "tf", "tg", "th", "tj", "tk", "tl", "tm", "tn", "to", "tr", "tt", "tv", "tw", "tz",
        "ua", "ug", "uk", "us", "uy", "uz",
        "va", "vc", "ve", "vg", "vi", "vn", "vu",
        "wf", "ws",
        "ye", "yt",
        "za", "zm", "zw",

        // A conservative selection of modern generic top-level domains not already covered above.
        "app", "dev", "xyz", "tech", "site", "online", "store", "cloud", "digital",
        "network", "systems", "software", "codes", "design", "studio", "agency", "solutions",
    ];

    /// <summary>
    /// The built-in top-level domains, compared case-insensitively. Covers the original
    /// generic top-level domains, a broad set of country-code domains, and a conservative
    /// selection of modern generic domains that are unlikely to collide with ordinary
    /// English words in prose.
    /// </summary>
    public static IReadOnlySet<string> Known { get; } = new HashSet<string>(SourceList, StringComparer.OrdinalIgnoreCase);
}
