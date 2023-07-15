# RestResource

Utility for creating representing resources containing data, links, and embedded resources. A tool for creating HATEOAS web services.

## Getting started

Install from Nuget.Org, and create a Resource() class. Use the extension methods .Data(), .Embedded() .Get(), .Query(), Post(), Put(), Patch(), and Delete() to add data and links to the resource.

### Prerequisites

No prerequisites are required; however, to get maximum benifits use SlySoft.RestResource.Hal, and SlySoft.RestResource.Html to serialize and deserialize resources.
SlySoft.RestResource.AspNetCoreUtils contains utilities for returning Resources from controller methods, respected the Accept header for serialization
SlySoft.RestResource.Client contains utilities for communicating with web services created using RestResource

## Usage

Example code (from an ASP.NET CORE MVC Controller):

```
    [HttpGet]
    public IActionResult Get() {
        var resource = new Resource()
            .Data("message", "Hello World!")
            .Get("getUser", "/user")
            .Post("addUser", "/user")
                .Field("userName")
                .Field("lastName")
                .Field("firstName")
                .Field("email")
            .EndBody();

        return StatusCode(200, resource);
    }
```

Find more examples of how to use RestResource at [my blog](https://sly-soft.com/rest-resource-quick-start/).

## Additional documentation

More comprehensive documentation is also at [my blog](https://sly-soft.com/rest-resource/).

## Feedback

Contact me for questions, issuess, or collaboration at <sylvesterjj@gmail.com>