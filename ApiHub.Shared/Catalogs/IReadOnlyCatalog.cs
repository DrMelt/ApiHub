using ApiHub.Shared.Models;

namespace ApiHub.Shared.Catalogs;

/// <summary>目录的读取契约。实现可为内存、远端或其他存储，不约束变更。</summary>
public interface IReadOnlyCatalog {
    /// <summary>目录中的提供商，反映当前状态。顺序不保证。</summary>
    IReadOnlyCollection<ProviderDefinition> Providers {
        get;
    }

    /// <summary>目录中的模型及其归属，反映当前状态。顺序不保证。</summary>
    IReadOnlyCollection<ModelDefinition> Models {
        get;
    }

    /// <summary>导出当前状态供持久化，与 <see cref="CatalogContents.Create"/> 互逆。导出时复制，已导出的快照不受后续变更影响。</summary>
    CatalogContents Contents {
        get;
    }

    /// <summary>按模型名查找模型，未命中返回 null。模型名按自身大小写精确匹配。</summary>
    ModelDefinition? FindModel(ModelName modelName);

    /// <summary>按提供商名查找提供商，未命中返回 null。提供商名不区分大小写。</summary>
    ProviderDefinition? FindProvider(ProviderName providerName);
}
