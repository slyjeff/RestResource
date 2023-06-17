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
    /// Find an input item in a link
    /// </summary>
    /// <param name="link">link to search</param>
    /// <param name="inputItemName">Name of the input item to find= case insensitive</param>
    /// <returns>Input item matching the name, if one exists</returns>
    public static InputItem? GetInputItem(this Link link, string inputItemName) {
        return link.InputItems.FirstOrDefault(x => x.Name.Equals(inputItemName, StringComparison.CurrentCultureIgnoreCase));
    }

    /// <summary>
    /// Get the type of input item this link supports (parameter, field)
    /// </summary>
    /// <param name="link">link with the input item</param>
    /// <returns>Type of input item this link supports (parameter, field)</returns>
    public static string GetInputItemName(this Link link) {
        return link.Verb.Equals("GET", StringComparison.CurrentCultureIgnoreCase) ? "parameter" : "field";
    }
}