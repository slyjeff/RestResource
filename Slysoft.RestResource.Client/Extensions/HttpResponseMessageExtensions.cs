using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Slysoft.RestResource.HalJson;
using Slysoft.RestResource.HalXml;

namespace Slysoft.RestResource.Client.Extensions; 

internal static class HttpResponseMessageExtensions {
    internal static string GetContent(this HttpResponseMessage response) {
#if NET6_0_OR_GREATER
        using var reader = new StreamReader(response.Content.ReadAsStream());
        return reader.ReadToEnd();
#else
        using var reader = new StreamReader(response.Content.ReadAsStringAsync().Result);
        return reader.ReadToEnd();
#endif
    }

    internal static Resource ToResource(this HttpResponseMessage response) {
        var contentType = response.Content.Headers.GetValues("Content-Type").First();
        var content = response.GetContent();

        if (contentType.IsJson()) {
            return new Resource().FromHalJson(content);
        }

        if (contentType.IsXml()) {
            return new Resource().FromHalXml(content);
        }

        throw new RestCallException($"content type {contentType} not supported- must be application/slysoft.hal+json or application/slysoft.hal+xml");
    }
    
    internal static async Task<Resource> ToResourceAsync(this HttpResponseMessage response) {
        var contentType = response.Content.Headers.GetValues("Content-Type").First();
        var content = await response.Content.ReadAsStringAsync();
        
        if (contentType.IsJson()) {
            return new Resource().FromHalJson(content);
        }

        if (contentType.IsXml()) {
            return new Resource().FromHalXml(content);
        }

        throw new RestCallException($"content type {contentType} not supported- must be application/slysoft.hal+json or application/slysoft.hal+xml");
    }

    private static bool IsJson(this string contentType) {
        if (!contentType.StartsWith("application", StringComparison.CurrentCultureIgnoreCase)) {
            return false;
        }

#if NET6_0_OR_GREATER
        return contentType.Contains("json", StringComparison.CurrentCultureIgnoreCase);
#else
        return contentType.ToLower().Contains("json");
#endif
    }

    private static bool IsXml(this string contentType) {
        if (!contentType.StartsWith("application", StringComparison.CurrentCultureIgnoreCase)) {
            return false;
        }

#if NET6_0_OR_GREATER
        return contentType.Contains("xml", StringComparison.CurrentCultureIgnoreCase);
#else
        return contentType.ToLower().Contains("xml");
#endif
    }
}