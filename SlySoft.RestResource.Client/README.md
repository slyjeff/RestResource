# RestResource.Client

Utility for consuming web services returning data serialized using RestResource.

## Getting started

Install from Nuget.Org.

### Prerequisites

Requires SlySoft.RestResource and SlySoft.RestResource.Hal

## Usage

```
var restClient = new RestClient("http://localhost:35093/");
var application = await restClient.CallAsync<IApplicationResource>();

Console.WriteLine(application.Information);

var tests = await application.GetTests();

Console.WriteLine(tests.Description);
```

Find more examples of how to use RestResource.Client at [my blog](https://sly-soft.com/rest-resource-quick-start/).

## Additional documentation

More comprehensive documentation is also at [my blog](https://sly-soft.com/rest-resource/).

## Feedback

Contact me for questions, issuess, or collaboration at <sylvesterjj@gmail.com>