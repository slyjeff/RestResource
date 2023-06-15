using System.Security.AccessControl;
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
                xmlWriter.WriteStartElement("Resource");

                if (!string.IsNullOrEmpty(resource.Uri)) {
                    xmlWriter.WriteAttributeString("rel", "self");
                    xmlWriter.WriteAttributeString("href", resource.Uri);
                }

                xmlWriter.WriteEndElement();
            }
            return stringWriter.ToString();
        }
    }
}