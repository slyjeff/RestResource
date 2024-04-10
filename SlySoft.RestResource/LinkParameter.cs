namespace SlySoft.RestResource;

/// <summary>
/// A class to define an item for input (ex: form field, query parameter)
/// </summary>
public class LinkParameter {
    /// <summary>
    /// Create a link parameter (query parameter for get, form field for post/put
    /// </summary>
    /// <param name="name">Name of the parameter</param>
    public LinkParameter(string name) {
        Name = name;
    }

    /// <summary>
    /// Name of the parameter
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Type of parameter (text, number, date, etc.)
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Default value for the parameter
    /// </summary>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// List of values that are acceptable for this parameter
    /// </summary>
    public IList<string> ListOfValues { get; } = new List<string>();
}