using System;
using System.Net.Http;
using SlySoft.RestResource.Client.Extensions;
using SlySoft.RestResource.HalXml;

namespace SlySoft.RestResource.Client.ResourceDeserializers;

internal class XmlDeserializer : IResourceDeserializer {
    public bool CanDeserialize(HttpResponseMessage response) {
        var contentType = response.GetContentType();
        if (!contentType.StartsWith("application", StringComparison.CurrentCultureIgnoreCase)) {
            return false;
        }

#if NET6_0_OR_GREATER
        return contentType.Contains("xml", StringComparison.CurrentCultureIgnoreCase);
#else
        return contentType.ToLower().Contains("xml");
#endif
    }

    public Resource Deserialize(HttpResponseMessage response) {
        try {
            var content = response.GetContent();
            return new Resource().FromHalXml(content);
        } catch {
            return new Resource();
        }
    }
}
