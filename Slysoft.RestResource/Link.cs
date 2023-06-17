namespace Slysoft.RestResource;

/// <summary>
/// Link contained by a resource so that applicable operations can be navigated (HATEOAS)
/// </summary>
public sealed class Link {
    /// <summary>
    /// Create a link to be assigned to a resource
    /// </summary>
    /// <param name="name">Human readable identifier of the link</param>
    /// <param name="href">Location of the link</param>
    /// <param name="verb">HTTP verb of the link- defaults to "GET"</param>
    /// <param name="templated">Whether or not the URI is templated</param>
    public Link(string name, string href, string verb = "GET", bool templated = false) {
        Name = name;
        Href = href;
        Verb = verb;
        Templated = templated;
    }

    /// <summary>
    /// Human readable identifier of the link
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Location of the link
    /// </summary>
    public string Href { get; }

    /// <summary>
    /// HTTP verb of the link
    /// </summary>
    public string Verb { get; }

    /// <summary>
    /// Whether or not the URI is templated
    /// </summary>
    public bool Templated { get; }

    /// <summary>
    /// List of of input items that contain information for interacting with the link (ex: form fields, query parameters)
    /// </summary>
    public IList<InputItem> InputItems { get; } = new List<InputItem>();
}