using System;

namespace SlySoft.RestResource.Client; 

/// <summary>
/// Add to a bool property to return whether a link exists in the resource- by convention will look for a link named "Can[PropertyName]"- a link name can also be specified in the constructor
/// </summary>
public sealed class LinkCheckAttribute : Attribute {
    /// <summary>
    /// Make the property return true if a link with "Can[PropertyName]" exists in the resource
    /// </summary>
    public LinkCheckAttribute() {
    }

    /// <summary>
    /// Make the property return true if a link with linkName exists in the resource
    /// </summary>
    /// <param name="linkName">Name of a link to look for in the resource</param>
    public LinkCheckAttribute(string linkName) {
        LinkName = linkName;
    }

    /// <summary>
    /// Name of a link to look for in the resource
    /// </summary>
    public string? LinkName { get; }
}

/// <summary>
/// Put on a IParameterInfo property to have it return info about a parameter (default value, list of values, type)
/// </summary>
public sealed class ParameterInfoAttribute : Attribute {
    /// <summary>
    /// Make a property return inf about a parameter in a link
    /// </summary>
    /// <param name="linkName">Name of the link</param>
    /// <param name="parameterName">A parameter in the link- this property will return info about the parameter</param>
    public ParameterInfoAttribute(string linkName, string parameterName) {
        LinkName = linkName;
        ParameterName = parameterName;
    }

    public string LinkName { get; }
    public string ParameterName { get; }
}