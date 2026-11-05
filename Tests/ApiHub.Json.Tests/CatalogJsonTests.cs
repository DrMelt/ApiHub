using ApiHub.Catalogs;
using ApiHub.Json;
using ApiHub.Shared.Models;
using ApiHub.Tests.Fixture;
using ErrorOr;
using System.Text.Json;

namespace ApiHub.Json.Tests;

public class CatalogJsonTests {
    private static Catalog Sample() => new(TestData.Contents(
        [TestData.Provider("dashscope", apiKey: "sk-1")],
        [TestData.Model("qwen-plus"), TestData.Model("qwen-max")]));

    private static string[] Entries(string json, string collection, string key) {
        using JsonDocument document = JsonDocument.Parse(json);

        return [.. document.RootElement.GetProperty(collection).EnumerateArray()
            .Select(item => item.GetProperty(key).GetString()!)];
    }

    private static void AssertInvalidJson(string json) {
        ErrorOr<CatalogContents> result = CatalogJson.Parse(json);

        Assert.True(result.IsError);
        Assert.Equal("CatalogJson.InvalidJson", result.FirstError.Code);
    }

    [Fact]
    public void 目录内容往返后保持查找结果() {
        Catalog rebuilt = new(TestData.Ok(CatalogJson.Parse(Sample().Contents.ToJson())));

        ModelDefinition model = rebuilt.FindModel(TestData.ModelName("qwen-plus"))!;

        Assert.Equal(2, rebuilt.Models.Count);
        Assert.Equal("dashscope", model.ProviderName.Value);
        Assert.Equal("sk-1", rebuilt.FindProvider(model.ProviderName)!.ApiKey.Value);
        Assert.Equal("https://example.com/v1", rebuilt.FindProvider(model.ProviderName)!.BaseAddress.Address.ToString());
        Assert.Equal(32768, model.ContextLength.Value);
    }

    [Fact]
    public void 原始大小写往返保留() {
        Catalog catalog = new(TestData.Contents(
            [TestData.Provider("DashScope")],
            [TestData.Model("Qwen-Plus", "DashScope")]));

        Catalog rebuilt = new(TestData.Ok(CatalogJson.Parse(catalog.Contents.ToJson())));

        Assert.Equal("DashScope", rebuilt.Providers.Single().ProviderName.Value);
        Assert.Equal("Qwen-Plus", rebuilt.Models.Single().ModelName.Value);
    }

    [Fact]
    public void 导出键名与结构固定() {
        using JsonDocument document = JsonDocument.Parse(Sample().Contents.ToJson());
        JsonElement provider = document.RootElement.GetProperty("providers")[0];

        Assert.Equal("dashscope", provider.GetProperty("providerName").GetString());
        Assert.Equal("https://example.com/v1", provider.GetProperty("baseAddress").GetString());
        Assert.Equal("sk-1", provider.GetProperty("apiKey").GetString());

        JsonElement model = document.RootElement.GetProperty("models")[0];

        Assert.Equal("qwen-max", model.GetProperty("modelName").GetString());
        Assert.Equal("dashscope", model.GetProperty("providerName").GetString());
Assert.Equal(32768, model.GetProperty("contextLength").GetInt32());
        Assert.Equal("qwen-max", model.GetProperty("providerModelName").GetString());
    }

    [Fact]
    public void 本地模型名与提供商模型名往返保留() {
        Catalog catalog = new(TestData.Contents(
            [TestData.Provider("aliyun")],
            [TestData.Model("qwen-plus-aliyun", "aliyun", "qwen-plus")]));

        Catalog rebuilt = new(TestData.Ok(CatalogJson.Parse(catalog.Contents.ToJson())));

        Assert.Equal("qwen-plus-aliyun", rebuilt.Models.Single().ModelName.Value);
        Assert.Equal("qwen-plus", rebuilt.Models.Single().ProviderModelName.Value);
    }

    [Fact]
    public void 提供商模型名缺失时取本地模型名() {
        ErrorOr<CatalogContents> result = CatalogJson.Parse("""
            {
              "providers": [
                { "providerName": "dashscope", "baseAddress": "https://example.com/v1", "apiKey": "sk" }
              ],
              "models": [
                { "modelName": "qwen-plus", "providerName": "dashscope", "contextLength": 32768 }
              ]
            }
            """);

        Assert.False(result.IsError);
        Assert.Equal("qwen-plus", result.Value.Models.Single().ProviderModelName.Value);
    }

    [Fact]
    public void 同一提供商模型名可挂在多个提供商下() {
        ErrorOr<CatalogContents> result = CatalogJson.Parse("""
            {
              "providers": [
                { "providerName": "a", "baseAddress": "https://example.com/a", "apiKey": "k1" },
                { "providerName": "b", "baseAddress": "https://example.com/b", "apiKey": "k2" }
              ],
              "models": [
                { "modelName": "qwen-plus-a", "providerName": "a", "providerModelName": "qwen-plus", "contextLength": 32768 },
                { "modelName": "qwen-plus-b", "providerName": "b", "providerModelName": "qwen-plus", "contextLength": 32768 }
              ]
            }
            """);

        Assert.False(result.IsError);
        Assert.Equal(2, result.Value.Models.Length);
    }

    [Fact]
    public void 导出项按名序数排序() {
        Catalog catalog = new(TestData.Contents(
            [TestData.Provider("zeta"), TestData.Provider("alpha")],
            [TestData.Model("qwen-plus", "zeta"), TestData.Model("qwen-max", "zeta")]));

        string json = catalog.Contents.ToJson();

        Assert.Equal(["alpha", "zeta"], Entries(json, "providers", "providerName"));
        Assert.Equal(["qwen-max", "qwen-plus"], Entries(json, "models", "modelName"));
    }

    [Fact]
    public void 同一内容的导出文本一致() {
        CatalogContents contents = Sample().Contents;

        Assert.Equal(contents.ToJson(), contents.ToJson());
    }

    [Fact]
    public void 端点按Uri规范化后导出() {
        Catalog catalog = new(TestData.Contents([TestData.Provider("p", "https://example.com")], []));

        Assert.Equal("https://example.com/", Entries(catalog.Contents.ToJson(), "providers", "baseAddress").Single());
    }

    [Fact]
    public void 空目录往返后仍为空() {
        CatalogContents contents = TestData.Ok(CatalogJson.Parse(new Catalog().Contents.ToJson()));

        Assert.Empty(contents.Providers);
        Assert.Empty(contents.Models);
    }

    [Fact]
    public void 读取时键名大小写不敏感() {
        CatalogContents contents = TestData.Ok(CatalogJson.Parse("""
            {
              "Providers": [
                { "ProviderName": "dashscope", "BaseAddress": "https://example.com/v1", "ApiKey": "sk-1" }
              ],
              "Models": [
                { "ModelName": "qwen-plus", "ProviderName": "dashscope", "ContextLength": 32768 }
              ]
            }
            """));

        Assert.Equal("sk-1", contents.Providers.Single().ApiKey.Value);
        Assert.Equal("qwen-plus", contents.Models.Single().ModelName.Value);
    }

    [Fact]
    public void 缺失集合视为空() {
        CatalogContents contents = TestData.Ok(CatalogJson.Parse("{}"));

        Assert.Empty(contents.Providers);
        Assert.Empty(contents.Models);
    }

    [Fact]
    public void 未知属性被忽略() {
        ErrorOr<CatalogContents> result = CatalogJson.Parse("""
            {
              "providers": [
                {
                  "providerName": "dashscope",
                  "baseAddress": "https://example.com/v1",
                  "apiKey": "sk-1",
                  "remark": "自定义"
                }
              ],
              "models": [],
              "version": 1
            }
            """);

        Assert.False(result.IsError);
        Assert.Equal("dashscope", result.Value.Providers.Single().ProviderName.Value);
    }

    [Fact]
    public void 无效JSON被拒() => AssertInvalidJson("{ \"providers\": [");

    [Fact]
    public void JSON为null或空被拒() {
        AssertInvalidJson("null");
        AssertInvalidJson("");
        AssertInvalidJson(null!);
    }

    [Fact]
    public void 结构不符被拒() => AssertInvalidJson("""{ "providers": 1 }""");

    [Fact]
    public void 列表项为null被拒() {
        AssertInvalidJson("""{ "providers": [null], "models": [] }""");
        AssertInvalidJson("""{ "providers": [], "models": [null] }""");
    }

    [Fact]
    public void 端点非法被拒() {
        ErrorOr<CatalogContents> result = CatalogJson.Parse("""
            {
              "providers": [
                { "providerName": "dashscope", "baseAddress": "ftp://example.com", "apiKey": "sk" }
              ],
              "models": []
            }
            """);

        Assert.True(result.IsError);
        Assert.Equal("ProviderEndpoint.Invalid", result.FirstError.Code);
    }

    [Fact]
    public void 提供商名为空被拒() {
        ErrorOr<CatalogContents> result = CatalogJson.Parse("""
            {
              "providers": [
                { "providerName": "  ", "baseAddress": "https://example.com/v1", "apiKey": "sk" }
              ],
              "models": []
            }
            """);

        Assert.True(result.IsError);
        Assert.Equal("ProviderName.Invalid", result.FirstError.Code);
    }

    [Fact]
    public void 模型名为空被拒() {
        ErrorOr<CatalogContents> result = CatalogJson.Parse("""
            {
              "providers": [],
              "models": [
                { "modelName": "", "providerName": "dashscope", "contextLength": 32768 }
              ]
            }
            """);

        Assert.True(result.IsError);
        Assert.Equal("ModelName.Invalid", result.FirstError.Code);
    }

    [Fact]
    public void 凭据为空被拒() {
        ErrorOr<CatalogContents> result = CatalogJson.Parse("""
            {
              "providers": [
                { "providerName": "dashscope", "baseAddress": "https://example.com/v1", "apiKey": "" }
              ],
              "models": []
            }
            """);

        Assert.True(result.IsError);
        Assert.Equal("ApiKey.Invalid", result.FirstError.Code);
    }

    [Fact]
    public void 缺失字段视为非法值() {
        ErrorOr<CatalogContents> result = CatalogJson.Parse("""
            {
              "providers": [
                { "baseAddress": "https://example.com/v1", "apiKey": "sk" }
              ],
              "models": []
            }
            """);

        Assert.True(result.IsError);
        Assert.Equal("ProviderName.Invalid", result.FirstError.Code);
    }

    [Fact]
    public void 字段错误一次报全() {
        ErrorOr<CatalogContents> result = CatalogJson.Parse("""
            {
              "providers": [
                { "providerName": "", "baseAddress": "https://example.com/v1", "apiKey": "sk" },
                { "providerName": "dashscope", "baseAddress": "ftp://example.com", "apiKey": "" }
              ],
              "models": [
                { "modelName": "qwen-plus", "providerName": "", "contextLength": 32768 }
              ]
            }
            """);

        Assert.True(result.IsError);
        Assert.Equal(4, result.Errors.Count);
        Assert.Contains(result.Errors, error => error.Code == "ProviderName.Invalid");
        Assert.Contains(result.Errors, error => error.Code == "ProviderEndpoint.Invalid");
        Assert.Contains(result.Errors, error => error.Code == "ApiKey.Invalid");
    }

    [Fact]
    public void 字段错误时不报集合错误() {
        ErrorOr<CatalogContents> result = CatalogJson.Parse("""
            {
              "providers": [
                { "providerName": "dashscope", "baseAddress": "ftp://example.com", "apiKey": "sk" }
              ],
              "models": [
                { "modelName": "qwen-plus", "providerName": "dashscope", "contextLength": 32768 }
              ]
            }
            """);

        Assert.True(result.IsError);
        Assert.Equal("ProviderEndpoint.Invalid", Assert.Single(result.Errors).Code);
    }

    [Fact]
    public void 凭据中的特殊字符往返保留() {
        Catalog catalog = new(TestData.Contents(
            [TestData.Provider("p", apiKey: "sk-\"quote\"\\slash\nnewline")],
            []));

        Catalog rebuilt = new(TestData.Ok(CatalogJson.Parse(catalog.Contents.ToJson())));

        Assert.Equal("sk-\"quote\"\\slash\nnewline", rebuilt.Providers.Single().ApiKey.Value);
    }

    [Fact]
    public void 提供商名重复被拒() {
        ErrorOr<CatalogContents> result = CatalogJson.Parse("""
            {
              "providers": [
                { "providerName": "dashscope", "baseAddress": "https://example.com/v1", "apiKey": "sk" },
                { "providerName": "dashscope", "baseAddress": "https://example.com/v1", "apiKey": "sk" }
              ],
              "models": []
            }
            """);

        Assert.True(result.IsError);
        Assert.Equal("Catalog.ProviderAlreadyExists", result.FirstError.Code);
    }

    [Fact]
    public void 提供商名大小写不同不视为重复() {
        ErrorOr<CatalogContents> result = CatalogJson.Parse("""
            {
              "providers": [
                { "providerName": "dashscope", "baseAddress": "https://example.com/v1", "apiKey": "sk" },
                { "providerName": "DashScope", "baseAddress": "https://example.com/v1", "apiKey": "sk" }
              ],
              "models": []
            }
            """);

        Assert.False(result.IsError);
        Assert.Equal(2, result.Value.Providers.Length);
    }

    [Fact]
    public void 模型名重复被拒() {
        ErrorOr<CatalogContents> result = CatalogJson.Parse("""
            {
              "providers": [
                { "providerName": "dashscope", "baseAddress": "https://example.com/v1", "apiKey": "sk" }
              ],
              "models": [
                { "modelName": "qwen-plus", "providerName": "dashscope", "contextLength": 32768 },
                { "modelName": "qwen-plus", "providerName": "dashscope", "contextLength": 32768 }
              ]
            }
            """);

        Assert.True(result.IsError);
        Assert.Equal("Catalog.ModelAlreadyExists", result.FirstError.Code);
    }

    [Fact]
    public void 模型引用不存在的提供商被拒() {
        ErrorOr<CatalogContents> result = CatalogJson.Parse("""
            {
              "providers": [
                { "providerName": "dashscope", "baseAddress": "https://example.com/v1", "apiKey": "sk" }
              ],
              "models": [
                { "modelName": "qwen-plus", "providerName": "missing", "contextLength": 32768 }
              ]
            }
            """);

        Assert.True(result.IsError);
        Assert.Equal("Catalog.ProviderNotFound", result.FirstError.Code);
    }

    [Fact]
    public void 上下文长度往返保留() {
        Catalog catalog = new(TestData.Contents(
            [TestData.Provider("dashscope")],
            [TestData.Model("qwen-plus", contextLength: 8192)]));

        Catalog rebuilt = new(TestData.Ok(CatalogJson.Parse(catalog.Contents.ToJson())));

        Assert.Equal(8192, rebuilt.FindModel(TestData.ModelName("qwen-plus"))!.ContextLength.Value);
    }

    [Fact]
    public void 模型上下文缺失被拒() {
        ErrorOr<CatalogContents> result = CatalogJson.Parse("""
            {
              "providers": [],
              "models": [
                { "modelName": "qwen-plus", "providerName": "dashscope" }
              ]
            }
            """);

        Assert.True(result.IsError);
        Assert.Equal("ModelContextLength.Invalid", result.FirstError.Code);
    }

    [Fact]
    public void 模型上下文非正被拒() {
        ErrorOr<CatalogContents> result = CatalogJson.Parse("""
            {
              "providers": [],
              "models": [
                { "modelName": "qwen-plus", "providerName": "dashscope", "contextLength": 0 }
              ]
            }
            """);

        Assert.True(result.IsError);
        Assert.Equal("ModelContextLength.Invalid", result.FirstError.Code);
    }
}
