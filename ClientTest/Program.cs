using System.Net;
using ClientTest;
using Slysoft.RestResource;
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
    } catch (ResponseErrorCodeException e) {
        test.AssertAreEqual("Resource not found.", e.Message);
        test.AssertAreEqual(HttpStatusCode.NotFound, e.StatusCode);
    }
});

await Test.StartAsync("Get Non-Resource Text", async test => {
    var text = await tests.Text();
    test.AssertAreEqual("Non-Resource text.", text);
});

await Test.StartAsync("Query Parameters", async test => {
    //arrange
    var parameter1 = GenerateRandom.String();
    var parameter2 = GenerateRandom.String();

    //act
    var queryResult = await tests.Query(parameter1, parameter2);

    //assert
    test.AssertAreEqual(parameter1, queryResult.Parameter1);
    test.AssertAreEqual(parameter2, queryResult.Parameter2);
});

await Test.StartAsync("Post", async test => {
    //arrange
    var parameter1 = GenerateRandom.String();
    var parameter2 = GenerateRandom.String();
    
    //act
    var postResult = await tests.Post(parameter1, parameter2);
    
    //assert
    test.AssertAreEqual(parameter1, postResult.Parameter1);
    test.AssertAreEqual(parameter2, postResult.Parameter2);
});

await Test.StartAsync("List", async test => {
    //arrange
    var list = new List<string> { GenerateRandom.String(), GenerateRandom.String(), GenerateRandom.String( )};

    //act
    var listResult = await tests.List(list);

    //assert
    test.AssertAreEqual(list[0], listResult.List[0]);
    test.AssertAreEqual(list[1], listResult.List[1]);
    test.AssertAreEqual(list[2], listResult.List[2]);
});

await Test.StartAsync("Manually Call Post", async test => {
    //arrange
    var postLink = tests.Resource.Links.FirstOrDefault(link => link.Verb == "POST");
    if (postLink == null) {
        throw new Exception("No POST link found");
    }

    var fields = postLink.Parameters.ToDictionary<LinkParameter?, string, object?>(parameter => parameter.Name, _ => GenerateRandom.String());

    //act
    var postResult = await tests.CallRestLinkAsync<Resource>(postLink.Name, fields);

    //assert
    foreach (var field in fields) {
        test.AssertAreEqual(field.Value, postResult.Data[field.Key]);
    }
});
