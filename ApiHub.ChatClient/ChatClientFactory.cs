using ApiHub.Shared.Models;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;

namespace ApiHub.ChatClient;

/// <summary>ChatClient 的静态工厂。</summary>
public static class ChatClientFactory {
    /// <summary>由提供商定义与提供商模型名建立直连模型的裸 IChatClient，中间件由调用方自行叠加。</summary>
    public static IChatClient CreateChatClient(this ProviderDefinition provider, ProviderModelName providerModelName) =>
        CreateChatClient(providerModelName, provider.BaseAddress, provider.ApiKey);

/// <summary>由模型连接信息建立直连模型的裸 IChatClient，中间件由调用方自行叠加。使用提供商模型名发起请求，模型上下文随客户端提供，经 GetService 取得。</summary>
    public static IChatClient CreateChatClient(this ModelConnection connection) {
        IChatClient client = CreateChatClient(connection.ProviderModelName, connection.BaseAddress, connection.ApiKey);

        return new ContextAwareChatClient(client, connection.ContextLength);
    }

    private static IChatClient CreateChatClient(
        ProviderModelName providerModelName,
        ProviderEndpoint baseAddress,
        ApiKey apiKey) {
        OpenAIClientOptions options = new() {
            Endpoint = baseAddress.Address
        };
        OpenAIClient client = new(new ApiKeyCredential(apiKey.Value), options);

        return client.GetChatClient(providerModelName.Value).AsIChatClient();
    }
}
