namespace Slysoft.RestResource.Client.Extensions; 

internal static class UrlExtensions {
    public static string AppendUrl(this string baseUrl, string url) {
#if NET6_0_OR_GREATER
        if (baseUrl.EndsWith('/')) {
            baseUrl = baseUrl[..^1];
        }

        if (url.StartsWith('/')) {
            url = url[1..];
        }
#else
        if (baseUrl.EndsWith("/")) {
            baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
        }

        if (url.StartsWith("/")) {
            url = url.Substring(1);
        }
#endif

        return baseUrl + url;
    }
}