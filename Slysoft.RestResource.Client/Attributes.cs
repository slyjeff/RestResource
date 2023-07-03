using System;

namespace Slysoft.RestResource.Client; 

public sealed class LinkCheckAttribute : Attribute {
    public LinkCheckAttribute() {
    }

    public LinkCheckAttribute(string linkName) {
        LinkName = linkName;
    }

    public string? LinkName { get; }
}