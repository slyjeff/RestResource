using System.Xml;
using Slysoft.RestResource.Extensions;

namespace Slysoft.RestResource.HalXml;

public static class ToHalXmlExtensions {
    /// <summary>
    /// Create a resource, formatted as XML using HAL, with extensions to support expanded links.
    /// MIME type = application/slysoft.hal+xml
    /// </summary>
    /// <param name="resource">Resource that contains the data to represent as xml</param>
    /// <returns>XML text in a HAL format (with slysoft extensions)</returns>
    public static string ToHalXml(this Resource resource) {
        using (var stringWriter = new StringWriter()) {
            using (var xmlWriter = XmlWriter.Create(stringWriter)) {
                xmlWriter.WriteStartElement("resource");

                if (!string.IsNullOrEmpty(resource.Uri)) {
                    xmlWriter.WriteAttributeString("rel", "self");
                    xmlWriter.WriteAttributeString("href", resource.Uri);
                }

                foreach (var data in resource.Data) {
                    xmlWriter.AddData(data);
                }

                foreach (var link in resource.Links) {
                    xmlWriter.AddLink(link);
                }

                xmlWriter.WriteEndElement();
            }

            return stringWriter.ToString();
        }
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
            case IList<IDictionary<string, object?>> listOfDictionary: {
                foreach (var dictionary in listOfDictionary) {
                    xmlWriter.WriteStartElement("value");
                    xmlWriter.WriteDictionary(dictionary);
                    xmlWriter.WriteEndElement();
                }

                break;
            }
            case IDictionary<string, object?> dictionaryObject:
                xmlWriter.WriteDictionary(dictionaryObject);
                break;
            default:
                xmlWriter.WriteValue(data.Value ?? string.Empty);
                break;
        }

        xmlWriter.WriteEndElement();
    }

    private static void WriteDictionary(this XmlWriter xmlWriter, IDictionary<string, object?> dictionary) {
        foreach (var data in dictionary) {
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

        foreach (var inputItem in link.InputItems) {
            xmlWriter.WriteStartElement(link.GetInputItemName());
            xmlWriter.WriteAttributeString("name", inputItem.Name);

            if (!string.IsNullOrEmpty(inputItem.Type)) {
                xmlWriter.WriteStartElement("type");
                xmlWriter.WriteValue(inputItem.Type);
                xmlWriter.WriteEndElement();
            }

            if (!string.IsNullOrEmpty(inputItem.DefaultValue)) {
                xmlWriter.WriteStartElement("defaultValue");
                xmlWriter.WriteValue(inputItem.DefaultValue);
                xmlWriter.WriteEndElement();
            }

            if (inputItem.ListOfValues.Any()) {
                xmlWriter.WriteStartElement("listOfValues");
                foreach (var value in inputItem.ListOfValues) {
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
}
