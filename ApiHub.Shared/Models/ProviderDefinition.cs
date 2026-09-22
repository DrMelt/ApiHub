namespace ApiHub.Shared.Models;

/// <summary>一个提供商：OpenAI 兼容端点与接入凭据。</summary>
public sealed record ProviderDefinition {
    /// <summary>提供商名，在目录内不区分大小写地唯一。</summary>
    public required ProviderName ProviderName {
        get; init;
    }

    /// <summary>OpenAI 兼容端点。</summary>
    public required ProviderEndpoint BaseAddress {
        get; init;
    }

    /// <summary>接入凭据。</summary>
    public required ApiKey ApiKey {
        get; init;
    }

    /// <summary>由提供商名、端点与凭据构成定义。</summary>
    public static ProviderDefinition Create(ProviderName providerName, ProviderEndpoint baseAddress, ApiKey apiKey) =>
        new() {
            ProviderName = providerName,
            BaseAddress = baseAddress,
            ApiKey = apiKey
        };
}
