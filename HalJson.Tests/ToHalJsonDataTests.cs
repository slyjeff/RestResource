using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestResource;
using RestResource.Extensions;
using TestUtils;

namespace HalJson.Tests; 

[TestClass]
public class ToHalJsonDataTests {
    [TestMethod]
    public void DataValuesMustBeConvertedToJson() {
        //arrange
        var stringValue = GenerateRandom.String();
        var intValue = GenerateRandom.Int();

        var resource = new Resource()
            .Data("stringValue", stringValue)
            .Data("intValue", intValue);

        //act
        var json = resource.ToHalJson();

        //assert
        var expectedJson = $"{{{Environment.NewLine}  \"stringValue\": \"{stringValue}\",{Environment.NewLine}  \"intValue\": \"{intValue}\"{Environment.NewLine}}}";
        Assert.AreEqual(expectedJson, json);
    }
}