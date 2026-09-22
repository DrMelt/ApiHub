using ErrorOr;

namespace ApiHub.Shared.Models;

/// <summary>OpenAI 兼容端点：http 或 https 的绝对地址。地址按 Uri 规范化，文本形式可能与输入不同。</summary>
public sealed record ProviderEndpoint {
    private ProviderEndpoint(Uri address) => Address = address;

    /// <summary>端点地址。</summary>
    public Uri Address {
        get;
    }

    /// <summary>解析端点，失败返回 Validation 错误。</summary>
    public static ErrorOr<ProviderEndpoint> Create(string value) {
        if (Uri.TryCreate(value, UriKind.Absolute, out Uri? address) && address.Scheme is "http" or "https") {
            return new ProviderEndpoint(address);
        }

        return Error.Validation("ProviderEndpoint.Invalid", "端点必须以 http 或 https 开头。");
    }
}
