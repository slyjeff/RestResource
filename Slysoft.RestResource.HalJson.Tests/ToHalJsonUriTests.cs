using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SlySoft.RestResource.Hal;
using TestUtils;

namespace SlySoft.RestResource.HalJson.Tests;

[TestClass]
public sealed class ToHalJsonUriTests {
    [TestMethod]
    public void UriMustBeAddedAsLinkInJson() {
        //arrange
        var uri = GenerateRandom.String();

        var resource = new Resource()
            .Uri(uri);

        //act
        var json = resource.ToSlySoftHalJson();

        //assert
        var expected = new {
            _links = new {
                self = new  {
                    href = uri
                }
            }
        };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }
}