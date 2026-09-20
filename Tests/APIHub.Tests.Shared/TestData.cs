using ApiHub.Models;
using ErrorOr;

namespace ApiHub.Tests.Shared;

/// <summary>测试用合法对象的构造口。</summary>
public static class TestData {
    public static T Ok<T>(ErrorOr<T> result) {
        Assert.False(result.IsError, string.Join(" ", result.Errors.Select(error => error.Description)));

        return result.Value;
    }

    public static ProviderName ProviderName(string value) => Ok(Models.ProviderName.Create(value));

    public static ModelName ModelName(string value) => Ok(Models.ModelName.Create(value));

    public static ApiKey Key(string value) => Ok(ApiKey.Create(value));

    public static ProviderEndpoint Endpoint(string value) => Ok(ProviderEndpoint.Create(value));

    public static ProviderDefinition Provider(
        string providerName,
        string baseAddress = "https://example.com/v1",
        string apiKey = "k") =>
        ProviderDefinition.Create(ProviderName(providerName), Endpoint(baseAddress), Key(apiKey));

    public static ModelDefinition Model(string modelName, string providerName = "dashscope") =>
        ModelDefinition.Create(ModelName(modelName), ProviderName(providerName));

    public static CatalogContents Contents(ProviderDefinition[] providers, ModelDefinition[] models) =>
        Ok(CatalogContents.Create([.. providers], [.. models]));
}
