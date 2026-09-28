using ErrorOr;

namespace ApiHub.Shared.Models;

/// <summary>提供商名，不接受空或空白，内容不限。相等与哈希按大小写敏感判定，目录据此唯一索引提供商。</summary>
public sealed record ProviderName {
    private ProviderName(string value) => Value = value;

    /// <summary>提供商名原文。</summary>
    public string Value {
        get;
    }

    /// <summary>解析提供商名，失败返回 Validation 错误。</summary>
    public static ErrorOr<ProviderName> Create(string value) {
        if (string.IsNullOrWhiteSpace(value)) {
            return Error.Validation("ProviderName.Invalid", "提供商名不能为空。");
        }

        return new ProviderName(value);
    }
}
