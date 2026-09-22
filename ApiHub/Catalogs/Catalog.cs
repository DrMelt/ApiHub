using ApiHub.Shared.Catalogs;
using ApiHub.Shared.Models;
using ErrorOr;

namespace ApiHub.Catalogs;

/// <summary>提供商与模型的内存目录。变更非线程安全，由调用方同步；读取无需加锁。</summary>
public sealed class Catalog : ICatalog {
    private readonly Dictionary<ProviderName, ProviderDefinition> _providers = [];
    private readonly Dictionary<ModelName, ModelDefinition> _models = [];

    /// <summary>空目录。</summary>
    public Catalog() {
    }

    /// <summary>由已校验的内容装配目录。</summary>
    public Catalog(CatalogContents contents) {
        foreach (ProviderDefinition provider in contents.Providers) {
            _providers.Add(provider.ProviderName, provider);
        }

        foreach (ModelDefinition model in contents.Models) {
            _models.Add(model.ModelName, model);
        }
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<ProviderDefinition> Providers => _providers.Values;

    /// <inheritdoc/>
    public IReadOnlyCollection<ModelDefinition> Models => _models.Values;

    /// <inheritdoc/>
    /// <remarks>目录自身的状态保证集合内自一致，导出不会失败。</remarks>
    public CatalogContents Contents => CatalogContents.Create([.. _providers.Values], [.. _models.Values]).Value;

    /// <inheritdoc/>
    public ModelDefinition? FindModel(ModelName modelName) =>
        _models.TryGetValue(modelName, out ModelDefinition? model) ? model : null;

    /// <inheritdoc/>
    public ProviderDefinition? FindProvider(ProviderName providerName) =>
        _providers.TryGetValue(providerName, out ProviderDefinition? provider) ? provider : null;

    /// <inheritdoc/>
    public ErrorOr<Success> AddProvider(ProviderDefinition provider) {
        if (_providers.ContainsKey(provider.ProviderName)) {
            return CatalogErrors.DuplicateProvider(provider.ProviderName);
        }

        _providers.Add(provider.ProviderName, provider);

        return Result.Success;
    }

    /// <inheritdoc/>
    public ErrorOr<Success> ReplaceProvider(ProviderDefinition provider) {
        if (!_providers.ContainsKey(provider.ProviderName)) {
            return CatalogErrors.ProviderNotFound(provider.ProviderName);
        }

        _providers[provider.ProviderName] = provider;

        return Result.Success;
    }

    /// <inheritdoc/>
    public ErrorOr<Success> RemoveProvider(ProviderName providerName) {
        if (!_providers.ContainsKey(providerName)) {
            return CatalogErrors.ProviderNotFound(providerName);
        }

        List<ModelDefinition> referenced = [.. _models.Values.Where(model => model.ProviderName == providerName)];
        if (referenced.Count > 0) {
            string models = string.Join("、", referenced.Select(model => model.ModelName.Value));

            return Error.Conflict("Catalog.ProviderInUse", $"提供商 {providerName.Value} 仍被模型 {models} 引用，先删除或改指向其他提供商。");
        }

        _providers.Remove(providerName);

        return Result.Success;
    }

    /// <inheritdoc/>
    public ErrorOr<Success> AddModel(ModelDefinition model) {
        if (_models.ContainsKey(model.ModelName)) {
            return CatalogErrors.DuplicateModel(model.ModelName);
        }

        if (!_providers.ContainsKey(model.ProviderName)) {
            return CatalogErrors.ProviderNotFound(model.ProviderName);
        }

        _models.Add(model.ModelName, model);

        return Result.Success;
    }

    /// <inheritdoc/>
    public ErrorOr<Success> ReplaceModel(ModelDefinition model) {
        if (!_models.ContainsKey(model.ModelName)) {
            return CatalogErrors.ModelNotFound(model.ModelName);
        }

        if (!_providers.ContainsKey(model.ProviderName)) {
            return CatalogErrors.ProviderNotFound(model.ProviderName);
        }

        _models[model.ModelName] = model;

        return Result.Success;
    }

    /// <inheritdoc/>
    public ErrorOr<Success> RemoveModel(ModelName modelName) {
        if (!_models.Remove(modelName)) {
            return CatalogErrors.ModelNotFound(modelName);
        }

        return Result.Success;
    }
}
