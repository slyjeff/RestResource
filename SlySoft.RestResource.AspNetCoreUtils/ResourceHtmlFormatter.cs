using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using SlySoft.RestResource.Html;

namespace SlySoft.RestResource.AspNetCoreUtils; 

public sealed class ResourceHtmlFormatter : TextOutputFormatter {
    public ResourceHtmlFormatter() {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(new StringSegment("text/html")));

        SupportedEncodings.Add(Encoding.ASCII);
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    protected override bool CanWriteType(Type type) {
        return typeof(Resource).IsAssignableFrom(type) && base.CanWriteType(type);
    }

    public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding) {
        var response = context.HttpContext.Response;

        var resource = context.Object as Resource;
        return response.WriteAsync(resource?.ToHtml());
    }
}