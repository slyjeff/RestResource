using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlySoft.RestResource.Client.Extensions; 

internal static class UrlExtensions {
    public static string AppendQueryParameters(this string url, IDictionary<string, object?>? parameters) {
        if (parameters == null || !parameters.Any()) {
            return url;
        }

        var stringBuilder = new StringBuilder();
        foreach (var item in parameters) {
            if (item.Value == null) {
                continue;
            }

            stringBuilder.Append(stringBuilder.Length == 0 ? '?' : '&');
            stringBuilder.Append(item.Key);
            stringBuilder.Append('=');
            stringBuilder.Append(item.Value);
        }

        return url + stringBuilder;
    }

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

        return $"{baseUrl}/{url}";
    }

    public static string ResolveTemplatedUrl(this string url, IDictionary<string, object?> parameters) {
        var templatedParameters = url.GetTemplatedParameters();
        foreach (var templatedParameter in templatedParameters) {
            var value = parameters.GetValue(templatedParameter);
            if (value != null) {
                url = url.Replace($"{{{templatedParameter}}}", value.ToString());
            }
        }

        return url;
    }

    private static IEnumerable<string> GetTemplatedParameters(this string url) {
        var parameters = new List<string>();
        var parameterStartIndex = url.IndexOf('{');
        while (parameterStartIndex >= 0) {
            var parameterEndIndex = url.IndexOf('}', parameterStartIndex);
            if (parameterEndIndex < 0) {
                return parameters;
            }

            parameters.Add(url.Substring(parameterStartIndex + 1, parameterEndIndex - parameterStartIndex - 1));

            parameterStartIndex = url.IndexOf('{', parameterStartIndex + 1);
        }

        return parameters;
    }
}