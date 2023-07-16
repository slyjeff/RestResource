namespace SlySoft.RestResource;


public sealed class ObjectData : Dictionary<string, object?> {
}

public sealed class ListData : List<ObjectData> {
}

public sealed class EmbeddedResourceData : Dictionary<string, object> {
}
