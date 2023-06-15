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

        return o.ToString();
    }
}