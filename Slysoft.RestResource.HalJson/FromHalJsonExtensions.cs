using System.Collections;
using Newtonsoft.Json.Linq;

namespace Slysoft.RestResource.HalJson; 

public static class FromHalJsonExtensions {
    /// <summary>
    /// Populate a resource from a JSON string in slysoft.hal+json or hal+json format
    /// </summary>
    /// <param name="resource">The resource to populate</param>
    /// <param name="json">JSON in  slysoft.hal+json format or hal+json format</param>
    /// <returns>The resource for ease of reference</returns>
    public static Resource FromHalJson(this Resource resource, string json) {
        var o = JObject.Parse(json);
        
        resource.GetUri(o);

        resource.GetData(o);

        resource.GetLinks(o);

        return resource;
    }

    private static void GetUri(this Resource resource, JObject o) {
        var links = o["_links"];
        if (links == null) {
            return;
        }

        if (links["self"] is not JObject self) {
            return;
        }

        var href = self["href"];
        if (href == null) {
            return;
        }

        resource.Uri = href.ToString();
    }

    private static void GetData(this Resource resource, JObject o) {
        resource.Data.PopulateFromJObject(o);
    }

    private static void PopulateFromJObject(this IDictionary<string, object?> dictionary, JObject o) {
        foreach (var item in o) {
            if (item.Key is "_links" or "_embedded") {
                continue;
            }

            switch (item.Value) {
                case JValue value:
                    dictionary[item.Key] = value.Value;
                    break;
                case JArray jArray:
                    dictionary[item.Key] = jArray.ToList();
                    break;
                case JObject jObject: {
                    var childDictionary = new Dictionary<string, object?>();
                    childDictionary.PopulateFromJObject(jObject);
                    dictionary[item.Key] = childDictionary;
                    break;
                }
            }
        }
    }

    private static IEnumerable ToList(this JArray jArray) {
        var firstValue = jArray.FirstOrDefault();
        if (firstValue == default) {
            return new List<object>();
        }

        if (firstValue is JValue) {
            return jArray.OfType<JValue>().Select(x => x.Value).ToList();
        }

        var list = new List<IDictionary<string, object?>>();
        foreach (var item in jArray.OfType<JObject>()) {
            var dictionary = new Dictionary<string, object?>();
            dictionary.PopulateFromJObject(item);
            list.Add(dictionary);
        }

        return list;
    }

    private static void GetLinks(this Resource resource, JObject o) {
        if (o["_links"] is not JObject links) {
            return;
        }

        foreach (var linkObject in links) {
            if (linkObject.Key == "self") {
                continue;
            }

            if (linkObject.Value is not JObject linkData) {
                continue;
            }

            if (linkData["href"] is not JValue href) {
                continue;
            }

            var verb = "GET";
            if (linkData["verb"] is JValue verbValue) {
                verb = verbValue.ToString();
            }

            var templated = linkData["templated"] != null;

            var timeout = 0;
            if (linkData["timeout"] is JValue timeoutValue) {
                timeout =  Convert.ToInt32(timeoutValue.Value);
            }

            var link = new Link(linkObject.Key, href.ToString(),  verb: verb, templated: templated, timeout: timeout);

            var inputItems = linkData["parameters"] as JObject;
            if (inputItems == null) {
                inputItems = linkData["fields"] as JObject;
            }


            if (inputItems != null) {
                foreach (var inputItemKeyValue in inputItems) {
                    var inputItem = new InputItem(inputItemKeyValue.Key);
                    link.InputItems.Add(inputItem);

                    if (inputItemKeyValue.Value is not JObject inputItemValue) {
                        continue;
                    }

                    if (inputItemValue["defaultValue"] is JValue defaultValue) {
                        inputItem.DefaultValue = defaultValue.ToString();
                    }

                    if (inputItemValue["type"] is JValue typeValue) {
                        inputItem.Type = typeValue.ToString();
                    }

                    if (inputItemValue["listOfValues"] is JArray listOfValues) {
                        foreach (var value in listOfValues) {
                            inputItem.ListOfValues.Add(value.ToString());
                        }
                    }
                }
            }

            resource.Links.Add(link);
        }
    }
}