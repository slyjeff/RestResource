namespace Slysoft.RestResource;

/// <summary>
/// Link contained by a resource so that applicable operations can be navigated (HATEOAS)
/// </summary>
public interface ILink {
    /// <summary>
    /// Human readable identifier of the link
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Location of the link
    /// </summary>
    string Uri { get; }
}

/// <summary>
/// Link contained by a resource so that applicable operations can be navigated (HATEOAS)
/// </summary>
public sealed class Link : ILink {
    /// <summary>
    /// Create a link to be assigned to a resource
    /// </summary>
    /// <param name="name">Human readable identifier of the link</param>
    /// <param name="uri">Location of the link</param>
    public Link(string name, string uri) {
        Name = name;
        Uri = uri;
    }

    /// <summary>
    /// Human readable identifier of the link
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Location of the link
    /// </summary>
    public string Uri { get; }
}