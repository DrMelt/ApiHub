namespace ApiHub.Shared.Models;

/// <summary>模型连接信息：本地模型名、提供商模型名与接入所需的提供商名、端点、凭据及模型上下文。通常由目录按本地模型名解析得到。</summary>
public sealed record ModelConnection
{
    private ModelConnection(ModelDefinition model, ProviderDefinition provider)
    {
        ModelName = model.ModelName;
        ProviderName = provider.ProviderName;
        ProviderModelName = model.ProviderModelName;
        BaseAddress = provider.BaseAddress;
        ApiKey = provider.ApiKey;
        ContextLength = model.ContextLength;
    }

    /// <summary>目录内的本地模型名，用作选择与流程引用。</summary>
    public ModelName ModelName
    {
        get;
    }

    /// <summary>提供该模型的提供商名。</summary>
    public ProviderName ProviderName
    {
        get;
    }

    /// <summary>请求提供商时使用的模型名。</summary>
    public ProviderModelName ProviderModelName
    {
        get;
    }

    /// <summary>OpenAI 兼容端点。</summary>
    public ProviderEndpoint BaseAddress
    {
        get;
    }

    /// <summary>接入凭据。</summary>
    public ApiKey ApiKey
    {
        get;
    }

    /// <summary>模型上下文窗口长度。</summary>
    public ModelContextLength ContextLength
    {
        get;
    }

    /// <summary>由模型定义与提供商定义构成连接信息。不校验两者的归属关系，归属由目录保证。</summary>
    public static ModelConnection Create(ModelDefinition model, ProviderDefinition provider) => new(model, provider);
}
