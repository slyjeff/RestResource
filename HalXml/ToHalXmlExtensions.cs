using System.Xml;
using RestResource;

namespace HalXml; 

public static class ToHalXmlExtensions {
    /// <summary>
    /// Create a resource, formatted as XML using HAL, with extensions to support expanded links.
    /// MIME type = application/slysoft.hal+xml
    /// </summary>
    /// <param name="resource">Resource that contains the data to represent as json</param>
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

                xmlWriter.WriteEndElement();
            }
            return stringWriter.ToString();
        }
    }

    private static void AddData(this XmlWriter xmlWriter, KeyValuePair<string, object?> data) {
        xmlWriter.WriteStartElement(data.Key);
        switch (data.Value) {
            case string stringValue:
                xmlWriter.WriteValue(stringValue);
                break;
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
        }
        xmlWriter.WriteEndElement();
    }

    private static void WriteDictionary(this XmlWriter xmlWriter, IDictionary<string, object?> dictionary) {
        foreach (var data in dictionary) {
            AddData(xmlWriter, data);
        }
    }
}