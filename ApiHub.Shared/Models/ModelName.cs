using ErrorOr;

namespace ApiHub.Shared.Models;

/// <summary>目录内的本地模型名，用作选择与流程引用，不接受空或空白。相等与哈希按大小写敏感判定。</summary>
public sealed record ModelName
{
    private ModelName(string value) => Value = value;

    /// <summary>模型名原文。</summary>
    public string Value
    {
        get;
    }

    /// <summary>解析模型名，失败返回 Validation 错误。</summary>
    public static ErrorOr<ModelName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Error.Validation("ModelName.Invalid", "模型名不能为空。");
        }

        return new ModelName(value);
    }
}
