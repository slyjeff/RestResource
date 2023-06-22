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

        resource.GetLinks(xElement);

        return resource;
    }

    private static void GetUri(this Resource resource, XElement xElement) {
        var hrefElement = xElement.Attributes().FirstOrDefault(x => x.Name == "href");
        if (hrefElement == null) {
            return;
        }

        resource.Uri = hrefElement.Value;
    }

    private static void GetData(this Resource resource, XContainer xElement) {
        PopulateFromXElement(resource.Data, xElement);
    }

    private static void PopulateFromXElement(this IDictionary<string, object?> dictionary, XContainer xElement) {
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

    private static void GetLinks(this Resource resource, XContainer xElements) {
        foreach (var element in xElements.Elements()) {
            if (element.Name.LocalName != "link") {
                continue;
            }

            var name = element.Attribute("rel")?.Value;
            if (name == null) {
                continue;
            }

            var href = element.Attribute("href")?.Value;
            if (href == null) {
                continue;
            }

            var verb = element.Attribute("verb")?.Value ?? "GET";

            bool.TryParse(element.Attribute("templated")?.Value ?? "false", out var templated);

            int.TryParse(element.Attribute("timeout")?.Value ?? "0", out var timeout);
            
            var link = new Link(name, href, verb: verb, templated: templated, timeout: timeout);

            foreach (var inputItemElement in element.Elements()) {
                if (inputItemElement.Name.LocalName is not ("parameter" or "field")) {
                    continue;
                }

                var inputElementName = inputItemElement.Attribute("name")?.Value;
                if (inputElementName == null) {
                    continue;
                }

                var inputItem = new InputItem(inputElementName);
                link.InputItems.Add(inputItem);

                foreach (var inputItemDataElement in inputItemElement.Elements()) {
                    if (inputItemDataElement.Name.LocalName == "defaultValue") {
                        inputItem.DefaultValue = inputItemDataElement.Value;
                        continue;
                    }

                    if (inputItemDataElement.Name.LocalName == "type") {
                        inputItem.Type = inputItemDataElement.Value;
                    }

                    if (inputItemDataElement.Name.LocalName == "listOfValues") {
                        foreach (var value in inputItemDataElement.Elements()) {
                            inputItem.ListOfValues.Add(value.Value);
                        }
                    }
                }
            }

            resource.Links.Add(link);
        }
    }

}