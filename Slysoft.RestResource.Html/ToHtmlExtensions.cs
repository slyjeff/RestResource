using System.Web.UI;

namespace Slysoft.RestResource.Html;

public static class ToHtmlExtensions {
    /// <summary>
    /// Create a resource, formatted as HTML
    /// MIME type = text/html
    /// </summary>
    /// <param name="resource">Resource that contains the data to represent as HTML</param>
    /// <returns>HTML text</returns>
    public static string ToHtml(this Resource resource) {
        using (var stringWriter = new StringWriter()) {
            using (var htmlWriter = new HtmlTextWriter(stringWriter)) {
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Html);

                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Body);

                foreach (var dataItem in resource.Data) {
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);
                    htmlWriter.Write(dataItem.Key);
                    htmlWriter.Write(dataItem.Value);
                    htmlWriter.RenderEndTag();
                }


                htmlWriter.RenderEndTag();
                htmlWriter.RenderEndTag();

                htmlWriter.Close();
            }

            return stringWriter.ToString();
        }
    }
}