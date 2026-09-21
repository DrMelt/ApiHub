# ApiHub

LLM 提供商与模型目录的数据模型与校验。字段与定义构造、目录改动通过 [ErrorOr](https://www.nuget.org/packages/ErrorOr) 返回错误，不抛异常。

## 用法

```csharp
using ApiHub.Catalogs;
using ApiHub.Models;
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

## 许可

Apache-2.0
