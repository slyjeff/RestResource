using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Slysoft.RestResource.HalXml;

namespace Slysoft.RestResource.AspNetCoreUtils; 

public sealed class ResourceHalXmlFormatter : TextOutputFormatter {
    public ResourceHalXmlFormatter() {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(new StringSegment("application/xml")));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(new StringSegment("application/hal+xml")));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(new StringSegment("application/slysoft.hal+xml")));

        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    protected override bool CanWriteType(Type type) {
        return typeof(Resource).IsAssignableFrom(type) && base.CanWriteType(type);
    }

    public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding) {
        var response = context.HttpContext.Response;

        context.HttpContext.Response.Headers["Content-Type"] = "application/slysoft.hal+xml";

        var resource = context.Object as Resource;
        return response.WriteAsync(resource?.ToHalXml());
    }
}