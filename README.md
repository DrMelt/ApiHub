# APIHub

LLM 提供商与模型目录相关的 .NET 包。

## 包

- [`APIHub`](https://github.com/DrMelt/APIHub/tree/main/APIHub) 提供商与模型目录的数据模型与校验。
- [`APIHub.ChatClient`](https://github.com/DrMelt/APIHub/tree/main/APIHub.ChatClient) 由提供商定义建立 OpenAI 兼容客户端。
- [`APIHub.Json`](https://github.com/DrMelt/APIHub/tree/main/APIHub.Json) 目录内容与 JSON 文本的互转。

## 开发

```shell
dotnet build APIHub.slnx
dotnet test APIHub.slnx
dotnet pack APIHub.slnx -c Release -o artifacts
```

## 许可

Apache-2.0
