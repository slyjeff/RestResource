namespace TestUtils;

public interface IHasStringValue {
    string StringValue { get; }
}

public sealed class TestObject : IHasStringValue {
    public string StringValue { get; set; } = GenerateRandom.String();
    public int IntValue { get; set; } = GenerateRandom.Int();
}