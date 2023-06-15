using Resource.Tests.Utils;

namespace RestResource.Tests.Utils; 

internal sealed class TestObject {
    public string StringValue { get; set; } = GenerateRandom.String();
    public int IntValue { get; set; } = GenerateRandom.Int();
}