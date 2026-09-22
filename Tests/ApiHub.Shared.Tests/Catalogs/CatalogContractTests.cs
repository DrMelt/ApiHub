using ApiHub.Shared.Catalogs;
using ApiHub.Shared.Models;
using ApiHub.Tests.Fixture;
using System.Collections.Immutable;

namespace ApiHub.Shared.Tests.Catalogs;

public class CatalogContractTests {
    private sealed class StubCatalog(ImmutableArray<ProviderDefinition> providers, ImmutableArray<ModelDefinition> models) : IReadOnlyCatalog {
        private readonly ImmutableArray<ProviderDefinition> _providers = providers;
        private readonly ImmutableArray<ModelDefinition> _models = models;

        public IReadOnlyCollection<ProviderDefinition> Providers => _providers;
        public IReadOnlyCollection<ModelDefinition> Models => _models;
        public CatalogContents Contents => TestData.Contents([.. _providers], [.. _models]);

        public ModelDefinition? FindModel(ModelName modelName) =>
            _models.FirstOrDefault(model => model.ModelName == modelName);

        public ProviderDefinition? FindProvider(ProviderName providerName) =>
            _providers.FirstOrDefault(provider => provider.ProviderName == providerName);
    }

    private static StubCatalog Sample() => new(
        [TestData.Provider("dashscope", "https://example.com/v1", "sk-1")],
        [TestData.Model("qwen-plus", "dashscope")]);

    [Fact]
    public void 仅实现读取契约即可解析连接信息() {
        IReadOnlyCatalog catalog = Sample();

        ModelConnection connection = catalog.FindModelConnection(TestData.ModelName("qwen-plus"))!;

        Assert.Equal("qwen-plus", connection.ModelName.Value);
        Assert.Equal("dashscope", connection.ProviderName.Value);
        Assert.Equal("https://example.com/v1", connection.BaseAddress.Address.ToString());
        Assert.Equal("sk-1", connection.ApiKey.Value);
    }

    [Fact]
    public void 模型未命中时连接信息为空() {
        Assert.Null(Sample().FindModelConnection(TestData.ModelName("missing")));
    }

    [Fact]
    public void 提供商未命中时连接信息为空() {
        IReadOnlyCatalog catalog = new StubCatalog([], [TestData.Model("qwen-plus", "missing")]);

        Assert.Null(catalog.FindModelConnection(TestData.ModelName("qwen-plus")));
    }

    [Fact]
    public void 无内存目录参与可由定义构成连接信息() {
        ModelConnection connection = ModelConnection.Create(
            TestData.Model("qwen-plus", "dashscope"),
            TestData.Provider("dashscope", "https://example.com/v1", "sk-1"));

        Assert.Equal("qwen-plus", connection.ModelName.Value);
        Assert.Equal("sk-1", connection.ApiKey.Value);
    }
}
