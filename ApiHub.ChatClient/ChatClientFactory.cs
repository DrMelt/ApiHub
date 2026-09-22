using ApiHub.Shared.Models;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;

namespace ApiHub.ChatClient;

/// <summary>ChatClient 的静态工厂。</summary>
public static class ChatClientFactory {
    /// <summary>由提供商定义与模型名建立直连模型的裸 IChatClient，中间件由调用方自行叠加。</summary>
    public static IChatClient CreateChatClient(this ProviderDefinition provider, ModelName modelName) =>
        CreateChatClient(modelName, provider.BaseAddress, provider.ApiKey);

    /// <summary>由模型连接信息建立直连模型的裸 IChatClient，中间件由调用方自行叠加。</summary>
    public static IChatClient CreateChatClient(this ModelConnection connection) =>
        CreateChatClient(connection.ModelName, connection.BaseAddress, connection.ApiKey);

    private static IChatClient CreateChatClient(ModelName modelName, ProviderEndpoint baseAddress, ApiKey apiKey) {
        OpenAIClientOptions options = new() {
            Endpoint = baseAddress.Address
        };
        OpenAIClient client = new(new ApiKeyCredential(apiKey.Value), options);

        return client.GetChatClient(modelName.Value).AsIChatClient();
    }
}
