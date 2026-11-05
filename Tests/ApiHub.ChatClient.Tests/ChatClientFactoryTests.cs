using ApiHub.Catalogs;
using ApiHub.ChatClient;
using ApiHub.Shared.Catalogs;
using ApiHub.Shared.Models;
using ApiHub.Tests.Fixture;
using Microsoft.Extensions.AI;

namespace ApiHub.ChatClient.Tests;

public class ChatClientFactoryTests {
    private const string BaseAddress = "http://127.0.0.1:11434/v1";

    [Fact]
    public void 建立客户端时使用提供商模型名与端点() {
        ProviderDefinition provider = TestData.Provider("local", BaseAddress, "ollama");

        IChatClient client = provider.CreateChatClient(TestData.ProviderModelName("qwen3"));

        ChatClientMetadata metadata = client.GetService<ChatClientMetadata>()!;

        Assert.Equal("qwen3", metadata.DefaultModelId);
        Assert.Equal(BaseAddress, metadata.ProviderUri!.ToString());
    }

    [Fact]
    public void 由目录取得的连接信息使用提供商模型名建立客户端() {
        Catalog catalog = new(TestData.Contents(
            [TestData.Provider("local", BaseAddress, "ollama")],
            [TestData.Model("qwen3-local", "local", "qwen3")]));

        IChatClient client = catalog.FindModelConnection(TestData.ModelName("qwen3-local"))!.CreateChatClient();

        ChatClientMetadata metadata = client.GetService<ChatClientMetadata>()!;

        Assert.Equal("qwen3", metadata.DefaultModelId);
    }

    [Fact]
    public void 由连接取得的客户端可经GetService取回模型上下文() {
        Catalog catalog = new(TestData.Contents(
            [TestData.Provider("local", BaseAddress, "ollama")],
            [TestData.Model("qwen3", "local", contextLength: 8192)]));

        IChatClient client = catalog.FindModelConnection(TestData.ModelName("qwen3"))!.CreateChatClient();

        ModelContextLength? contextLength = client.GetService<ModelContextLength>();

        Assert.NotNull(contextLength);
        Assert.Equal(8192, contextLength.Value);
    }
}
