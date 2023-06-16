namespace TestUtils; 

public sealed class TestObjectWithList {
    public string StringValue { get; } = GenerateRandom.String();
    public int IntValue { get; } = GenerateRandom.Int();
    public IList<TestObject> TestObjects { get; } = new List<TestObject> { new(), new(), new()};
}