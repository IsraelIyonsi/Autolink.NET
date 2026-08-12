namespace Autolink.Net.Tests.Linkifying;

public class LinkifyFixtureTests
{
    [Theory]
    [MemberData(nameof(LinkifyFixtures.Cases), MemberType = typeof(LinkifyFixtures))]
    public void Linkify_produces_exact_html_for_fixture(string text, string expectedHtml)
    {
        var actualHtml = Autolinker.Linkify(text);

        Assert.Equal(expectedHtml, actualHtml);
    }
}
