using ErrorOr;

namespace ApiHub.Shared.Models;

/// <summary>提供商使用的模型名，作为请求的模型参数，不接受空或空白。相等与哈希按大小写敏感判定。</summary>
public sealed record ProviderModelName
{
    private ProviderModelName(string value) => Value = value;

    /// <summary>提供商模型名原文。</summary>
    public string Value
    {
        get;
    }

    /// <summary>解析提供商模型名，失败返回 Validation 错误。</summary>
    public static ErrorOr<ProviderModelName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Error.Validation("ProviderModelName.Invalid", "提供商模型名不能为空。");
        }

        return new ProviderModelName(value);
    }
}
