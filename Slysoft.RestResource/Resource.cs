namespace Slysoft.RestResource; 

/// <summary>
/// Represents a REST resource that can contain data, links, and embedded resource
/// </summary>
public sealed class Resource {
    /// <summary>
    /// URI of the resource that will be used to construct a "self" link
    /// </summary>
    public string Uri { get; set; } = string.Empty;

    /// <summary>
    /// Data values contained by this resource
    /// </summary>
    public IDictionary<string, object?> Data { get; } = new Dictionary<string, object?>();
}