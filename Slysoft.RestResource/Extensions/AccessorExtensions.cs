namespace Slysoft.RestResource.Extensions; 

public static class AccessorExtensions {
    /// <summary>
    /// Find a link in a resource
    /// </summary>
    /// <param name="resource">Resource to search</param>
    /// <param name="linkName">Name of the link to find= case insensitive</param>
    /// <returns>Link matching the name, if one exists</returns>
    public static Link? GetLink(this Resource resource, string linkName) {
        return resource.Links.FirstOrDefault(x => x.Name.Equals(linkName, StringComparison.CurrentCultureIgnoreCase));
    }

    /// <summary>
    /// Find an input spec in a link
    /// </summary>
    /// <param name="link">link to search</param>
    /// <param name="inputSpecName">Name of the input spec to find= case insensitive</param>
    /// <returns>Input spec matching the name, if one exists</returns>
    public static InputSpec? GetInputSpec(this Link link, string inputSpecName) {
        return link.InputSpecs.FirstOrDefault(x => x.Name.Equals(inputSpecName, StringComparison.CurrentCultureIgnoreCase));
    }
}