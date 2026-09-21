using System.Collections.Immutable;
using ErrorOr;

namespace ApiHub.Models;

/// <summary>目录内容：提供商与模型集合。集合内自一致：名唯一，模型指向的提供商在集合中。成员不可变，交出后不被目录的后续变更影响。</summary>
public sealed class CatalogContents {
    private CatalogContents(ImmutableArray<ProviderDefinition> providers, ImmutableArray<ModelDefinition> models) {
        Providers = providers;
        Models = models;
    }

    /// <summary>目录中的提供商。</summary>
    public ImmutableArray<ProviderDefinition> Providers {
        get;
    }

    /// <summary>目录中的模型及其归属。</summary>
    public ImmutableArray<ModelDefinition> Models {
        get;
    }

    /// <summary>由提供商与模型集合构成内容，一次给出全部错误：集合为未初始化值、提供商重复、模型重复、模型引用不存在的提供商。</summary>
    public static ErrorOr<CatalogContents> Create(
        ImmutableArray<ProviderDefinition> providers,
        ImmutableArray<ModelDefinition> models) {
        if (providers.IsDefault || models.IsDefault) {
            return Error.Validation("CatalogContents.Invalid", "提供商与模型集合不能是未初始化值。");
        }

        List<Error> errors = [];
        HashSet<ProviderName> providerNames = [];
        HashSet<ModelName> modelNames = [];

        foreach (ProviderDefinition provider in providers) {
            if (!providerNames.Add(provider.ProviderName)) {
                errors.Add(DuplicateProviderError(provider.ProviderName));
            }
        }

        foreach (ModelDefinition model in models) {
            if (!modelNames.Add(model.ModelName)) {
                errors.Add(DuplicateModelError(model.ModelName));
            }

            if (!providerNames.Contains(model.ProviderName)) {
                errors.Add(DanglingReferenceError(model.ProviderName));
            }
        }

        if (errors.Count > 0) {
            return errors;
        }

        return new CatalogContents(providers, models);
    }

    /// <summary>由已保证集合内自一致的集合构成内容，不做校验。供程序集内可信路径使用。</summary>
    internal static CatalogContents FromValid(
        ImmutableArray<ProviderDefinition> providers,
        ImmutableArray<ModelDefinition> models) =>
        new(providers, models);

    /// <summary>提供商名重复的冲突错误。</summary>
    internal static Error DuplicateProviderError(ProviderName providerName) =>
        Error.Conflict("Catalog.ProviderAlreadyExists", $"提供商 {providerName.Value} 已存在。");

    /// <summary>模型名重复的冲突错误。</summary>
    internal static Error DuplicateModelError(ModelName modelName) =>
        Error.Conflict("Catalog.ModelAlreadyExists", $"模型 {modelName.Value} 已存在。");

    /// <summary>模型引用不存在提供商的未找到错误。</summary>
    internal static Error DanglingReferenceError(ProviderName providerName) =>
        Error.NotFound("Catalog.ProviderNotFound", $"目录中没有提供商 {providerName.Value}。");
}
