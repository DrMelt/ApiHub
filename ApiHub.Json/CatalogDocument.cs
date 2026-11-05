namespace ApiHub.Json;

/// <summary>目录内容的持久化结构与字段原文。</summary>
internal sealed record CatalogDocument
{
    /// <summary>提供商列表，缺失或为 null 时视为空集合。</summary>
    public List<ProviderDocument>? Providers
    {
        get; init;
    }

    /// <summary>模型列表，缺失或为 null 时视为空集合。</summary>
    public List<ModelDocument>? Models
    {
        get; init;
    }
}

/// <summary>提供商的持久化字段原文。</summary>
internal sealed record ProviderDocument
{
    /// <summary>提供商名原文。</summary>
    public string? ProviderName
    {
        get; init;
    }

    /// <summary>端点原文。</summary>
    public string? BaseAddress
    {
        get; init;
    }

    /// <summary>凭据原文。</summary>
    public string? ApiKey
    {
        get; init;
    }
}

/// <summary>模型的持久化字段原文。</summary>
internal sealed record ModelDocument
{
    /// <summary>本地模型名原文，在目录内唯一。</summary>
    public string? ModelName
    {
        get; init;
    }

    /// <summary>提供商名原文。</summary>
    public string? ProviderName
    {
        get; init;
    }

    /// <summary>提供商使用的模型名原文，缺失时取本地模型名。</summary>
    public string? ProviderModelName
    {
        get; init;
    }

    /// <summary>模型上下文窗口长度原文，缺失时视为非法。</summary>
    public int? ContextLength
    {
        get; init;
    }
}
