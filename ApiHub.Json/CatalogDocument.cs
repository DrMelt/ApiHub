namespace ApiHub.Json;

/// <summary>目录内容的持久化结构与字段原文。</summary>
internal sealed record CatalogDocument {
    /// <summary>提供商列表，缺失或为 null 时视为空集合。</summary>
    public List<ProviderDocument>? Providers {
        get; init;
    }

    /// <summary>模型列表，缺失或为 null 时视为空集合。</summary>
    public List<ModelDocument>? Models {
        get; init;
    }
}

/// <summary>提供商的持久化字段原文。</summary>
internal sealed record ProviderDocument {
    /// <summary>提供商名原文。</summary>
    public string? ProviderName {
        get; init;
    }

    /// <summary>端点原文。</summary>
    public string? BaseAddress {
        get; init;
    }

    /// <summary>凭据原文。</summary>
    public string? ApiKey {
        get; init;
    }
}

/// <summary>模型的持久化字段原文。</summary>
internal sealed record ModelDocument {
    /// <summary>模型名原文。</summary>
    public string? ModelName {
        get; init;
    }

    /// <summary>提供商名原文。</summary>
    public string? ProviderName {
        get; init;
    }
}
