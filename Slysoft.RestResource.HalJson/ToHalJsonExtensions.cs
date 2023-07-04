using Newtonsoft.Json.Linq;
using Slysoft.RestResource;
using Slysoft.RestResource.Extensions;

namespace SlySoft.RestResource.HalJson; 

public static class ToHalJsonExtensions {
    /// <summary>
    /// Create a resource, formatted as JSON using HAL, with extensions to support expanded links.
    /// MIME type = application/slysoft.hal+json
    /// </summary>
    /// <param name="resource">Resource that contains the data to represent as json</param>
    /// <returns>JSON text in a HAL format (with slysoft extensions)</returns>
    public static string ToHalJson(this Resource resource) {
        return resource.CreateJObject().ToString();
    }

    private static JObject CreateJObject(this Resource resource) {
        var o = new JObject();

        foreach (var data in resource.Data) {
            o.AddData(data);
        }

        if (!string.IsNullOrEmpty(resource.Uri)) {
            o.AddLink(new Link("self", resource.Uri));
        }

        foreach (var link in resource.Links) {
            o.AddLink(link);
        }

        foreach (var embeddedResource in resource.EmbeddedResources) {
            o.AddEmbeddedResource(embeddedResource.Key, embeddedResource.Value);
        }

        return o;
    }

    private static void AddData(this JObject o, KeyValuePair<string, object?> data) {
        if (data.Value is string stringValue) {
            o[data.Key] = stringValue;
            return;
        }

        if (data.Value is FormattedValue formattedValue) {
            o[data.Key] = new JRaw(formattedValue.Value);
            return;
        }

        if (data.Value is IList<object?> listOfObjects) {
            var array = new JArray();
            foreach (var item in listOfObjects) {
                array.Add(item);
            }
            o[data.Key] = array;
            return;
        }

        if (data.Value is IList<IDictionary<string, object?>> listOfDictionary) {
            var array = new JArray();
            foreach (var dictionary in listOfDictionary) {
                array.Add(dictionary.ToJson());
            }
            o[data.Key] = array;
            return;
        }

        if (data.Value is IDictionary<string, object?> dictionaryObject) {
            o[data.Key] = dictionaryObject.ToJson();
            return;
        }

        o[data.Key] = new JValue(data.Value);
    }

    private static JObject ToJson(this IDictionary<string, object?> dictionary) {
        var jObject = new JObject();
        foreach (var item in dictionary) {
            jObject.AddData(item);
        }

        return jObject;
    }

    private static void AddLink(this JObject o, Link link) {
        if (!o.ContainsKey("_links")) {
            o["_links"] = new JObject();
        }

        var links = o["_links"];
        if (links == null) {
            return;
        }

        var linkObject = new JObject {
            ["href"] = link.Href
        };

        if (link.Templated) {
            linkObject["templated"] = true;
        }

        if (link.Verb != "GET") {
            linkObject["verb"] = link.Verb;
        }

        if (link.Timeout != 0) {
            linkObject["timeout"] = link.Timeout;
        }

        if (link.Parameters.Any()) {
            linkObject.AddLinkParameters(link);
        }

        links[link.Name] = linkObject;
    }

    private static void AddLinkParameters(this JObject linkObject, Link link) {
        var linkParameters = new JObject();
        foreach (var linkParameter in link.Parameters) {
            var linkParameterObject = new JObject();

            if (!string.IsNullOrEmpty(linkParameter.Type)) {
                linkParameterObject["type"] = linkParameter.Type;
            }

            if (!string.IsNullOrEmpty(linkParameter.DefaultValue)) {
                linkParameterObject["defaultValue"] = linkParameter.DefaultValue;
            }

            if (linkParameter.ListOfValues.Any()) {
                linkParameterObject["listOfValues"] = new JArray(linkParameter.ListOfValues);
            }

            linkParameters[linkParameter.Name] = linkParameterObject;
        }

        var linkParameterName = link.GetParameterTypeName() + "s";
        linkObject[linkParameterName] = linkParameters;
    }

    private static void AddEmbeddedResource(this JObject o, string name, object resourceObject) {
        if (!o.ContainsKey("_embedded")) {
            o["_embedded"] = new JObject();
        }

        var embedded = o["_embedded"];
        if (embedded == null) {
            return;
        }

        switch (resourceObject) {
            case Resource resource: {
                embedded[name] = resource.CreateJObject();
                return;
            }

            case IList<Resource> resourceList: {
                var jArray = new JArray();
                foreach (var resource in resourceList) {
                    jArray.Add(resource.CreateJObject());
                }
                embedded[name] = jArray;
                break;
            }
        }
    }
}