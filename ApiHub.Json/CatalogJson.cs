using ApiHub.Shared.Models;
using ErrorOr;
using System.Text.Json;

namespace ApiHub.Json;

/// <summary>目录内容与 JSON 文本的互转。</summary>
public static class CatalogJson {
    private const string InvalidJsonCode = "CatalogJson.InvalidJson";

    /// <summary>导出目录内容为 JSON 文本。提供商与模型按名序数排序，同一内容导出的文本一致。</summary>
    public static string ToJson(this CatalogContents contents) {
        CatalogDocument document = new() {
            Providers = [.. contents.Providers
                .OrderBy(provider => provider.ProviderName.Value, StringComparer.Ordinal)
                .Select(provider => new ProviderDocument {
                    ProviderName = provider.ProviderName.Value,
                    BaseAddress = provider.BaseAddress.Address.ToString(),
                    ApiKey = provider.ApiKey.Value
                })],
            Models = [.. contents.Models
                .OrderBy(model => model.ModelName.Value, StringComparer.Ordinal)
                .Select(model => new ModelDocument {
                    ModelName = model.ModelName.Value,
                    ProviderName = model.ProviderName.Value
                })]
        };

        return JsonSerializer.Serialize(document, CatalogJsonContext.Default.CatalogDocument);
    }

    /// <summary>解析 JSON 为目录内容。键名大小写不敏感，集合缺失或为 null 视为空集合，未知属性忽略。字段错误一次报全，字段全部合法后再校验集合自一致性。</summary>
    public static ErrorOr<CatalogContents> Parse(string json) {
        if (string.IsNullOrWhiteSpace(json)) {
            return Error.Validation(InvalidJsonCode, "JSON 不能为空。");
        }

        CatalogDocument? document;
        try {
            document = JsonSerializer.Deserialize(json, CatalogJsonContext.Default.CatalogDocument);
        }
        catch (JsonException exception) {
            return Error.Validation(InvalidJsonCode, $"JSON 无法解析：{exception.Message}");
        }

        if (document is null) {
            return Error.Validation(InvalidJsonCode, "JSON 为 null。");
        }

        List<Error> errors = [];
        List<ProviderDefinition> providers = [];
        foreach (ProviderDocument? item in document.Providers ?? []) {
            if (item is null) {
                errors.Add(Error.Validation(InvalidJsonCode, "提供商列表含 null 项。"));
                continue;
            }

            ErrorOr<ProviderName> providerName = ProviderName.Create(item.ProviderName ?? "");
            ErrorOr<ProviderEndpoint> baseAddress = ProviderEndpoint.Create(item.BaseAddress ?? "");
            ErrorOr<ApiKey> apiKey = ApiKey.Create(item.ApiKey ?? "");
            AddErrors(providerName, errors);
            AddErrors(baseAddress, errors);
            AddErrors(apiKey, errors);

            if (providerName.IsError || baseAddress.IsError || apiKey.IsError) {
                continue;
            }

            providers.Add(ProviderDefinition.Create(providerName.Value, baseAddress.Value, apiKey.Value));
        }

        List<ModelDefinition> models = [];
        foreach (ModelDocument? item in document.Models ?? []) {
            if (item is null) {
                errors.Add(Error.Validation(InvalidJsonCode, "模型列表含 null 项。"));
                continue;
            }

            ErrorOr<ModelName> modelName = ModelName.Create(item.ModelName ?? "");
            ErrorOr<ProviderName> providerName = ProviderName.Create(item.ProviderName ?? "");
            AddErrors(modelName, errors);
            AddErrors(providerName, errors);

            if (modelName.IsError || providerName.IsError) {
                continue;
            }

            models.Add(ModelDefinition.Create(modelName.Value, providerName.Value));
        }

        if (errors.Count > 0) {
            return errors;
        }

        return CatalogContents.Create([.. providers], [.. models]);
    }

    private static void AddErrors<T>(ErrorOr<T> result, List<Error> errors) {
        if (result.IsError) {
            errors.AddRange(result.Errors);
        }
    }
}
