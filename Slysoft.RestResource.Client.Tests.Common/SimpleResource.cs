using TestUtils;

namespace Slysoft.RestResource.Client.Tests.Common;

public interface ISimpleResource {
    string Message { get; }
}

public sealed class SimpleResource {
    public string Message { get; set; } = GenerateRandom.String();
}