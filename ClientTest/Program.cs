using System.Net;
using ClientTest;
using Slysoft.RestResource.Client;
using TestUtils;

var restClient = new RestClient("http://localhost:35093/");
IApplicationResource? application = null;

Test.Start("Get Application", test => {
    application = restClient.Call<IApplicationResource>();
    const string expectedInformation = "This is a test web service for demonstrating how to use Slysoft.RestResource and related libraries.";
    test.AssertAreEqual(expectedInformation, application.Information);
});

await Test.StartAsync("Get Application Async", async test => {
    restClient = new RestClient("http://localhost:35093/");
    application = await restClient.CallAsync<IApplicationResource>(string.Empty);
    const string expectedInformation = "This is a test web service for demonstrating how to use Slysoft.RestResource and related libraries.";
    test.AssertAreEqual(expectedInformation, application.Information);
});

if (application == null) {
    Environment.Exit(0);
}

ITestsResource? tests = null;
await Test.StartAsync("Get Link", async test => {
    tests = await application.GetTests();
    const string expectedDescription  = "Tests used by the ClientTest app.";
    test.AssertAreEqual(expectedDescription, tests.Description);
});

if (tests == null) {
    Environment.Exit(0);
}

await Test.StartAsync("Response Error Code Exception", async test => {
    try {
        await tests.NotFound();
        test.Error("ResponseErrorCodeException exception not thrown");
    }
    catch (ResponseErrorCodeException e) {
        test.AssertAreEqual("Resource not found.", e.Message);
        test.AssertAreEqual(HttpStatusCode.NotFound, e.StatusCode);
    }
});

await Test.StartAsync("Get Non-Resource Text", async test => {
    var text = await tests.Text();
    test.AssertAreEqual("Non-Resource text.", text);
});

await Test.StartAsync("Query Parameters", async test => {
    var parameter1 = GenerateRandom.String();
    var parameter2 = GenerateRandom.String();
    var queryResult = await tests.Query(parameter1, parameter2);
    test.AssertAreEqual(parameter1, queryResult.Parameter1);
    test.AssertAreEqual(parameter2, queryResult.Parameter2);
});
