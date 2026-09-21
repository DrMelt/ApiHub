namespace ApiHub.Models;

/// <summary>模型定义：模型名及其提供商归属。</summary>
public sealed record ModelDefinition {
    /// <summary>模型名，直接作为请求的模型参数。</summary>
    public required ModelName ModelName {
        get; init;
    }

    /// <summary>提供该模型的提供商名。</summary>
    public required ProviderName ProviderName {
        get; init;
    }

    /// <summary>由字段构成定义。字段自身的合法性由各类型对象的解析口保证。</summary>
    public static ModelDefinition Create(ModelName modelName, ProviderName providerName) =>
        new() {
            ModelName = modelName,
            ProviderName = providerName
        };
}
