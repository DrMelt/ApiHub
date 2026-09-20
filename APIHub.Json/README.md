# APIHub.Json

APIHub 的扩展包：把目录内容与 JSON 文本互转，用于落盘与加载。提供商与模型的数据模型见 [APIHub](https://github.com/DrMelt/APIHub)。

## 用法

```csharp
using ApiHub.Catalogs;
using ApiHub.Json;

string json = catalog.Contents.ToJson();
CatalogContents contents = CatalogJson.Parse(json).Value; // Value 只在 IsError 为 false 时取用
Catalog rebuilt = new(contents);
```

`catalog.Contents` 导出快照，`CatalogJson.Parse` 读回内容，`Catalog` 构造函数由内容装配目录。字段校验沿用 [APIHub](https://github.com/DrMelt/APIHub) 的解析口，字段全部合法后再校验集合自一致性，不合法时以 [ErrorOr](https://www.nuget.org/packages/ErrorOr) 报出错误，不抛异常。

## 格式

```json
{
  "providers": [
    { "providerName": "dashscope", "baseAddress": "https://dashscope.aliyuncs.com/compatible-mode/v1", "apiKey": "sk-..." }
  ],
  "models": [
    { "modelName": "qwen-plus", "providerName": "dashscope" }
  ]
}
```

读取时键名大小写不敏感，集合缺失或为 null 视为空集合，列表项或字段为 null 视为非法，未知属性忽略，因此旧版本可读新增字段的文件。导出时提供商与模型按名序数排序，端点按 `Uri` 规范化，文本形式可能与输入不同。

凭据以明文写入，落盘文件的访问权限由调用方控制。

## 许可

Apache-2.0