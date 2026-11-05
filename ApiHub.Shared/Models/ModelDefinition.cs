namespace ApiHub.Shared.Models;

/// <summary>模型定义：本地模型名、提供商归属、请求提供商时使用的模型名与可选的上下文窗口长度。</summary>
public sealed record ModelDefinition
{
    /// <summary>目录内的本地模型名，用作选择与流程引用，在目录内唯一。</summary>
    public required ModelName ModelName
    {
        get; init;
    }

    /// <summary>提供该模型的提供商名。</summary>
    public required ProviderName ProviderName
    {
        get; init;
    }

    /// <summary>请求提供商时使用的模型名。</summary>
    public required ProviderModelName ProviderModelName
    {
        get; init;
    }

    /// <summary>模型上下文窗口长度，null 表示未知。</summary>
    public required ModelContextLength? ContextLength
    {
        get; init;
    }

    /// <summary>由字段构成定义。上下文长度可缺省，缺省表示未知。字段自身的合法性由各类型对象的解析口保证。</summary>
    public static ModelDefinition Create(
            ModelName modelName,
            ProviderName providerName,
            ProviderModelName providerModelName,
            ModelContextLength? contextLength = null) =>
            new()
            {
                ModelName = modelName,
                ProviderName = providerName,
                ProviderModelName = providerModelName,
                ContextLength = contextLength
            };
}
