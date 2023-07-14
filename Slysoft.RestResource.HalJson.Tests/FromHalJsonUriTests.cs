using Microsoft.VisualStudio.TestTools.UnitTesting;
using Slysoft.RestResource.Extensions;
using TestUtils;

namespace Slysoft.RestResource.HalJson.Tests;

[TestClass]
public sealed class FromHalJsonUriTests {
    [TestMethod]
    public void MustBeAbleToReadUriFromResource() {
        //arrange
        var uri = GenerateRandom.String();
        var resource = new Resource()
            .Uri(uri);

        var json = resource.ToHalJson();
        
        //act
        var deserializedResource = new Resource().FromHalJson(json);

        //assert
        Assert.AreEqual(uri, deserializedResource.Uri);
    }
}