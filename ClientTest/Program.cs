using System.Net.Mime;
using ClientTest;
using Slysoft.RestResource.Client;

var restClient = new RestClient("http://localhost:35093/");
IApplicationResource? application = null;

Test.Start("Get Application", test => {
    application = restClient.Call<IApplicationResource>(string.Empty);
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

await Test.StartAsync("Get Link", async test => {
    var tests = await application.GetTests();
    const string expectedDescription  = "Tests used by the ClientTest app.";
    test.AssertAreEqual(expectedDescription, tests.Description);
});

