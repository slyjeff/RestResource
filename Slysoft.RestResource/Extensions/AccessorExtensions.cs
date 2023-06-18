using System.Drawing;

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

    /// <summary>
    /// Get a list of parameters in a templated link href
    /// </summary>
    /// <param name="link">Link containing the parameters</param>
    /// <returns>List of parameters</returns>
    public static IEnumerable<string> GetParameters(this Link link) {
        var parameters = new List<string>();
        if (!link.Templated) {
            return parameters;
        }

        for (var index = 0; index < link.Href.Length; index++) {
            if (link.Href[index] != '{') {
                continue;
            }

            var closingBracketIndex = link.Href.IndexOf('}', index);
            if (closingBracketIndex < index) {
                continue;
            }

            var parameterStart = index + 1;
            var parameterEnd = closingBracketIndex - 1;
            parameters.Add(link.Href.Substring(parameterStart, parameterEnd - parameterStart));
        }

        return parameters;
    }
}