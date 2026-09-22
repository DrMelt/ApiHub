using ApiHub.Shared.Models;
using ErrorOr;

namespace ApiHub.Shared.Tests.Models;

public class ModelNameTests {
    [Theory]
    [InlineData("qwen-plus")]
    [InlineData("gpt-4o-mini")]
    [InlineData("us.anthropic.claude-v2:1")]
    public void 合法模型名通过(string value) {
        ModelName name = ModelName.Create(value).Value;

        Assert.Equal(value, name.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void 空模型名被拒(string value) {
        ErrorOr<ModelName> result = ModelName.Create(value);

        Assert.True(result.IsError);
        Assert.Equal("ModelName.Invalid", result.FirstError.Code);
    }

    [Fact]
    public void 大小写不同不相等() {
        Assert.Equal(ModelName.Create("qwen-plus").Value, ModelName.Create("qwen-plus").Value);
        Assert.NotEqual(ModelName.Create("qwen-plus").Value, ModelName.Create("Qwen-Plus").Value);
    }
}
