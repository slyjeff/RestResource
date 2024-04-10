namespace SlySoft.RestResource;

public sealed class FormattedValue  {
    public FormattedValue(object value, string format) {
        Value = string.Format($"{{0:{format}}}", value);
        OriginalType = value.GetType();
    }

    public string Value { get; }
    public Type OriginalType { get; }

    public override string ToString() {
        return Value;
    }
}