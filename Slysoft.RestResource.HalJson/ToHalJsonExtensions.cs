using Newtonsoft.Json.Linq;
using Slysoft.RestResource;

namespace SlySoft.RestResource.HalJson; 

public static class ToHalJsonExtensions {
    /// <summary>
    /// Create a resource, formatted as JSON using HAL, with extensions to support expanded links.
    /// MIME type = application/slysoft.hal+json
    /// </summary>
    /// <param name="resource">Resource that contains the data to represent as json</param>
    /// <returns>JSON text in a HAL format (with slysoft extensions)</returns>
    public static string ToHalJson(this Resource resource) {
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

        return o.ToString();
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

        if (link.InputSpecs.Any()) {
            linkObject.AddInputSpecs(link);
        }

        links[link.Name] = linkObject;
    }

    private static void AddInputSpecs(this JObject linkObject, Link link) {
        var inputSpecs = new JObject();
        foreach (var inputSpec in link.InputSpecs) {
            var inputSpecObject = new JObject();

            if (!string.IsNullOrEmpty(inputSpec.Type)) {
                inputSpecObject["type"] = inputSpec.Type;
            }

            if (!string.IsNullOrEmpty(inputSpec.DefaultValue)) {
                inputSpecObject["defaultValue"] = inputSpec.DefaultValue;
            }

            if (inputSpec.ListOfValues.Any()) {
                inputSpecObject["listOfValues"] = new JArray(inputSpec.ListOfValues);
            }

            inputSpecs[inputSpec.Name] = inputSpecObject;
        }

        var inputSpecsName = link.Verb == "GET" ? "parameters" : "fields";
        linkObject[inputSpecsName] = inputSpecs;
    }
}