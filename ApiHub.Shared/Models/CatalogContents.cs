using ApiHub.Shared.Catalogs;
using System.Collections.Immutable;
using ErrorOr;

namespace ApiHub.Shared.Models;

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
                errors.Add(CatalogErrors.DuplicateProvider(provider.ProviderName));
            }
        }

        foreach (ModelDefinition model in models) {
            if (!modelNames.Add(model.ModelName)) {
                errors.Add(CatalogErrors.DuplicateModel(model.ModelName));
            }

            if (!providerNames.Contains(model.ProviderName)) {
                errors.Add(CatalogErrors.ProviderNotFound(model.ProviderName));
            }
        }

        if (errors.Count > 0) {
            return errors;
        }

        return new CatalogContents(providers, models);
    }
}
