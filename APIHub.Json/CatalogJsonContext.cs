using System.Text.Json.Serialization;

namespace ApiHub.Json;

/// <summary>源生成的序列化上下文，供裁剪与 AOT 使用。</summary>
[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(CatalogDocument))]
internal partial class CatalogJsonContext : JsonSerializerContext {
}
