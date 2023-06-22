using Microsoft.VisualStudio.TestTools.UnitTesting;
using Slysoft.RestResource.Extensions;
using SlySoft.RestResource.HalJson;

namespace Slysoft.RestResource.HalJson.Tests;

[TestClass]
public sealed class FromHalJsonLinkTests {
    [TestMethod]
    public void MustBeAbleToReadGetLinkFromResource() {
        //arrange
        const string uri = "/api/user";

        var resource = new Resource()
            .Get("GetUsers", uri);

        var json = resource.ToHalJson();

        //act
        var deserializedResource = new Resource().FromHalJson(json);

        //assert
        var link = deserializedResource.GetLink("getUsers");
        Assert.IsNotNull(link);
        Assert.AreEqual("getUsers", link.Name);
        Assert.AreEqual(uri, link.Href);
        Assert.IsFalse(link.Templated);
        Assert.AreEqual("GET", link.Verb);
    }

    [TestMethod]
    public void MustBeAbleToReadTemplatingFromResource() {
        //arrange
        var resource = new Resource()
            .Get("getUser", "/api/user/{id}", templated: true);

        var json = resource.ToHalJson();

        //act
        var deserializedResource = new Resource().FromHalJson(json);

        //assert
        var link = deserializedResource.GetLink("getUser");
        Assert.IsNotNull(link);
        Assert.IsTrue(link.Templated);
    }

    [TestMethod]
    public void MustBeAbleToReadTimeoutFromResource() {
        //arrange
        var resource = new Resource()
            .Get("getUser", "/api/user/{id}", timeout: 60);

        var json = resource.ToHalJson();

        //act
        var deserializedResource = new Resource().FromHalJson(json);

        //assert
        var link = deserializedResource.GetLink("getUser");
        Assert.IsNotNull(link);
        Assert.AreEqual(link.Timeout, 60);
    }

    [TestMethod]
    public void MustBeAbleReadQueryParametersFromResource() {
        //arrange
        var resource = new Resource()
            .Query("search", "/api/user")
                .Parameter("lastName")
                .Parameter("firstName")
            .EndQuery();

        var json = resource.ToHalJson();

        //act
        var deserializedResource = new Resource().FromHalJson(json);

        //assert
        var link = deserializedResource.GetLink("search");
        Assert.IsNotNull(link);
        Assert.IsNotNull(link.GetInputItem("lastName"));
        Assert.IsNotNull(link.GetInputItem("firstName"));
    }

    [TestMethod]
    public void MustBeAbleReadDefaultValueFromResource() {
        //arrange
        var resource = new Resource()
            .Query("search", "/api/user")
                .Parameter("position", defaultValue: "admin")
            .EndQuery();

        var json = resource.ToHalJson();

        //act
        var deserializedResource = new Resource().FromHalJson(json);

        //assert
        var link = deserializedResource.GetLink("search");
        Assert.IsNotNull(link);
        var queryParameter = link.GetInputItem("position");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("admin", queryParameter.DefaultValue);
    }

    [TestMethod]
    public void MustBeAbleReadListOfValuesFromResource() {
        //arrange
        var resource = new Resource()
            .Query("search", "/api/user")
                .Parameter("position", listOfValues: new[] { "Standard", "Admin" })
            .EndQuery();

        var json = resource.ToHalJson();

        //act
        var deserializedResource = new Resource().FromHalJson(json);

        //assert
        var link = deserializedResource.GetLink("search");
        Assert.IsNotNull(link);
        var queryParameter = link.GetInputItem("position");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("Standard", queryParameter.ListOfValues[0]);
        Assert.AreEqual("Admin", queryParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void MustBeAbleReadTypeFromResource() {
        //arrange
        var resource = new Resource()
            .Query("search", "/api/user")
                .Parameter("yearsEmployed", type: "number")
            .EndQuery();

        var json = resource.ToHalJson();

        //act
        var deserializedResource = new Resource().FromHalJson(json);

        //assert
        var link = deserializedResource.GetLink("search");
        Assert.IsNotNull(link);
        var queryParameter = link.GetInputItem("yearsEmployed");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("number", queryParameter.Type);
    }
}