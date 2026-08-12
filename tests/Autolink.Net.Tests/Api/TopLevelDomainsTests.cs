namespace Autolink.Net.Tests.Api;

public class TopLevelDomainsTests
{
    [Theory]
    [InlineData("com")]
    [InlineData("COM")]
    [InlineData("org")]
    [InlineData("net")]
    [InlineData("io")]
    [InlineData("dev")]
    [InlineData("ai")]
    [InlineData("co")]
    [InlineData("ng")]
    [InlineData("uk")]
    public void Known_contains_common_top_level_domains_case_insensitively(string tld)
    {
        Assert.Contains(tld, TopLevelDomains.Known);
    }

    [Theory]
    [InlineData("txt")]
    [InlineData("zip")]
    [InlineData("jpeg")]
    [InlineData("corpnet")]
    [InlineData("localhost")]
    public void Known_does_not_contain_common_non_domain_suffixes(string suffix)
    {
        Assert.DoesNotContain(suffix, TopLevelDomains.Known);
    }
}
