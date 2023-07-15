using System.Collections.Specialized;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace SlySoft.RestResource.AspNetCoreUtils;

public static class RegisterTransformUrlEncodedFormsToJsonMiddleware {
    /// <summary>
    /// Converge all UrlEncoded form data to json, so the controllers can consistently be written to expected "body" data and don't have to support multiple forms of payload data
    /// </summary>
    /// <param name="builder">Application builder initialized during app start up</param>
    /// <returns></returns>
    public static IApplicationBuilder TransformUrlEncodedFormsToJson(this IApplicationBuilder builder) {
        return builder.UseMiddleware<TransformUrlEncodedFormsToJsonMiddleware>();
    }
}

internal class TransformUrlEncodedFormsToJsonMiddleware {
    private const string UrlEncodedForm = "application/x-www-form-urlencoded";
    private const string JsonFormat = "application/json";

    private readonly RequestDelegate _next;

    public TransformUrlEncodedFormsToJsonMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task Invoke(HttpContext context) {
        if (!UrlEncodedForm.Equals(context.Request.ContentType, StringComparison.CurrentCultureIgnoreCase)) {
            await _next(context);
            return;
        }
        await Transform(context);
    }

    private async Task Transform(HttpContext context) {
        var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var bodyParameters = HttpUtility.ParseQueryString(requestBody);

        HandleTunneling(context, bodyParameters.AllKeys);

        var json = CreateJsonFromBodyParameters(bodyParameters);
        await WriteJsonToBody(context, json);
    }
    
    private static void HandleTunneling(HttpContext context, string?[] parameters) {
        if (parameters.Contains("_isPut")) {
            context.Request.Method = "PUT";
            return;
        }

        if (parameters.Contains("_isPatch")) {
            context.Request.Method = "PATCH";
            return;
        }

        if (parameters.Contains("_isDelete")) {
            context.Request.Method = "DELETE";
        }
    }

    private static JObject CreateJsonFromBodyParameters(NameValueCollection bodyParameters) {
        var o = new JObject();

        foreach (var key in bodyParameters.AllKeys) {
            if (key is "_isPut" or "_isPatch" or "_isDelete" or null) {
                continue;
            }

            var value = bodyParameters[key];

            if (string.IsNullOrEmpty(value)) {
                continue;
            }

            if (bool.TryParse(value, out var boolValue)) {
                o[key] = boolValue;
                continue;
            }

            o[key] = value;
        }

        return o;
    }

    private async Task WriteJsonToBody(HttpContext context, JObject json) {
        using var newBodyStream = new MemoryStream();
        var request = context.Request;
        request.Body = newBodyStream;

        await using var streamWriter = new StreamWriter(newBodyStream);
        await streamWriter.WriteAsync(json.ToString());
        await streamWriter.FlushAsync();
        newBodyStream.Seek(0, SeekOrigin.Begin);

        request.ContentType = JsonFormat;

        //we have to keep this stream open, so keep going- this will eventually get closed after the request is fully processed
        await _next(context);
    }
}