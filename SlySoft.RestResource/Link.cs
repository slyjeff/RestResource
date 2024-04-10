namespace SlySoft.RestResource;

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
    /// <param name="timeout">A suggested timeout in seconds that the client should use when calling this link</param>
    public Link(string name, string href, string verb = "GET", bool templated = false, int timeout = 0) {
        Name = name;
        Href = href;
        Verb = verb;
        Templated = templated;
        Timeout = timeout;
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
    /// List of of parameter this link expects (query parameters for the URL, form fields for the body)
    /// </summary>
    public IList<LinkParameter> Parameters { get; } = new List<LinkParameter>();

    /// <summary>
    /// A suggested timeout in seconds that the client should use when calling this link
    /// </summary>
    public int Timeout { get; }
}