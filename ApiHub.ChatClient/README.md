# ApiHub.ChatClient

ApiHub 的扩展包：由提供商定义建立 OpenAI 兼容客户端。提供商与模型的目录数据模型见 [ApiHub](https://github.com/DrMelt/ApiHub)。

## 用法

```csharp
using ApiHub.ChatClient;
using ApiHub.Models;
using Microsoft.Extensions.AI;

IChatClient client = connection.CreateChatClient();
```

`connection` 由目录 `FindModelConnection` 取得，含模型名、提供商名、端点与凭据。也可由提供商定义与模型名建立：`provider.CreateChatClient(modelName)`。返回直连模型的裸 `IChatClient`，中间件由调用方自行叠加。

## 许可

Apache-2.0