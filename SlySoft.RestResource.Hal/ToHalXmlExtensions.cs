using System.Xml;

namespace SlySoft.RestResource.Hal;

public static class ToHalXmlExtensions {
    /// <summary>
    /// Create a resource, formatted as XML using HAL, with extensions to support expanded links.
    /// MIME type = application/slysoft.hal+xml
    /// </summary>
    /// <param name="resource">Resource that contains the data to represent as xml</param>
    /// <returns>XML text in a HAL format (with slysoft extensions)</returns>
    public static string ToSlySoftHalXml(this Resource resource) {
        using var stringWriter = new StringWriter();
        using (var xmlWriter = XmlWriter.Create(stringWriter)) {
            xmlWriter.WriteResource(resource, "self");
        }

        return stringWriter.ToString();
    }

    private static void WriteResource(this XmlWriter xmlWriter, Resource resource, string name) {
        xmlWriter.WriteStartElement("resource");

        //if there's no URI and this is the root, we don't need to write "self"
        xmlWriter.WriteAttributeString("rel", name);

        if (!string.IsNullOrEmpty(resource.Uri)) {
            xmlWriter.WriteAttributeString("href", resource.Uri);
        }

        foreach (var data in resource.Data) {
            xmlWriter.AddData(data);
        }

        foreach (var link in resource.Links) {
            xmlWriter.AddLink(link);
        }

        foreach (var embedded in resource.EmbeddedResources) {
            xmlWriter.AddEmbedded(embedded.Key, embedded.Value);
        }

        xmlWriter.WriteEndElement();
    }

    private static void AddData(this XmlWriter xmlWriter, KeyValuePair<string, object?> data) {
        xmlWriter.WriteStartElement(data.Key);
        switch (data.Value) {
            case FormattedValue formattedValue: {
                xmlWriter.WriteValue(formattedValue.Value);
                break;

            }
            case IList<object?> listOfObjects: {
                foreach (var value in listOfObjects) {
                    xmlWriter.WriteStartElement("value");
                    if (value != default) {
                        xmlWriter.WriteValue(value);
                    }

                    xmlWriter.WriteEndElement();
                }

                break;
            }
            case ListData listData: {
                foreach (var objectData in listData) {
                    xmlWriter.WriteStartElement("value");
                    xmlWriter.WriteObjectData(objectData);
                    xmlWriter.WriteEndElement();
                }

                break;
            }
            case ObjectData objectData:
                xmlWriter.WriteObjectData(objectData);
                break;
            default:
                xmlWriter.WriteValue(data.Value ?? string.Empty);
                break;
        }

        xmlWriter.WriteEndElement();
    }

    private static void WriteObjectData(this XmlWriter xmlWriter, ObjectData objectData) {
        foreach (var data in objectData) {
            AddData(xmlWriter, data);
        }
    }

    private static void AddLink(this XmlWriter xmlWriter, Link link) {
        xmlWriter.WriteStartElement("link");
        xmlWriter.WriteAttributeString("rel", link.Name);
        xmlWriter.WriteAttributeString("href", link.Href);

        if (link.Templated) {
            xmlWriter.WriteAttributeString("templated", "true");
        }

        if (link.Verb != "GET") {
            xmlWriter.WriteAttributeString("verb", link.Verb);
        }
        
        if (link.Timeout != 0) {
            xmlWriter.WriteAttributeString("timeout", link.Timeout.ToString());
        }

        foreach (var linkParameter in link.Parameters) {
            xmlWriter.WriteStartElement(link.GetParameterTypeName());
            xmlWriter.WriteAttributeString("name", linkParameter.Name);

            if (!string.IsNullOrEmpty(linkParameter.Type)) {
                xmlWriter.WriteStartElement("type");
                xmlWriter.WriteValue(linkParameter.Type!);
                xmlWriter.WriteEndElement();
            }

            if (!string.IsNullOrEmpty(linkParameter.DefaultValue)) {
                xmlWriter.WriteStartElement("defaultValue");
                xmlWriter.WriteValue(linkParameter.DefaultValue!);
                xmlWriter.WriteEndElement();
            }

            if (linkParameter.ListOfValues.Any()) {
                xmlWriter.WriteStartElement("listOfValues");
                foreach (var value in linkParameter.ListOfValues) {
                    xmlWriter.WriteStartElement("value");
                    xmlWriter.WriteValue(value);
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }

        xmlWriter.WriteEndElement();
    }

    private static void AddEmbedded(this XmlWriter xmlWriter, string name, object resourceObject) {
        switch (resourceObject) {
            case Resource resource:
                xmlWriter.WriteResource(resource, name);
                return;
            case IList<Resource> resourceList: {
                foreach (var resourceListItem in resourceList) {
                    xmlWriter.WriteResource(resourceListItem, name);
                }
                return;
            }
        }
    }
}