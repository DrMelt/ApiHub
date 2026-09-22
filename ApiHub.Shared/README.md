# ApiHub.Shared

LLM 提供商与模型目录的数据对象与契约，不含存储与变更逻辑。内存目录实现见 [ApiHub](https://github.com/DrMelt/ApiHub/tree/main/ApiHub)。

字段与定义构造、目录内容校验通过 [ErrorOr](https://www.nuget.org/packages/ErrorOr) 返回错误，不抛异常。

## 契约

- `IReadOnlyCatalog` 读取：`Providers`、`Models`、`Contents`、`FindModel`、`FindProvider`。
- `ICatalog : IReadOnlyCatalog` 变更：`AddProvider`、`ReplaceProvider`、`RemoveProvider`、`AddModel`、`ReplaceModel`、`RemoveModel`。
- `CatalogExtensions.FindModelConnection` 由读取契约解析接入信息，实现方无需提供。
- `CatalogErrors` 目录错误，错误码属于契约，供调用方按 `Error.Code` 判定。

## 数据对象

`ModelName`、`ProviderName`、`ProviderEndpoint`、`ApiKey`、`ModelDefinition`、`ProviderDefinition`、`ModelConnection`、`CatalogContents`。解析口为 `Create`，失败返回 Validation 错误。

## 用法

包一层时公开签名只出现契约与数据对象，实现由组合根注入：

```csharp
using ApiHub.Shared.Catalogs;
using ApiHub.Shared.Models;
using ErrorOr;

public sealed class ModelRouter {
    private readonly IReadOnlyCatalog _catalog;

    public ModelRouter(IReadOnlyCatalog catalog) => _catalog = catalog;

    public ErrorOr<ModelConnection> Resolve(string modelName) {
        ErrorOr<ModelName> parsed = ModelName.Create(modelName);
        if (parsed.IsError) {
            return parsed.Errors;
        }

        if (_catalog.FindModelConnection(parsed.Value) is not ModelConnection connection) {
            return CatalogErrors.ModelNotFound(parsed.Value);
        }

        return connection;
    }
}
```

调用方只需引用 `ApiHub.Shared`。目录实现可用 `ApiHub` 的 `Catalog`，也可换成任意 `ICatalog` 实现或测试替身。

## 许可

Apache-2.0
