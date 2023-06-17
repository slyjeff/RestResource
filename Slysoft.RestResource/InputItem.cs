namespace Slysoft.RestResource; 

/// <summary>
/// A class to define an item for input (ex: form field, query parameter)
/// </summary>
public class InputItem {
    /// <summary>
    /// Create an input item for a link
    /// </summary>
    /// <param name="name">Name of the input item</param>
    public InputItem(string name) {
        Name = name;
    }

    /// <summary>
    /// Name of the input item
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Type of input (text, number, date, etc.)
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Default value for the input item
    /// </summary>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// List of values that are acceptable for this input
    /// </summary>
    public IList<string> ListOfValues { get; } = new List<string>();
}