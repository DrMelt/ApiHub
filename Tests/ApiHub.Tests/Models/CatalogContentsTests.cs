using ApiHub.Models;
using ApiHub.Tests.Shared;
using ErrorOr;

namespace ApiHub.Tests.Models;

public class CatalogContentsTests {
    [Fact]
    public void 集合内自一致的内容被接受() {
        CatalogContents contents = TestData.Contents(
            [TestData.Provider("dashscope")],
            [TestData.Model("qwen-plus")]);

        Assert.Single(contents.Providers);
        Assert.Single(contents.Models);
    }

    [Fact]
    public void 空集合被接受() {
        CatalogContents contents = TestData.Ok(CatalogContents.Create([], []));

        Assert.Empty(contents.Providers);
        Assert.Empty(contents.Models);
    }

    [Fact]
    public void 提供商名重复被拒且不区分大小写() {
        ErrorOr<CatalogContents> result = CatalogContents.Create(
            [TestData.Provider("dashscope"), TestData.Provider("DashScope")],
            []);

        Assert.True(result.IsError);
        Assert.Equal("Catalog.ProviderAlreadyExists", result.FirstError.Code);
    }

    [Fact]
    public void 模型名重复被拒() {
        ErrorOr<CatalogContents> result = CatalogContents.Create(
            [TestData.Provider("dashscope")],
            [TestData.Model("qwen-plus"), TestData.Model("qwen-plus")]);

        Assert.True(result.IsError);
        Assert.Equal("Catalog.ModelAlreadyExists", result.FirstError.Code);
    }

    [Fact]
    public void 模型名大小写不同不视为重复() {
        ErrorOr<CatalogContents> result = CatalogContents.Create(
            [TestData.Provider("dashscope")],
            [TestData.Model("qwen-plus"), TestData.Model("Qwen-Plus")]);

        Assert.False(result.IsError);
    }

    [Fact]
    public void 模型引用不存在的提供商被拒() {
        ErrorOr<CatalogContents> result = CatalogContents.Create(
            [TestData.Provider("dashscope")],
            [TestData.Model("qwen-plus", "missing")]);

        Assert.True(result.IsError);
        Assert.Equal("Catalog.ProviderNotFound", result.FirstError.Code);
    }

    [Fact]
    public void 错误一次报全() {
        ErrorOr<CatalogContents> result = CatalogContents.Create(
            [TestData.Provider("dashscope"), TestData.Provider("DashScope")],
            [TestData.Model("qwen-plus"), TestData.Model("qwen-plus"), TestData.Model("gpt-4o", "missing")]);

        Assert.True(result.IsError);
        Assert.Equal(3, result.Errors.Count);
        Assert.Contains(result.Errors, error => error.Code == "Catalog.ProviderAlreadyExists");
        Assert.Contains(result.Errors, error => error.Code == "Catalog.ModelAlreadyExists");
        Assert.Contains(result.Errors, error => error.Code == "Catalog.ProviderNotFound");
    }

    [Fact]
    public void 未初始化的集合被拒() {
        ErrorOr<CatalogContents> result = CatalogContents.Create(default, []);

        Assert.True(result.IsError);
        Assert.Equal("CatalogContents.Invalid", result.FirstError.Code);
    }
}
