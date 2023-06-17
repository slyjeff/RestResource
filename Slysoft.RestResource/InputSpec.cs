namespace Slysoft.RestResource; 

/// <summary>
/// A class to define the elements of an input (ex: form field, query parameter)
/// </summary>
public class InputSpec {
    /// <summary>
    /// Create an Input Spec for a link
    /// </summary>
    /// <param name="name">Name of the input spec</param>
    public InputSpec(string name) {
        Name = name;
    }

    /// <summary>
    /// Name of the input spec
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Type of input (text, number, date, etc.)
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Default value for the inputc
    /// </summary>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// List of values that are acceptable for this input
    /// </summary>
    public IList<string> ListOfValues { get; } = new List<string>();
}