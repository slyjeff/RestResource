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
    public void GetMustBeAbleToReadTemplatingFromResource() {
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
    public void GetMustBeAbleToReadTimeoutFromResource() {
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
}