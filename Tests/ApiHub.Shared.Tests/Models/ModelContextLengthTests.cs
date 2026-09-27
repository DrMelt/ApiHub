using ApiHub.Shared.Models;
using ErrorOr;

namespace ApiHub.Shared.Tests.Models;

public class ModelContextLengthTests {
    [Theory]
    [InlineData(1)]
    [InlineData(32768)]
    public void 合法上下文长度通过(int value) {
        ModelContextLength contextLength = ModelContextLength.Create(value).Value;

        Assert.Equal(value, contextLength.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void 非正上下文长度被拒(int value) {
        ErrorOr<ModelContextLength> result = ModelContextLength.Create(value);

        Assert.True(result.IsError);
        Assert.Equal("ModelContextLength.Invalid", result.FirstError.Code);
    }
}
