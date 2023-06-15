using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resource.Tests.Utils;
using RestResource.Extensions;

namespace RestResource.Tests;

[TestClass]
public sealed class ResourceDataTests {
    [TestMethod]
    public void MustBeAbleToAddDataToResource() {
        //arrange
        var resource = new Resource();
        var message = GenerateRandom.String();

        //act
        resource.Data("message", message);

        //assert
        Assert.AreEqual(message, resource.Data["message"]);
    }

    [TestMethod]
    public void DataNameMustBeCamelCased() {
        //arrange
        var resource = new Resource();
        var message = GenerateRandom.String();

        //act
        resource.Data("Message", message);

        //assert
        Assert.AreEqual(message, resource.Data["message"]);
    }

    [TestMethod]
    public void DataMustBeChainable() {
        //arrange
        var uri = GenerateRandom.String();
        var value1 = GenerateRandom.String();
        var value2 = GenerateRandom.Int();

        //act
        var resource = new Resource()
            .Uri(uri)
            .Data("value1", value1)
            .Data("value2", value2);

        //assert
        Assert.AreEqual(uri, resource.Uri);
        Assert.AreEqual(value1, resource.Data["value1"]);
        Assert.AreEqual(value2, resource.Data["value2"]);
    }
}