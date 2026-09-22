using ApiHub.Catalogs;
using ApiHub.Shared.Catalogs;
using ApiHub.Shared.Models;
using ApiHub.Tests.Fixture;
using ErrorOr;
using System.Collections.Immutable;

namespace ApiHub.Tests.Catalogs;

public class CatalogTests {
    private static Catalog Sample() => new(TestData.Contents(
        [TestData.Provider("dashscope", "https://example.com/v1", "sk-1")],
        [TestData.Model("qwen-plus"), TestData.Model("qwen-max")]));

    private static Catalog CatalogOf(
        ImmutableArray<ProviderDefinition> providers,
        ImmutableArray<ModelDefinition> models) =>
        new(TestData.Ok(CatalogContents.Create(providers, models)));

    private static string[] ProviderNames(Catalog catalog) =>
        [.. catalog.Providers.Select(provider => provider.ProviderName.Value)];

    private static string[] ModelNames(Catalog catalog) =>
        [.. catalog.Models.Select(model => model.ModelName.Value)];

    [Fact]
    public void 查找模型命中其归属提供商() {
        Catalog catalog = Sample();

        ModelDefinition model = catalog.FindModel(TestData.ModelName("qwen-plus"))!;

        Assert.Equal("dashscope", model.ProviderName.Value);
        Assert.Equal("sk-1", catalog.FindProvider(model.ProviderName)!.ApiKey.Value);
    }

    [Fact]
    public void 查找模型名大小写敏感() {
        Assert.Null(Sample().FindModel(TestData.ModelName("Qwen-Plus")));
    }

    [Fact]
    public void 查找提供商名不区分大小写() {
        Assert.NotNull(Sample().FindProvider(TestData.ProviderName("DASHSCOPE")));
    }

    [Fact]
    public void 查找未知模型与未知提供商返回空() {
        Catalog catalog = Sample();

        Assert.Null(catalog.FindModel(TestData.ModelName("gpt-4o")));
        Assert.Null(catalog.FindProvider(TestData.ProviderName("missing")));
    }

    [Fact]
    public void 查找连接信息给出接入所需全部信息() {
        ModelConnection connection = Sample().FindModelConnection(TestData.ModelName("qwen-plus"))!;

        Assert.Equal("qwen-plus", connection.ModelName.Value);
        Assert.Equal("dashscope", connection.ProviderName.Value);
        Assert.Equal("https://example.com/v1", connection.BaseAddress.Address.ToString());
        Assert.Equal("sk-1", connection.ApiKey.Value);
    }

    [Fact]
    public void 查找连接信息未命中返回空() {
        Assert.Null(Sample().FindModelConnection(TestData.ModelName("gpt-4o")));
    }

    [Fact]
    public void 查找连接信息模型名大小写敏感() {
        Assert.Null(Sample().FindModelConnection(TestData.ModelName("Qwen-Plus")));
    }

    [Fact]
    public void 模型改指向另一提供商后连接信息随新提供商() {
        Catalog catalog = CatalogOf(
            [
                TestData.Provider("a", "https://example.com/a", "k1"),
                TestData.Provider("b", "https://example.com/b", "k2")
            ],
            [TestData.Model("m", "a")]);

        Assert.False(catalog.ReplaceModel(TestData.Model("m", "b")).IsError);

        ModelConnection connection = catalog.FindModelConnection(TestData.ModelName("m"))!;

        Assert.Equal("b", connection.ProviderName.Value);
        Assert.Equal("https://example.com/b", connection.BaseAddress.Address.ToString());
        Assert.Equal("k2", connection.ApiKey.Value);
    }

    [Fact]
    public void 替换提供商后连接信息随新凭据() {
        Catalog catalog = Sample();
        ProviderDefinition replacement = TestData.Provider("DashScope", "https://example.com/other", "sk-9");

        Assert.False(catalog.ReplaceProvider(replacement).IsError);

        ModelConnection connection = catalog.FindModelConnection(TestData.ModelName("qwen-plus"))!;

        Assert.Equal("https://example.com/other", connection.BaseAddress.Address.ToString());
        Assert.Equal("sk-9", connection.ApiKey.Value);
    }

    [Fact]
    public void 删除模型后不再命中查找() {
        Catalog catalog = Sample();

        Assert.False(catalog.RemoveModel(TestData.ModelName("qwen-max")).IsError);

        Assert.Null(catalog.FindModel(TestData.ModelName("qwen-max")));
        Assert.Null(catalog.FindModelConnection(TestData.ModelName("qwen-max")));
    }

    [Fact]
    public void 新增提供商就地生效() {
        Catalog catalog = Sample();

        Assert.False(catalog.AddProvider(TestData.Provider("local", "https://example.com/v1", "sk-2")).IsError);

        Assert.Contains("local", ProviderNames(catalog));
    }

    [Fact]
    public void 新增提供商失败状态不变() {
        Catalog catalog = Sample();
        string[] providers = ProviderNames(catalog);

        Assert.True(catalog.AddProvider(TestData.Provider("DashScope", "https://example.com/v1", "sk-2")).IsError);

        Assert.Equal(providers, ProviderNames(catalog));
    }

    [Fact]
    public void 新增已存在的提供商被拒() {
        ProviderDefinition provider = TestData.Provider("DashScope", "https://example.com/v1", "sk-1");

        ErrorOr<Success> result = Sample().AddProvider(provider);

        Assert.True(result.IsError);
        Assert.Equal("Catalog.ProviderAlreadyExists", result.FirstError.Code);
    }

    [Fact]
    public void 替换提供商就地生效() {
        Catalog catalog = Sample();
        ProviderDefinition replacement = TestData.Provider("DashScope", "https://example.com/other", "sk-9");

        Assert.False(catalog.ReplaceProvider(replacement).IsError);

        Assert.Single(catalog.Providers);
        Assert.Equal("sk-9", catalog.FindProvider(TestData.ProviderName("DashScope"))!.ApiKey.Value);
    }

    [Fact]
    public void 替换不存在的提供商被拒() {
        ErrorOr<Success> result = Sample().ReplaceProvider(TestData.Provider("missing"));

        Assert.True(result.IsError);
        Assert.Equal("Catalog.ProviderNotFound", result.FirstError.Code);
    }

    [Fact]
    public void 删除提供商后不再在目录中() {
        Catalog catalog = CatalogOf([TestData.Provider("local")], []);

        Assert.False(catalog.RemoveProvider(TestData.ProviderName("local")).IsError);

        Assert.Empty(catalog.Providers);
    }

    [Fact]
    public void 删除仍被模型引用的提供商被拒() {
        ErrorOr<Success> result = Sample().RemoveProvider(TestData.ProviderName("dashscope"));

        Assert.True(result.IsError);
        Assert.Equal("Catalog.ProviderInUse", result.FirstError.Code);
    }

    [Fact]
    public void 删除提供商失败状态不变() {
        Catalog catalog = Sample();
        string[] providers = ProviderNames(catalog);

        Assert.True(catalog.RemoveProvider(TestData.ProviderName("dashscope")).IsError);

        Assert.Equal(providers, ProviderNames(catalog));
    }

    [Fact]
    public void 删除不存在的提供商被拒() {
        ErrorOr<Success> result = Sample().RemoveProvider(TestData.ProviderName("missing"));

        Assert.True(result.IsError);
        Assert.Equal("Catalog.ProviderNotFound", result.FirstError.Code);
    }

    [Fact]
    public void 新增模型可被查找命中() {
        Catalog catalog = Sample();

        Assert.False(catalog.AddModel(TestData.Model("qwen3")).IsError);

        Assert.Equal("dashscope", catalog.FindModel(TestData.ModelName("qwen3"))!.ProviderName.Value);
    }

    [Fact]
    public void 模型名大小写不同视为新模型() {
        Catalog catalog = Sample();

        Assert.False(catalog.AddModel(TestData.Model("Qwen-Plus")).IsError);

        Assert.Equal(3, catalog.Models.Count);
        Assert.NotNull(catalog.FindModel(TestData.ModelName("Qwen-Plus")));
    }

    [Fact]
    public void 新增模型引用不存在的提供商被拒() {
        ErrorOr<Success> result = Sample().AddModel(TestData.Model("m", "missing"));

        Assert.True(result.IsError);
        Assert.Equal("Catalog.ProviderNotFound", result.FirstError.Code);
    }

    [Fact]
    public void 新增模型失败状态不变() {
        Catalog catalog = Sample();
        string[] models = ModelNames(catalog);

        Assert.True(catalog.AddModel(TestData.Model("m", "missing")).IsError);

        Assert.Equal(models, ModelNames(catalog));
    }

    [Fact]
    public void 新增已存在的模型被拒() {
        ErrorOr<Success> result = Sample().AddModel(TestData.Model("qwen-plus"));

        Assert.True(result.IsError);
        Assert.Equal("Catalog.ModelAlreadyExists", result.FirstError.Code);
    }

    [Fact]
    public void 替换模型改指向另一提供商() {
        Catalog catalog = CatalogOf(
            [
                TestData.Provider("a", "https://example.com/v1", "k1"),
                TestData.Provider("b", "https://example.com/v1", "k2")
            ],
            [TestData.Model("m", "a")]);

        Assert.False(catalog.ReplaceModel(TestData.Model("m", "b")).IsError);

        ModelDefinition model = catalog.FindModel(TestData.ModelName("m"))!;

        Assert.Equal("b", model.ProviderName.Value);
        Assert.Equal("k2", catalog.FindProvider(model.ProviderName)!.ApiKey.Value);
    }

    [Fact]
    public void 替换不存在的模型被拒() {
        ErrorOr<Success> result = Sample().ReplaceModel(TestData.Model("missing"));

        Assert.True(result.IsError);
        Assert.Equal("Catalog.ModelNotFound", result.FirstError.Code);
    }

    [Fact]
    public void 替换模型改指向不存在的提供商被拒() {
        ErrorOr<Success> result = Sample().ReplaceModel(TestData.Model("qwen-plus", "missing"));

        Assert.True(result.IsError);
        Assert.Equal("Catalog.ProviderNotFound", result.FirstError.Code);
    }

    [Fact]
    public void 变更操作与内容校验给出同一错误() {
        ErrorOr<CatalogContents> fromContents = CatalogContents.Create(
            [TestData.Provider("dashscope", "https://example.com/v1", "sk-1")],
            [TestData.Model("m", "missing")]);
        ErrorOr<Success> fromChange = Sample().AddModel(TestData.Model("m", "missing"));

        Assert.True(fromChange.IsError);
        Assert.Equal(fromContents.FirstError.Code, fromChange.FirstError.Code);
    }

    [Fact]
    public void 删除不存在的模型被拒() {
        ErrorOr<Success> result = Sample().RemoveModel(TestData.ModelName("missing"));

        Assert.True(result.IsError);
        Assert.Equal("Catalog.ModelNotFound", result.FirstError.Code);
    }

    [Fact]
    public void 从内容构造目录并查找命中() {
        CatalogContents contents = TestData.Contents(
            [TestData.Provider("dashscope", "https://example.com/v1", "sk-1")],
            [TestData.Model("qwen-plus")]);

        Catalog catalog = new(contents);

        Assert.Equal("sk-1", catalog.FindProvider(TestData.ProviderName("dashscope"))!.ApiKey.Value);
    }

    [Fact]
    public void 空目录没有提供商与模型() {
        Catalog catalog = new();

        Assert.Empty(catalog.Providers);
        Assert.Empty(catalog.Models);
    }

    [Fact]
    public void 文档示例构造目录并查找() {
        Catalog catalog = new();

        ErrorOr<Success> added = ProviderName.Create("dashscope")
            .Then(providerName => ApiKey.Create("sk-...")
                .Then(key => ProviderEndpoint.Create("https://dashscope.aliyuncs.com/compatible-mode/v1")
                    .Then(endpoint => catalog.AddProvider(ProviderDefinition.Create(providerName, endpoint, key)))));
        ErrorOr<Success> registered = ProviderName.Create("dashscope")
            .Then(providerName => ModelName.Create("qwen-plus")
                .Then(modelName => catalog.AddModel(ModelDefinition.Create(modelName, providerName))));

        ModelDefinition? model = catalog.FindModel(TestData.ModelName("qwen-plus"));
        ProviderDefinition? provider = model is null ? null : catalog.FindProvider(model.ProviderName);

        Assert.False(added.IsError);
        Assert.False(registered.IsError);
        Assert.Equal("sk-...", provider!.ApiKey.Value);
    }

    [Fact]
    public void 导出内容后再构造保持原有查找结果() {
        Catalog catalog = Sample();

        Catalog rebuilt = new(catalog.Contents);

        Assert.Equal(2, rebuilt.Models.Count);
        Assert.NotNull(rebuilt.FindModel(TestData.ModelName("qwen-max")));
        Assert.Equal("sk-1", rebuilt.FindProvider(TestData.ProviderName("dashscope"))!.ApiKey.Value);
    }

    [Fact]
    public void 导出的集合与目录内部不共享() {
        Catalog catalog = Sample();

        List<ProviderDefinition> providers = [.. catalog.Contents.Providers];
        providers.Clear();

        Assert.Single(catalog.Contents.Providers);
    }

    [Fact]
    public void 导出的快照在后续变更后保持旧内容() {
        Catalog catalog = Sample();
        CatalogContents snapshot = catalog.Contents;

        Assert.False(catalog.AddProvider(TestData.Provider("local", "https://example.com/v1", "sk-2")).IsError);

        Assert.Single(snapshot.Providers);
        Assert.Equal(2, catalog.Contents.Providers.Length);
    }

    [Fact]
    public void 读取的集合随目录变更反映新状态() {
        Catalog catalog = Sample();
        IReadOnlyCollection<ProviderDefinition> providers = catalog.Providers;

        Assert.False(catalog.AddProvider(TestData.Provider("local", "https://example.com/v1", "sk-2")).IsError);

        Assert.Equal(2, providers.Count);
    }

    [Fact]
    public void 内存目录可作为读写契约使用() {
        Catalog catalog = new();

        Assert.False(catalog.AddProvider(TestData.Provider("dashscope")).IsError);
        Assert.False(catalog.AddModel(TestData.Model("qwen-plus")).IsError);

        Assert.Equal("dashscope", catalog.FindModel(TestData.ModelName("qwen-plus"))!.ProviderName.Value);
        Assert.Equal("k", catalog.FindModelConnection(TestData.ModelName("qwen-plus"))!.ApiKey.Value);
        Assert.Single(catalog.Contents.Providers);
    }
}
