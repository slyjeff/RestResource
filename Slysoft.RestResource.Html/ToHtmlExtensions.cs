using System.Collections;
using Slysoft.RestResource.Extensions;
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
        using var stringWriter = new StringWriter();
        using (var htmlWriter = new HtmlTextWriter(stringWriter)) {
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Html);

            WriteStyles(htmlWriter);

            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Body);

            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "container");
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);

            var scripts = new List<string>();

            WriteResourceAsHtml(htmlWriter, resource, scripts);

            WriteScripts(htmlWriter, scripts);

            htmlWriter.RenderEndTag(); //div
            htmlWriter.RenderEndTag(); //body
            htmlWriter.RenderEndTag(); //html

            htmlWriter.Close();
        }

        return stringWriter.ToString();
    }

    private const string Css =
        @"body{
    margin: 0;
    padding: 0;
    color: #333;
    background-color: #eee;
    font: 1em/1.2 'Helvetica Neue', Helvetica, Arial, Geneva, sans-serif;
}

h1,h2,h3 {
    margin: 0 0 .5em;
    font-weight: 500;
    line-height: 1.1;
}

h1 { font-size: 2.25em; }
h2 { font-size: 1.375em; }
h3 { font-size: 1.375em; background: lightgrey; padding: 0.25em }

p {
    margin: 0 0 1.5em;
    line-height: 1.5;
}

 

table {
    background-color: transparent;
    border-spacing: 0;
    border-collapse: collapse;
    border-top: 1px solid #ddd;
    width: 100%
}

th, td {
    padding: .5em 1em;
    vertical-align: center;
    text-align: left;
    border-bottom: 1px solid #ddd;
}

td:last-child {
    width:100%;
}


.btn {
    color: #fff !important;
    background-color: GRAY;
    border-color: #222;
    display: inline-block;
    padding: .5em 1em;
    font-weight: 400;
    line-height: 1.2;
    text-align: center;
    white-space: nowrap;
    vertical-align: middle;
    cursor: pointer;
    border: 1px solid transparent;
    border-radius: .2em;
    text-decoration: none;
}

.btn:hover {
    color: #fff !important;
    background-color: #555;
}

.btn:focus {
    color: #fff !important;
    background-color: #555;
}

.btn:active {
    color: #fff !important;
    background-color: #555;
}

.container {
    max-width: 70em;
    margin: 0 auto;
    background-color: #fff;
}

.header {
    color: #fff;
    background: #555;
    p
}

.subheader {
    color: #fff;
    background: #555;
    p
}

.header-heading { margin: 0; }

.content { padding: 1em 1.25em; }

.embedded { padding-left: 1.5em }

@media (min-width: 42em) {
    .header { padding: 1.5em 3em; }
    .subheader { padding: .2em 3em; }
    .content { padding: 2em 3em; }
}";

    private static void WriteStyles(HtmlTextWriter htmlWriter) {
        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Style);
        htmlWriter.Write(Css);
        htmlWriter.RenderEndTag(); //style
    }

    private static void WriteHeader(HtmlTextWriter htmlWriter, Resource resource, bool isRoot) {
        htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, isRoot ? "header" : "subheader");
        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);

        htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "header-heading");
        htmlWriter.RenderBeginTag(isRoot ? HtmlTextWriterTag.H1 : HtmlTextWriterTag.H2);

        htmlWriter.Write(resource.GetResourceName());
        htmlWriter.RenderEndTag(); // H1/H2

        htmlWriter.RenderEndTag(); //div
    }

    private static string GetResourceName(this Resource resource) {
        if (resource.Uri == "/") {
            return "application";
        }

        //find the last segment that isn't a number
        var segments = resource.Uri.Split('/');
        foreach (var segment in segments.Reverse()) {
            if (!int.TryParse(segment, out var _)) {
                return segment;
            }
        }

        return resource.Uri;
    }

    private static void WriteResourceAsHtml(HtmlTextWriter htmlWriter, Resource resource, IList<string> scripts, bool isRoot = true) {
        WriteHeader(htmlWriter, resource, isRoot);

        htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "content");
        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);
        WriteSelfLink(htmlWriter, resource);
        WriteData(htmlWriter, resource.Data);
        WriteLinks(htmlWriter, resource, scripts);
        WriteEmbedded(htmlWriter, resource, scripts);
        htmlWriter.RenderEndTag(); //div
    }

    private static void WriteSelfLink(HtmlTextWriter htmlWriter, Resource resource) {
        if (string.IsNullOrEmpty(resource.Uri)) {
            return;
        }

        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);

        htmlWriter.AddAttribute(HtmlTextWriterAttribute.Href, resource.Uri);
        htmlWriter.RenderBeginTag(HtmlTextWriterTag.A);
        htmlWriter.Write(resource.Uri);
        htmlWriter.RenderEndTag(); //a

        htmlWriter.RenderEndTag(); //div

        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Br);
        htmlWriter.RenderEndTag(); //br
    }

    private static void WriteData(HtmlTextWriter htmlWriter, IDictionary<string, object?> data) {
        if (data.Count == 0) {
            return;
        }

        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Table);

        foreach (var dataItem in data) {
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Tr);

            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Td);
            htmlWriter.Write(dataItem.Key);
            htmlWriter.RenderEndTag(); //td

            switch (dataItem.Value) {
                case IList<IDictionary<string, object?>> list:
                    WriteListOfObjects(htmlWriter, list);
                    break;
                case string stringValue:
                    WriteValue(htmlWriter, stringValue);
                    break;
                case IEnumerable enumerable:
                    WriteEnumerable(htmlWriter, enumerable);
                    break;
                default:
                    WriteValue(htmlWriter, dataItem.Value);
                    break;
            }

            htmlWriter.RenderEndTag(); //tr
        }

        htmlWriter.RenderEndTag(); //table
    }

    private static void WriteListOfObjects(HtmlTextWriter htmlWriter, IList<IDictionary<string, object?>> list) {
        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Td);

        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Table);
        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Tr);

        var firstItem = list.FirstOrDefault();
        if (firstItem != null) {
            foreach (var data in firstItem) {
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Th);
                htmlWriter.Write(data.Key);
                htmlWriter.RenderEndTag(); //th
            }
        }

        htmlWriter.RenderEndTag(); //tr

        foreach (var item in list) {
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Tr);

            foreach (var data in item) {
                switch (data.Value) {
                    case IList<IDictionary<string, object?>> subList:
                        WriteListOfObjects(htmlWriter, subList);
                        break;
                    case string stringValue:
                        WriteValue(htmlWriter, stringValue);
                        break;
                    case IEnumerable enumerable:
                        WriteEnumerable(htmlWriter, enumerable);
                        break;
                    default:
                        WriteValue(htmlWriter, data.Value);
                        break;
                }
            }

            htmlWriter.RenderEndTag(); //tr
        }

        htmlWriter.RenderEndTag(); //table
        htmlWriter.RenderEndTag(); //td
    }

    private static void WriteEnumerable(HtmlTextWriter htmlWriter, IEnumerable enumerable) {
        var values = (from object item in enumerable select item.ToString()).ToList();
        WriteValue(htmlWriter, string.Join(", ", values));
    }

    private static void WriteValue(HtmlTextWriter htmlWriter, object? value) {
        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Td);
        htmlWriter.Write(value);
        htmlWriter.RenderEndTag(); //td
    }

    private const string UpdateLinkScript =
        @"var {link}_link= document.getElementById('{link}_link');
              var {link}_input= document.getElementById('{link}_input');
              {link}_input.onchange={link}_input.onkeyup= function() {
                  {link}_link.href = '{uri1}' + encodeURIComponent({link}_input.value) + '{uri2}';
              };";

    private static void WriteLinks(HtmlTextWriter htmlWriter, Resource resource, ICollection<string> scripts) {
        if (resource.Links.Count == 0) {
            return;
        }

        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Br);
        htmlWriter.RenderEndTag(); //br

        htmlWriter.RenderBeginTag(HtmlTextWriterTag.H3);
        htmlWriter.Write("Actions");
        htmlWriter.RenderEndTag(); //h3

        htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "embedded");

        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);
        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Table);

        foreach (var link in resource.Links) {
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Tr);

            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Td);
            htmlWriter.Write(link.Name);
            htmlWriter.RenderEndTag(); //td

            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Td);

            var parameter = link.GetParameters().FirstOrDefault();

            switch (link.Verb) {
                case "GET" when link.InputItems.Count > 0: {
                    var href = link.Href;

                    htmlWriter.AddAttribute("action", href);
                    htmlWriter.AddAttribute("method", "GET");
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Form);

                    if (!string.IsNullOrEmpty(parameter)) {
                        WriteQueryParameter(htmlWriter, parameter, string.Empty);
                    }

                    foreach (var queryParameter in link.InputItems) {
                        WriteQueryParameter(htmlWriter, queryParameter.Name, queryParameter.DefaultValue);
                    }

                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Type, "submit");
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "btn");
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Value, "Query");
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Input);
                    htmlWriter.RenderEndTag(); //input

                    htmlWriter.RenderEndTag(); //form
                    break;
                }
                case "GET" when !string.IsNullOrEmpty(parameter): {
                    var href1 = link.Href;
                    var href2 = string.Empty;

                    var parameterText = "{" + parameter + "}";

                    if (link.Href.ToUpper().Contains(parameterText.ToUpper())) {
                        var parameterPosition = link.Href.ToUpper().IndexOf(parameterText.ToUpper(), StringComparison.Ordinal);
                        href1 = link.Href.Substring(0, parameterPosition);
                        href2 = link.Href.Substring(parameterPosition + parameterText.Length);
                    } else {
                        if (!href1.EndsWith("/")) {
                            href1 += "/";
                        }
                    }

                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Id, link.Name + "_link");
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Href, href1);
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.A);
                    htmlWriter.Write(href1);
                    htmlWriter.RenderEndTag(); //a

                    htmlWriter.Write("&nbsp;");

                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Id, link.Name + "_input");
                    htmlWriter.AddAttribute("placeholder", parameter);
                    htmlWriter.AddAttribute("size", "5");
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Input);
                    htmlWriter.RenderEndTag(); //input

                    if (!string.IsNullOrEmpty(href2)) {
                        htmlWriter.Write("&nbsp;");
                        htmlWriter.Write(href2);
                    }

                    scripts.Add(UpdateLinkScript
                        .Replace("{link}", link.Name)
                        .Replace("{uri1}", href1)
                        .Replace("{uri2}", href2));
                    break;
                }
                case "GET":
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Href, link.Href);
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.A);
                    htmlWriter.Write(link.Href);
                    htmlWriter.RenderEndTag(); //a
                    break;
                case "POST" or "PUT" or "PATCH": {
                    htmlWriter.AddAttribute("action", link.Href);
                    htmlWriter.AddAttribute("method", "POST");
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Form);

                    foreach (var formField in link.InputItems) {
                        htmlWriter.AddAttribute(HtmlTextWriterAttribute.Name, formField.Name);
                        htmlWriter.AddAttribute("placeholder", formField.Name);

                        if (!string.IsNullOrEmpty(formField.DefaultValue)) {
                            htmlWriter.AddAttribute("value", formField.DefaultValue);
                        }

                        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Input);
                        htmlWriter.RenderEndTag(); //input

                        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Br);
                        htmlWriter.RenderEndTag(); //br
                    }

                    switch (link.Verb) {
                        case "PUT":
                            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Name, "_isPut");
                            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
                            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Input);
                            htmlWriter.RenderEndTag(); //input
                            break;
                        case "PATCH":
                            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Name, "_isPatch");
                            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
                            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Input);
                            htmlWriter.RenderEndTag(); //input
                            break;
                    }

                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Type, "submit");
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "btn");
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Value, link.Verb == "PUT" ? "Put" : "Post");
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Input);
                    htmlWriter.RenderEndTag(); //input

                    htmlWriter.RenderEndTag(); //form
                    break;
                }
                case "DELETE":
                    htmlWriter.AddAttribute("action", link.Href);
                    htmlWriter.AddAttribute("method", "POST");
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Form);

                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Name, "_isDelete");
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Input);
                    htmlWriter.RenderEndTag(); //input

                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Type, "submit");
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "btn");
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Value, "Delete");
                    htmlWriter.AddStyleAttribute(HtmlTextWriterStyle.MarginTop, "1em");
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Input);
                    htmlWriter.RenderEndTag(); //input

                    htmlWriter.RenderEndTag(); //form
                    break;
            }
            
            htmlWriter.RenderEndTag(); //td
            htmlWriter.RenderEndTag(); //tr
        }

        htmlWriter.RenderEndTag(); //table
        htmlWriter.RenderEndTag(); //div
    }

    private static void WriteQueryParameter(HtmlTextWriter htmlWriter, string name, string? defaultValue) {
        htmlWriter.AddAttribute(HtmlTextWriterAttribute.Name, name);
        htmlWriter.AddAttribute("placeholder", name);

        if (!string.IsNullOrEmpty(defaultValue)) {
            htmlWriter.AddAttribute("value", defaultValue);
        }

        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Input);
        htmlWriter.RenderEndTag(); //input

        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Br);
        htmlWriter.RenderEndTag(); //br
    }

    private static void WriteEmbedded(HtmlTextWriter htmlWriter, Resource resource, IList<string> scripts) {
        foreach (var embedded in resource.EmbeddedResources) {
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Br);
            htmlWriter.RenderEndTag(); //br

            htmlWriter.RenderBeginTag(HtmlTextWriterTag.H3);
            htmlWriter.Write(embedded.Key);
            htmlWriter.RenderEndTag(); //h3

            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "embedded");
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);

            switch (embedded.Value) {
                case Resource embeddedResource:
                    WriteResourceAsHtml(htmlWriter, embeddedResource, scripts, false);
                    break;
                case IList<Resource> resourceList: {
                    foreach (var resourceListItem in resourceList) {
                        WriteResourceAsHtml(htmlWriter, resourceListItem, scripts, false);
                    }

                    break;
                }
            }

            htmlWriter.RenderEndTag(); //div
        }
    }

    private static void WriteScripts(HtmlTextWriter htmlWriter, List<string> scripts) {
        htmlWriter.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
        
        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Script);
        
        foreach (var script in scripts) {
            htmlWriter.WriteLine(script);
        }
        
        htmlWriter.RenderEndTag(); //script
    }
}
 