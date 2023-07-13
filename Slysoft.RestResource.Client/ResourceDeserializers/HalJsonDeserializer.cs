using Slysoft.RestResource.Client.Extensions;
using System;
using System.Net.Http;
using Slysoft.RestResource.HalJson;

namespace Slysoft.RestResource.Client.ResourceDeserializers;

internal class HalJsonDeserializer : IResourceDeserializer {
    public bool CanDeserialize(HttpResponseMessage response) {
        var contentType = response.GetContentType();
        if (!contentType.StartsWith("application", StringComparison.CurrentCultureIgnoreCase)) {
            return false;
        }

#if NET6_0_OR_GREATER
        return contentType.Contains("json", StringComparison.CurrentCultureIgnoreCase);
#else
        return contentType.ToLower().Contains("json");
#endif
    }

    public Resource Deserialize(HttpResponseMessage response) {
        try {
            var content = response.GetContent();
            return new Resource().FromHalJson(content);
        } catch {
            return new Resource();
        }
    }
}
