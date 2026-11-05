using ApiHub.Shared.Models;
using Microsoft.Extensions.AI;

namespace ApiHub.ChatClient;

/// <summary>转发到内层客户端，并可经 GetService 取得模型上下文。</summary>
internal sealed class ContextAwareChatClient(IChatClient innerClient, ModelContextLength contextLength) : DelegatingChatClient(innerClient)
{
    private readonly ModelContextLength _contextLength = contextLength;

    public override object? GetService(Type serviceType, object? serviceKey = null) =>
        serviceType == typeof(ModelContextLength) && serviceKey is null
            ? _contextLength
            : base.GetService(serviceType, serviceKey);
}
