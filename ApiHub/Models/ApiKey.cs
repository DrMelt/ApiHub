using ErrorOr;

namespace ApiHub.Models;

/// <summary>接入凭据，不接受空或空白。文本形式为掩码，不暴露凭据原文。</summary>
public sealed record ApiKey {
    private ApiKey(string value) => Value = value;

    /// <summary>凭据原文。</summary>
    public string Value {
        get;
    }

    /// <summary>解析凭据，失败返回 Validation 错误。</summary>
    public static ErrorOr<ApiKey> Create(string value) {
        if (string.IsNullOrWhiteSpace(value)) {
            return Error.Validation("ApiKey.Invalid", "密钥不能为空。");
        }

        return new ApiKey(value);
    }

    /// <summary>输出固定掩码，避免凭据随对象进入日志。</summary>
    public override string ToString() => "ApiKey { Value = *** }";
}
