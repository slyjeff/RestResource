namespace TestUtils;

public sealed class TestObject2 {
    public DateTime DateValue { get; set; } = GenerateRandom.DateTime();
    public float FloatValue { get; set; } = GenerateRandom.Float();
}