using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Slysoft.RestResource.Extensions;
using SlySoft.RestResource.HalJson;
using TestUtils;

namespace Slysoft.RestResource.HalJson.Tests; 

[TestClass]
public sealed class DataTests {
    [TestMethod]
    public void UriMustBeAddedAsLinkInJson() {
        //arrange
        var uri = GenerateRandom.String();

        var resource = new Resource()
            .Uri(uri);

        //act
        var json = resource.ToHalJson();

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