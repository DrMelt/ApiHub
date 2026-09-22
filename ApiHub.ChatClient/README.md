# ApiHub.ChatClient

由提供商定义或模型连接信息建立 OpenAI 兼容客户端。数据对象与目录契约见 [ApiHub.Shared](https://github.com/DrMelt/ApiHub/tree/main/ApiHub.Shared)。

## 用法

```csharp
using ApiHub.ChatClient;
using ApiHub.Shared.Models;
using Microsoft.Extensions.AI;

IChatClient client = connection.CreateChatClient();
```

`connection` 由目录的 `FindModelConnection` 取得，含模型名、提供商名、端点与凭据。也可由提供商定义与模型名建立：`provider.CreateChatClient(modelName)`。返回直连模型的裸 `IChatClient`，中间件由调用方自行叠加。

本包只依赖 `ApiHub.Shared`，不需要目录实现包 `ApiHub`。

## 许可

Apache-2.0