using Newtonsoft.Json.Linq;
using RestResource;

namespace HalJson; 

public static class ResourceExtensions {
    public static string ToHalJson(this Resource resource) {
        var o = new JObject();
        foreach (var data in resource.Data) {
            if (data.Value is string stringValue) {
                o[data.Key] = stringValue;
            }
        }

        if (!string.IsNullOrEmpty(resource.Uri)) {
            o.AddLink(new Link("self", resource.Uri));
        }

        return o.ToString();
    }

    private static void AddLink(this JObject o, ILink link) {
        if (!o.ContainsKey("_links")) {
            o["_links"] = new JObject();
        }

        var links = o["_links"];
        if (links == null) {
            return;
        }

        var linkObject = new JObject {
            ["href"] = link.Uri
        };

        links[link.Name] = linkObject;
    }
}