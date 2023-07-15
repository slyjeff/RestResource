using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlySoft.RestResource.Hal;
using TestUtils;

namespace SlySoft.RestResource.HalJson.Tests;

[TestClass]
public sealed class FromHalJsonUriTests {
    [TestMethod]
    public void MustBeAbleToReadUriFromResource() {
        //arrange
        var uri = GenerateRandom.String();
        var resource = new Resource()
            .Uri(uri);

        var json = resource.ToSlySoftHalJson();
        
        //act
        var deserializedResource = new Resource().FromSlySoftHalJson(json);

        //assert
        Assert.AreEqual(uri, deserializedResource.Uri);
    }
}