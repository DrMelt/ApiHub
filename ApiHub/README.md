# ApiHub

LLM 提供商与模型目录的内存实现。数据对象与契约见 [ApiHub.Shared](https://github.com/DrMelt/ApiHub/tree/main/ApiHub.Shared)。

`Catalog` 以两份字典索引提供商与模型，变更时维护名唯一与引用完整，通过 [ErrorOr](https://www.nuget.org/packages/ErrorOr) 返回错误，不抛异常。

## 用法

```csharp
using ApiHub.Catalogs;
using ApiHub.Shared.Catalogs;
using ApiHub.Shared.Models;
using ErrorOr;

Catalog catalog = new();

ErrorOr<Success> added = ProviderName.Create("dashscope")
    .Then(providerName => ApiKey.Create("sk-...")
        .Then(key => ProviderEndpoint.Create("https://dashscope.aliyuncs.com/compatible-mode/v1")
            .Then(endpoint => catalog.AddProvider(ProviderDefinition.Create(providerName, endpoint, key)))));
ErrorOr<Success> registered = ProviderName.Create("dashscope")
    .Then(providerName => ModelName.Create("qwen-plus")
        .Then(modelName => catalog.AddModel(ModelDefinition.Create(modelName, providerName))));

ModelDefinition? model = catalog.FindModel(ModelName.Create("qwen-plus").Value); // Value 只在 IsError 为 false 时取用
ProviderDefinition? provider = model is null ? null : catalog.FindProvider(model.ProviderName);
ModelConnection? connection = catalog.FindModelConnection(ModelName.Create("qwen-plus").Value);
```

`FindModelConnection` 是 `IReadOnlyCatalog` 的扩展方法，来自 `ApiHub.Shared.Catalogs`。需要隐藏实现时以契约接收：`ICatalog catalog = new Catalog();`。

## 许可

Apache-2.0
