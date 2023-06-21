using System.Collections;
using System.Xml.Linq;

namespace Slysoft.RestResource.HalXml; 

public static class FromHalXmlExtensions {
    /// <summary>
    /// Populate a resource from an XML string in slysoft.hal+xml or hal+xml format
    /// </summary>
    /// <param name="resource">The resource to populate</param>
    /// <param name="xml">XML in slysoft.hal+xml format or hal+lmx format</param>
    /// <returns>The resource for ease of reference</returns>
    public static Resource FromHalXml(this Resource resource, string xml) {
        var xElement = XElement.Parse(xml);

        resource.GetUri(xElement);

        resource.GetData(xElement);

        return resource;
    }

    private static void GetUri(this Resource resource, XElement xElement) {
        var hrefElement = xElement.Attributes().FirstOrDefault(x => x.Name == "href");
        if (hrefElement == null) {
            return;
        }

        resource.Uri = hrefElement.Value;
    }

    private static void GetData(this Resource resource, XElement xElement) {
        PopulateFromXElement(resource.Data, xElement);
    }

    private static void PopulateFromXElement(this IDictionary<string, object?> dictionary, XElement xElement) {
        foreach (var element in xElement.Elements()) {
            if (element.Name.LocalName is "link" or "resource") {
                continue;
            }

            var children = element.Elements().ToList();
            if (children.Count > 1 && children.All(x => x.Name.LocalName == "value")) {
                dictionary[element.Name.LocalName] = children.ToList();
                continue;
            }

            if (children.Any()) {
                var childDictionary = new Dictionary<string, object?>();
                childDictionary.PopulateFromXElement(element);
                dictionary[element.Name.LocalName] = childDictionary;
                continue;
            }

            dictionary[element.Name.LocalName] = element.Value;
        }
    }

    private static IEnumerable ToList(this List<XElement> xElements) {
        var firstValue = xElements.FirstOrDefault();
        if (firstValue == default) {
            return new List<object>();
        }

        if (!firstValue.Elements().Any()) {
            return xElements.Select(x => (object ? )x.Value).ToList();
        }

        var list = new List<IDictionary<string, object?>>();
        foreach (var childElement in xElements) {
            var dictionary = new Dictionary<string, object?>();
            dictionary.PopulateFromXElement(childElement);
            list.Add(dictionary);
        }

        return list;
    }
}