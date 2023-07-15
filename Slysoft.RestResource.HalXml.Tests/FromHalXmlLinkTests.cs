using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlySoft.RestResource.Hal;

namespace SlySoft.RestResource.HalXml.Tests;

[TestClass]
public sealed class FromHalXmlLinkTests {
    [TestMethod]
    public void MustBeAbleToReadGetLinkFromResource() {
        //arrange
        const string uri = "/api/user";

        var resource = new Resource()
            .Get("GetUsers", uri);

        var xml = resource.ToSlySoftHalXml();

        //act
        var deserializedResource = new Resource().FromSlySoftHalXml(xml);

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

        var xml = resource.ToSlySoftHalXml();

        //act
        var deserializedResource = new Resource().FromSlySoftHalXml(xml);

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

        var xml = resource.ToSlySoftHalXml();

        //act
        var deserializedResource = new Resource().FromSlySoftHalXml(xml);

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

        var xml = resource.ToSlySoftHalXml();

        //act
        var deserializedResource = new Resource().FromSlySoftHalXml(xml);

        //assert
        var link = deserializedResource.GetLink("search");
        Assert.IsNotNull(link);
        Assert.IsNotNull(link.GetParameter("lastName"));
        Assert.IsNotNull(link.GetParameter("firstName"));
    }

    [TestMethod]
    public void MustBeAbleReadDefaultValueFromResource() {
        //arrange
        var resource = new Resource()
            .Query("search", "/api/user")
                .Parameter("position", defaultValue: "admin")
            .EndQuery();

        var xml = resource.ToSlySoftHalXml();

        //act
        var deserializedResource = new Resource().FromSlySoftHalXml(xml);

        //assert
        var link = deserializedResource.GetLink("search");
        Assert.IsNotNull(link);
        var queryParameter = link.GetParameter("position");
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

        var xml = resource.ToSlySoftHalXml();

        //act
        var deserializedResource = new Resource().FromSlySoftHalXml(xml);

        //assert
        var link = deserializedResource.GetLink("search");
        Assert.IsNotNull(link);
        var queryParameter = link.GetParameter("position");
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

        var xml = resource.ToSlySoftHalXml();

        //act
        var deserializedResource = new Resource().FromSlySoftHalXml(xml);

        //assert
        var link = deserializedResource.GetLink("search");
        Assert.IsNotNull(link);
        var queryParameter = link.GetParameter("yearsEmployed");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("number", queryParameter.Type);
    }

    [TestMethod]
    public void MustBeAbleToReadNonGetLinkFromResource() {
        //arrange
        const string uri = "/api/user";

        var resource = new Resource()
            .Post("CreateUser", uri)
            .EndBody();

        var xml = resource.ToSlySoftHalXml();

        //act
        var deserializedResource = new Resource().FromSlySoftHalXml(xml);

        //assert
        var link = deserializedResource.GetLink("createUser");
        Assert.IsNotNull(link);
        Assert.AreEqual("createUser", link.Name);
        Assert.AreEqual(uri, link.Href);
        Assert.IsFalse(link.Templated);
        Assert.AreEqual("POST", link.Verb);
    }


    [TestMethod]
    public void MustBeAbleReadFieldsFromResource() {
        //arrange
        var resource = new Resource()
            .Post("createUser", "/api/user")
                .Field("lastName")
                .Field("firstName")
            .EndBody();

        var xml = resource.ToSlySoftHalXml();

        //act
        var deserializedResource = new Resource().FromSlySoftHalXml(xml);

        //assert
        var link = deserializedResource.GetLink("createUser");
        Assert.IsNotNull(link);
        Assert.IsNotNull(link.GetParameter("lastName"));
        Assert.IsNotNull(link.GetParameter("firstName"));
    }
}