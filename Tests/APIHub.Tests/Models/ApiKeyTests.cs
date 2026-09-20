using ApiHub.Models;
using ErrorOr;

namespace ApiHub.Tests.Models;

public class ApiKeyTests {
    [Fact]
    public void 合法密钥通过() {
        ApiKey key = ApiKey.Create("sk-1").Value;

        Assert.Equal("sk-1", key.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void 空密钥被拒(string value) {
        ErrorOr<ApiKey> result = ApiKey.Create(value);

        Assert.True(result.IsError);
        Assert.Equal("ApiKey.Invalid", result.FirstError.Code);
    }

    [Fact]
    public void 文本形式为掩码不暴露原文() {
        ApiKey key = ApiKey.Create("sk-secret").Value;

        Assert.DoesNotContain("sk-secret", key.ToString());
    }
}
