using System.Text.RegularExpressions;
using Xunit;

namespace Resto.UI.Tests;

public sealed class DesignAssetTests
{
    private static string CssPath(string name) =>
        Path.Combine(AppContext.BaseDirectory, "DesignAssets", "css", name);

    [Fact]
    public void ThemeTokensMatchTheConstitution()
    {
        var css = File.ReadAllText(CssPath("tokens.css"));
        var darkMode = css.IndexOf("@media", StringComparison.Ordinal);
        Assert.True(darkMode > 0);
        var light = css[..darkMode];
        var dark = css[darkMode..];

        foreach (var (token, lightColor, darkColor) in new[]
        {
            ("navy", "#1B2B3F", "#2C4766"),
            ("copper", "#B8713A", "#D08A52"),
            ("bg", "#F6F4F0", "#0F151D"),
            ("surface", "#FFFFFF", "#17202B"),
            ("ok", "#2E7D5B", "#55B387"),
            ("warn", "#9A6512", "#E3AA45"),
            ("bad", "#C0392B", "#E5705F")
        })
        {
            Assert.Matches($@"--{token}:\s*{lightColor}\b", light);
            Assert.Matches($@"--{token}:\s*{darkColor}\b", dark);
        }
    }

    [Fact]
    public void AllDeclaredFontFacesUseBundledFontFiles()
    {
        var path = CssPath("base.css");
        var css = File.ReadAllText(path);
        var sources = Regex.Matches(css, @"url\(['""]?([^'"")]+)['""]?\)");
        Assert.Equal(4, sources.Count);
        foreach (Match source in sources)
        {
            var url = source.Groups[1].Value;
            Assert.StartsWith("../fonts/", url);
            var font = File.ReadAllBytes(Path.GetFullPath(url, Path.GetDirectoryName(path)!));
            Assert.Equal(new byte[] { 0, 1, 0, 0 }, font[..4]);
        }
    }

    [Theory]
    [InlineData("btn")]
    [InlineData("pill")]
    [InlineData("chip")]
    [InlineData("kpi")]
    [InlineData("tbl")]
    [InlineData("panel")]
    [InlineData("modal")]
    [InlineData("seg")]
    [InlineData("toggle")]
    public void SharedComponentSelectorIsDefined(string name)
    {
        var css = File.ReadAllText(CssPath("components.css"));
        Assert.Matches($@"\.{name}\s*\{{", css);
    }
}
