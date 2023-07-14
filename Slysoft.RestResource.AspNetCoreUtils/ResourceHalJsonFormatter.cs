using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Slysoft.RestResource.HalJson;

namespace Slysoft.RestResource.AspNetCoreUtils; 

public sealed class ResourceHalJsonFormatter : TextOutputFormatter {
    public ResourceHalJsonFormatter() {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(new StringSegment("application/json")));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(new StringSegment("application/hal+json")));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(new StringSegment("application/slysoft.hal+json")));

        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    protected override bool CanWriteType(Type type) {
        return typeof(Resource).IsAssignableFrom(type) && base.CanWriteType(type);
    }

    public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding) {
        var response = context.HttpContext.Response;

        context.HttpContext.Response.Headers["Content-Type"] ="application/slysoft.hal+json";

        var resource = context.Object as Resource;
        return response.WriteAsync(resource?.ToHalJson());
    }
}