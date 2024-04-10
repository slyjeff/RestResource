using System;
using Newtonsoft.Json;

namespace SlySoft.RestResource.HalJson.Tests.Utils;

internal static class JsonStringExtensions {
    public static string WithPlaceholder(this string json, string placeholder, string value) {
        json = json.Replace($"\"{{{placeholder}}}\"", value);
        json = json.Replace(" ", "").Replace(Environment.NewLine, "");
        return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json), Formatting.Indented);
    }
}