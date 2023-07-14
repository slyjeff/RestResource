namespace SlySoft.RestResource;

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

    /// <summary>
    /// Embedded resources; can be either single resources, or lists of resources
    /// </summary>
    public IDictionary<string, object> EmbeddedResources = new Dictionary<string, object>();

    /// <summary>
    /// Links for this resource
    /// </summary>
    public IList<Link> Links { get; } = new List<Link>();
}