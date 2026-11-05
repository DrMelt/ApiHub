using ApiHub.Shared.Models;
using ErrorOr;

namespace ApiHub.Shared.Tests.Models;

public class ProviderNameTests
{
    [Theory]
    [InlineData("local")]
    [InlineData("DashScope")]
    [InlineData("bad id")]
    [InlineData("提供者")]
    [InlineData("https://example.com/v1")]
    public void 任意内容通过(string value)
    {
        ProviderName name = ProviderName.Create(value).Value;

        Assert.Equal(value, name.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void 空标识被拒(string value)
    {
        ErrorOr<ProviderName> result = ProviderName.Create(value);

        Assert.True(result.IsError);
        Assert.Equal("ProviderName.Invalid", result.FirstError.Code);
    }

    [Fact]
    public void 标识大小写敏感地判定()
    {
        ProviderName lower = ProviderName.Create("local").Value;
        ProviderName mixed = ProviderName.Create("Local").Value;

        Assert.Equal(lower, ProviderName.Create("local").Value);
        Assert.True(lower == ProviderName.Create("local").Value);
        Assert.NotEqual(lower, mixed);
        Assert.True(lower != mixed);
    }
}
