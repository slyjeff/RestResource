using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Slysoft.RestResource.Extensions;
using SlySoft.RestResource.HalJson;
using TestUtils;

namespace Slysoft.RestResource.HalJson.Tests; 

[TestClass]
public class ToHalJsonGetLinkTest {
    [TestMethod]
    public void GetMustBeConvertedToLinkInJson() {
        //arrange
        var href = GenerateRandom.String();
        var resource = new Resource()
            .Get("getLink", href);

        //act
        var json = resource.ToHalJson();

        //assert
        var expectedJson = $"{{{Environment.NewLine}  \"_links\": {{{Environment.NewLine}    \"getLink\": {{{Environment.NewLine}      \"href\": \"{href}\"{Environment.NewLine}    }}{Environment.NewLine}  }}{Environment.NewLine}}}";
        Assert.AreEqual(expectedJson, json);
    }

    [TestMethod]
    public void MultipleGetsMustBeConvertedToLinksInJson() {
        //arrange
        var href1 = GenerateRandom.String();
        var href2 = GenerateRandom.String();
        var resource = new Resource()
            .Get("getLink1", href1)
            .Get("getLink2", href2);


        //act
        var json = resource.ToHalJson();

        //assert
        var expectedJson = $"{{{Environment.NewLine}  \"_links\": {{{Environment.NewLine}    \"getLink1\": {{{Environment.NewLine}      \"href\": \"{href1}\"{Environment.NewLine}    }},{Environment.NewLine}    \"getLink2\": {{{Environment.NewLine}      \"href\": \"{href2}\"{Environment.NewLine}    }}{Environment.NewLine}  }}{Environment.NewLine}}}";
        Assert.AreEqual(expectedJson, json);
    }

    [TestMethod]
    public void GetMustIncludeTemplatedInJson() {
        //arrange
        var href = GenerateRandom.String();
        var resource = new Resource()
            .Get("getLink/{id}", href, templated: true);

        //act
        var json = resource.ToHalJson();

        //assert
        var expectedJson = $"{{{Environment.NewLine}  \"_links\": {{{Environment.NewLine}    \"getLink/{{id}}\": {{{Environment.NewLine}      \"href\": \"{href}\",{Environment.NewLine}      \"templated\": true{Environment.NewLine}    }}{Environment.NewLine}  }}{Environment.NewLine}}}";
        Assert.AreEqual(expectedJson, json);
    }
}