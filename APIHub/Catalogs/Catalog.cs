using ApiHub.Models;
using ErrorOr;

namespace ApiHub.Catalogs;

/// <summary>提供商与模型的目录。变更非线程安全，由调用方同步；读取无需加锁。</summary>
public sealed class Catalog {
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

    /// <summary>目录中的提供商，反映当前状态。顺序不保证。</summary>
    public IReadOnlyCollection<ProviderDefinition> Providers => _providers.Values;

    /// <summary>目录中的模型及其归属，反映当前状态。顺序不保证。</summary>
    public IReadOnlyCollection<ModelDefinition> Models => _models.Values;

    /// <summary>导出当前状态供持久化，与构造互逆。导出时复制，已导出的快照不受后续变更影响。</summary>
    public CatalogContents Contents => CatalogContents.FromValid([.. _providers.Values], [.. _models.Values]);

    /// <summary>按模型名查找模型，未命中返回 null。模型名按自身大小写精确匹配。</summary>
    public ModelDefinition? FindModel(ModelName modelName) =>
        _models.TryGetValue(modelName, out ModelDefinition? model) ? model : null;

    /// <summary>按提供商名查找提供商，未命中返回 null。提供商名不区分大小写。</summary>
    public ProviderDefinition? FindProvider(ProviderName providerName) =>
        _providers.TryGetValue(providerName, out ProviderDefinition? provider) ? provider : null;

    /// <summary>按模型名查找接入所需信息，未命中返回 null。模型名按自身大小写精确匹配。</summary>
    public ModelConnection? FindModelConnection(ModelName modelName) {
        if (!_models.TryGetValue(modelName, out ModelDefinition? model)) {
            return null;
        }

        return new ModelConnection(model, _providers[model.ProviderName]);
    }

    /// <summary>新增提供商。提供商名已存在时拒绝。</summary>
    public ErrorOr<Success> AddProvider(ProviderDefinition provider) {
        if (_providers.ContainsKey(provider.ProviderName)) {
            return CatalogContents.DuplicateProviderError(provider.ProviderName);
        }

        _providers.Add(provider.ProviderName, provider);

        return Result.Success;
    }

    /// <summary>替换提供商。提供商名不存在时拒绝。</summary>
    public ErrorOr<Success> ReplaceProvider(ProviderDefinition provider) {
        if (!_providers.ContainsKey(provider.ProviderName)) {
            return ProviderNotFoundError(provider.ProviderName);
        }

        _providers[provider.ProviderName] = provider;

        return Result.Success;
    }

    /// <summary>删除提供商。仍被模型引用时拒绝，避免留下悬空引用。</summary>
    public ErrorOr<Success> RemoveProvider(ProviderName providerName) {
        if (!_providers.ContainsKey(providerName)) {
            return ProviderNotFoundError(providerName);
        }

        List<ModelDefinition> referenced = [.. _models.Values.Where(model => model.ProviderName == providerName)];
        if (referenced.Count > 0) {
            string models = string.Join("、", referenced.Select(model => model.ModelName.Value));

            return Error.Conflict("Catalog.ProviderInUse", $"提供商 {providerName.Value} 仍被模型 {models} 引用，先删除或改指向其他提供商。");
        }

        _providers.Remove(providerName);

        return Result.Success;
    }

    /// <summary>新增模型。模型名已存在或提供商不在目录中时拒绝。</summary>
    public ErrorOr<Success> AddModel(ModelDefinition model) {
        if (_models.ContainsKey(model.ModelName)) {
            return CatalogContents.DuplicateModelError(model.ModelName);
        }

        if (!_providers.ContainsKey(model.ProviderName)) {
            return CatalogContents.DanglingReferenceError(model.ProviderName);
        }

        _models.Add(model.ModelName, model);

        return Result.Success;
    }

    /// <summary>替换模型。模型名或新指向的提供商不存在时拒绝。</summary>
    public ErrorOr<Success> ReplaceModel(ModelDefinition model) {
        if (!_models.ContainsKey(model.ModelName)) {
            return ModelNotFoundError(model.ModelName);
        }

        if (!_providers.ContainsKey(model.ProviderName)) {
            return CatalogContents.DanglingReferenceError(model.ProviderName);
        }

        _models[model.ModelName] = model;

        return Result.Success;
    }

    /// <summary>删除模型。模型名不存在时拒绝。</summary>
    public ErrorOr<Success> RemoveModel(ModelName modelName) {
        if (!_models.Remove(modelName)) {
            return ModelNotFoundError(modelName);
        }

        return Result.Success;
    }

    private static Error ProviderNotFoundError(ProviderName providerName) =>
        Error.NotFound("Catalog.ProviderNotFound", $"提供商 {providerName.Value} 不存在。");

    private static Error ModelNotFoundError(ModelName modelName) =>
        Error.NotFound("Catalog.ModelNotFound", $"模型 {modelName.Value} 不存在。");
}
