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
    public void 建立客户端时使用给定模型名与端点() {
        ProviderDefinition provider = TestData.Provider("local", BaseAddress, "ollama");

        IChatClient client = provider.CreateChatClient(TestData.ModelName("qwen3"));

        ChatClientMetadata metadata = client.GetService<ChatClientMetadata>()!;

        Assert.Equal("qwen3", metadata.DefaultModelId);
        Assert.Equal(BaseAddress, metadata.ProviderUri!.ToString());
    }

    [Fact]
    public void 由目录取得的连接信息建立客户端() {
        Catalog catalog = new(TestData.Contents(
            [TestData.Provider("local", BaseAddress, "ollama")],
            [TestData.Model("qwen3", "local")]));

        IChatClient client = catalog.FindModelConnection(TestData.ModelName("qwen3"))!.CreateChatClient();

        ChatClientMetadata metadata = client.GetService<ChatClientMetadata>()!;

        Assert.Equal("qwen3", metadata.DefaultModelId);
    }
}
