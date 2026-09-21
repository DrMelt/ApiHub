using ApiHub.Models;
using ApiHub.Tests.Shared;

namespace ApiHub.Tests.Models;

public class ProviderDefinitionTests {
    [Fact]
    public void 给出端点则保留() {
        ProviderDefinition provider = TestData.Provider("p");

        Assert.Equal("https://example.com/v1", provider.BaseAddress.Address.ToString());
    }

    [Fact]
    public void 文本形式为掩码不暴露凭据() {
        ProviderDefinition provider = TestData.Provider("p", apiKey: "sk-secret");

        Assert.DoesNotContain("sk-secret", provider.ToString());
    }
}
