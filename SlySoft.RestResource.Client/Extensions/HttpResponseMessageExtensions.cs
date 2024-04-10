using System.IO;
using System.Linq;
using System.Net.Http;

namespace SlySoft.RestResource.Client.Extensions; 

internal static class HttpResponseMessageExtensions {
    internal static string GetContent(this HttpResponseMessage response) {
        string content;
        try {
#if NET6_0_OR_GREATER
        using var reader = new StreamReader(response.Content.ReadAsStream());
        content = reader.ReadToEnd();
#else
            content = response.Content.ReadAsStringAsync().Result;
#endif
        } catch {
            return $"HTTP Status Code: {response.StatusCode}";
        }

        return string.IsNullOrEmpty(content) 
            ? $"HTTP Status Code: {response.StatusCode}" 
            : content;
    }

    internal static string GetContentType(this HttpResponseMessage response) {
        return response.Content.Headers.GetValues("Content-Type").First();
    }
}