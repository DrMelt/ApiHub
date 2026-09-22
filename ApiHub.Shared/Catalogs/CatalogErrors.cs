using ApiHub.Shared.Models;
using ErrorOr;

namespace ApiHub.Shared.Catalogs;

/// <summary>目录契约的错误。错误码属于契约，供调用方按 Code 判定。</summary>
public static class CatalogErrors {
    /// <summary>提供商名重复的冲突错误。</summary>
    public static Error DuplicateProvider(ProviderName providerName) =>
        Error.Conflict("Catalog.ProviderAlreadyExists", $"提供商 {providerName.Value} 已存在。");

    /// <summary>模型名重复的冲突错误。</summary>
    public static Error DuplicateModel(ModelName modelName) =>
        Error.Conflict("Catalog.ModelAlreadyExists", $"模型 {modelName.Value} 已存在。");

    /// <summary>提供商不在目录中的未找到错误。</summary>
    public static Error ProviderNotFound(ProviderName providerName) =>
        Error.NotFound("Catalog.ProviderNotFound", $"提供商 {providerName.Value} 不存在。");

    /// <summary>模型不在目录中的未找到错误。</summary>
    public static Error ModelNotFound(ModelName modelName) =>
        Error.NotFound("Catalog.ModelNotFound", $"模型 {modelName.Value} 不存在。");
}
