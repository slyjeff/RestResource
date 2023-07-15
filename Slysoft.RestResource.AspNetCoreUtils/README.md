# RestResource.AspNetCoreUtils

Utility for returning RestResource objects from controller methods respecting the Accept header of the request

## Getting started

Install from Nuget.Org, and add the following in the start up code:
```
builder.Services.AddControllers(options => {
    options.OutputFormatters.Insert(0, new ResourceHalJsonFormatter());
    options.OutputFormatters.Insert(1, new ResourceHalXmlFormatter());
    options.OutputFormatters.Insert(2, new ResourceHtmlFormatter());
    options.RespectBrowserAcceptHeader = true;
});
```

### Prerequisites

Requires SlySoft.RestResource, SlySoft.RestResource.Hal, and SlySoft.RestResource.Html
SlySoft.RestResource.Client contains utilities for communicating with web services created using RestResource

## Usage

Find examples of how to use RestResource.AspNetCoreUtils at [my blog](https://sly-soft.com/rest-resource-quick-start/).

## Additional documentation

More comprehensive documentation is also at [my blog](https://sly-soft.com/rest-resource/).

## Feedback

Contact me for questions, issuess, or collaboration at <sylvesterjj@gmail.com>