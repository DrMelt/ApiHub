using ApiHub.Shared.Models;
using ApiHub.Tests.Fixture;
using ErrorOr;

namespace ApiHub.Shared.Tests.Models;

public class ModelDefinitionTests
{
    [Fact]
    public void 由类型对象字段构成定义()
    {
        ModelDefinition model = TestData.Model("qwen-plus", contextLength: 8192);

        Assert.Equal("qwen-plus", model.ModelName.Value);
        Assert.Equal("dashscope", model.ProviderName.Value);
        Assert.Equal("qwen-plus", model.ProviderModelName.Value);
        Assert.Equal(8192, model.ContextLength!.Value);
    }

    [Fact]
    public void 本地模型名与提供商模型名可以不同()
    {
        ModelDefinition model = TestData.Model("qwen-plus-aliyun", "aliyun", "qwen-plus", contextLength: 8192);

        Assert.Equal("qwen-plus-aliyun", model.ModelName.Value);
        Assert.Equal("qwen-plus", model.ProviderModelName.Value);
        Assert.Equal(8192, model.ContextLength!.Value);
    }

    [Fact]
    public void 上下文长度可缺省()
    {
        ModelDefinition model = ModelDefinition.Create(
            TestData.ModelName("qwen-plus"),
            TestData.ProviderName("dashscope"),
            TestData.ProviderModelName("qwen-plus"));

        Assert.Null(model.ContextLength);
        Assert.Equal("qwen-plus", model.ProviderModelName.Value);
    }

    [Fact]
    public void 字段错误由类型对象给出()
    {
        ErrorOr<ModelName> modelName = ModelName.Create(" ");
        ErrorOr<ProviderName> providerName = ProviderName.Create(" ");
        ErrorOr<ProviderModelName> providerModelName = ProviderModelName.Create(" ");
        ErrorOr<ModelContextLength> contextLength = ModelContextLength.Create(0);

        Assert.Equal("ModelName.Invalid", modelName.FirstError.Code);
        Assert.Equal("ProviderName.Invalid", providerName.FirstError.Code);
        Assert.Equal("ProviderModelName.Invalid", providerModelName.FirstError.Code);
        Assert.Equal("ModelContextLength.Invalid", contextLength.FirstError.Code);
    }
}
