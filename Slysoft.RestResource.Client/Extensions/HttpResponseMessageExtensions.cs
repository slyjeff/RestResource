using System.IO;
using System.Linq;
using System.Net.Http;

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

    internal static string GetContentType(this HttpResponseMessage response) {
        return response.Content.Headers.GetValues("Content-Type").First();
    }
}