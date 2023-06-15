using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Slysoft.RestResource.Extensions;
using SlySoft.RestResource.HalJson;
using TestUtils;

namespace Slysoft.RestResource.HalJson.Tests; 

[TestClass]
public sealed class DataTests {
    [TestMethod]
    public void DataValuesMustBeConvertedToJson() {
        //arrange
        var uri = GenerateRandom.String();

        var resource = new Resource()
            .Uri(uri);

        //act
        var json = resource.ToHalJson();

        //assert
        var expectedJson = $"{{{Environment.NewLine}  \"_links\": {{{Environment.NewLine}    \"self\": {{{Environment.NewLine}      \"href\": \"{uri}\"{Environment.NewLine}    }}{Environment.NewLine}  }}{Environment.NewLine}}}";
        Assert.AreEqual(expectedJson, json);
    }
}