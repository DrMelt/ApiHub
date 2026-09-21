using ApiHub.Models;
using ErrorOr;

namespace ApiHub.Tests.Models;

public class ProviderEndpointTests {
    [Theory]
    [InlineData("https://example.com/v1")]
    [InlineData("http://127.0.0.1:11434/v1")]
    public void 合法端点通过(string value) {
        ProviderEndpoint endpoint = ProviderEndpoint.Create(value).Value;

        Assert.Equal(value, endpoint.Address.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData("example.com/v1")]
    [InlineData("/v1")]
    [InlineData("ftp://example.com")]
    public void 非法端点被拒(string value) {
        ErrorOr<ProviderEndpoint> result = ProviderEndpoint.Create(value);

        Assert.True(result.IsError);
        Assert.Equal("ProviderEndpoint.Invalid", result.FirstError.Code);
    }

    [Fact]
    public void 地址按Uri规范化() {
        ProviderEndpoint endpoint = ProviderEndpoint.Create("https://example.com").Value;

        Assert.Equal("https://example.com/", endpoint.Address.ToString());
    }
}
