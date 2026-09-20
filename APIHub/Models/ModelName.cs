using ErrorOr;

namespace ApiHub.Models;

/// <summary>模型名，直接作为请求的模型参数，不接受空或空白。相等与哈希按大小写敏感判定。</summary>
public sealed record ModelName {
    private ModelName(string value) => Value = value;

    /// <summary>模型名原文。</summary>
    public string Value {
        get;
    }

    /// <summary>解析模型名，失败返回 Validation 错误。</summary>
    public static ErrorOr<ModelName> Create(string value) {
        if (string.IsNullOrWhiteSpace(value)) {
            return Error.Validation("ModelName.Invalid", "模型名不能为空。");
        }

        return new ModelName(value);
    }
}
