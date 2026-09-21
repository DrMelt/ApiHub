namespace ApiHub.Models;

/// <summary>模型连接信息：模型名与接入所需的提供商名、端点、凭据。由目录按模型名解析得到。</summary>
public sealed record ModelConnection {
    /// <summary>由模型定义与其归属的提供商定义构成连接信息。</summary>
    internal ModelConnection(ModelDefinition model, ProviderDefinition provider) {
        ModelName = model.ModelName;
        ProviderName = provider.ProviderName;
        BaseAddress = provider.BaseAddress;
        ApiKey = provider.ApiKey;
    }

    /// <summary>模型名，直接作为请求的模型参数。</summary>
    public ModelName ModelName {
        get;
    }

    /// <summary>提供该模型的提供商名。</summary>
    public ProviderName ProviderName {
        get;
    }

    /// <summary>OpenAI 兼容端点。</summary>
    public ProviderEndpoint BaseAddress {
        get;
    }

    /// <summary>接入凭据。</summary>
    public ApiKey ApiKey {
        get;
    }
}
