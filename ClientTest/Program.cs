using ClientTest;
using Slysoft.RestResource.Client;
using System.Runtime.Intrinsics.X86;

IRestClient? restClient;

Test.Start("GetApplication", (test) => {
    restClient = new RestClient("http://localhost:35093/");
    var application = restClient.Call<IApplicationResource>(string.Empty);
    var expectedInformation = "This is a test web service for demonstrating how to use Slysoft.RestResource and related libraries.";
    test.AssertAreEqual(expectedInformation, application.Information);
});

