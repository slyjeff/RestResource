using Microsoft.VisualStudio.TestTools.UnitTesting;
using Slysoft.RestResource.Extensions;
using TestUtils;

namespace Slysoft.RestResource.Tests;

[TestClass]
public sealed class UriTests {
    [TestMethod]
    public void MustBeABleToSpecifyResourceUri() {
        //arrange
        var resource = new Resource();
        var uri = GenerateRandom.String();

        //act
        resource.Uri(uri);

        //assert
        Assert.AreEqual(uri, resource.Uri);
    }

    [TestMethod]
    public void UriMustBeChainable() {
        //arrange
        var uri = GenerateRandom.String();
        var message = GenerateRandom.String();
        
        //act
        var resource = new Resource()
            .Uri(uri)
            .Data("message", message);

        //assert
        Assert.AreEqual(uri, resource.Uri);
        Assert.AreEqual(message, resource.Data["message"]);
    }
}