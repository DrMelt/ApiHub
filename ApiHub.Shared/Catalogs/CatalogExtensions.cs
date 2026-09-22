using ApiHub.Shared.Models;

namespace ApiHub.Shared.Catalogs;

/// <summary>由读取契约派生的查询，不需要实现方逐个提供。</summary>
public static class CatalogExtensions {
    /// <summary>按模型名查找接入所需信息，模型或提供商未命中时返回 null。模型名按自身大小写精确匹配。</summary>
    public static ModelConnection? FindModelConnection(this IReadOnlyCatalog catalog, ModelName modelName) {
        ModelDefinition? model = catalog.FindModel(modelName);
        if (model is null) {
            return null;
        }

        ProviderDefinition? provider = catalog.FindProvider(model.ProviderName);

        return provider is null ? null : ModelConnection.Create(model, provider);
    }
}
