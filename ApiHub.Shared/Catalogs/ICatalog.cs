using ApiHub.Shared.Models;
using ErrorOr;

namespace ApiHub.Shared.Catalogs;

/// <summary>目录的读取与变更契约。变更非线程安全，由调用方同步；读取无需加锁。</summary>
public interface ICatalog : IReadOnlyCatalog {
    /// <summary>新增提供商。提供商名已存在时拒绝。</summary>
    ErrorOr<Success> AddProvider(ProviderDefinition provider);

    /// <summary>替换提供商。提供商名不存在时拒绝。</summary>
    ErrorOr<Success> ReplaceProvider(ProviderDefinition provider);

    /// <summary>删除提供商。仍被模型引用时拒绝，避免留下悬空引用。</summary>
    ErrorOr<Success> RemoveProvider(ProviderName providerName);

    /// <summary>新增模型。模型名已存在或提供商不在目录中时拒绝。</summary>
    ErrorOr<Success> AddModel(ModelDefinition model);

    /// <summary>替换模型。模型名或新指向的提供商不存在时拒绝。</summary>
    ErrorOr<Success> ReplaceModel(ModelDefinition model);

    /// <summary>删除模型。模型名不存在时拒绝。</summary>
    ErrorOr<Success> RemoveModel(ModelName modelName);
}
