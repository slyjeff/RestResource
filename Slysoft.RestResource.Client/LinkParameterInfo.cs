using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Slysoft.RestResource.Client; 

/// <summary>
/// Information about a field/query parameter defined by the server
/// </summary>
public interface IParameterInfo {
    /// <summary>
    /// Type of parameter- does not need to mach the .net type. Can be more general (number, text, etc.)
    /// </summary>
    string? Type { get; }

    /// <summary>
    /// Initial value, and also the value that will be used if a null is provided
    /// </summary>
    string? DefaultValue { get; }

    /// <summary>
    /// List of values that are acceptable this parameter
    /// </summary>
    IReadOnlyList<string> ListOfValues { get; } 
}

internal sealed class LinkParameterInfo : IParameterInfo {
    public LinkParameterInfo(InputItem? inputItem = null) {
        if (inputItem == null) {
            return;
        }

        Type = inputItem.Type;
        DefaultValue = inputItem.DefaultValue;
        ListOfValues = new ReadOnlyCollection<string>(inputItem.ListOfValues);
    }

    public string? Type { get; }
    public string? DefaultValue { get; }
    public IReadOnlyList<string> ListOfValues { get; } = new ReadOnlyCollection<string>(new List<string>());
}