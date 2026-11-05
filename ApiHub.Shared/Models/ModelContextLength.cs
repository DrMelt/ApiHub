using ErrorOr;

namespace ApiHub.Shared.Models;

/// <summary>模型上下文窗口长度，按 token 计。必须大于 0。</summary>
public sealed record ModelContextLength
{
    private ModelContextLength(int value) => Value = value;

    /// <summary>上下文窗口长度。</summary>
    public int Value
    {
        get;
    }

    /// <summary>解析上下文窗口长度，失败返回 Validation 错误。</summary>
    public static ErrorOr<ModelContextLength> Create(int value) =>
        value > 0
            ? new ModelContextLength(value)
            : Error.Validation("ModelContextLength.Invalid", "上下文长度必须大于 0。");
}
