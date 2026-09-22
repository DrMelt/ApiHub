using ApiHub.Shared.Models;
using ApiHub.Tests.Fixture;
using ErrorOr;

namespace ApiHub.Shared.Tests.Models;

public class ModelDefinitionTests {
    [Fact]
    public void 由类型对象字段构成定义() {
        ModelDefinition model = TestData.Model("qwen-plus");

        Assert.Equal("qwen-plus", model.ModelName.Value);
        Assert.Equal("dashscope", model.ProviderName.Value);
    }

    [Fact]
    public void 字段错误由类型对象给出() {
        ErrorOr<ModelName> modelName = ModelName.Create(" ");
        ErrorOr<ProviderName> providerName = ProviderName.Create(" ");

        Assert.Equal("ModelName.Invalid", modelName.FirstError.Code);
        Assert.Equal("ProviderName.Invalid", providerName.FirstError.Code);
    }
}
