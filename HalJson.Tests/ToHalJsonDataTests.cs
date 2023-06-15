using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestResource;
using RestResource.Extensions;
using TestUtils;

namespace HalJson.Tests; 

[TestClass]
public sealed class ToHalJsonDataTests {
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

    [TestMethod]
    public void ObjectDataMustBeConvertedToJson() {
        //arrange
        var testObject = new TestObject();

        var resource = new Resource()
            .Data("testObject", testObject);

        //act
        var json = resource.ToHalJson();

        //assert
        var expectedJson = $"{{{Environment.NewLine}  \"testObject\": {{{Environment.NewLine}    \"stringValue\": \"{testObject.StringValue}\",{Environment.NewLine}    \"intValue\": \"{testObject.IntValue}\"{Environment.NewLine}  }}{Environment.NewLine}}}";
        Assert.AreEqual(expectedJson, json);
    }

    [TestMethod]
    public void ArrayDataValuesMustBeConvertedToJson() {
        //arrange
        var strings = new List<string> {
            GenerateRandom.String(),
            GenerateRandom.String(),
            GenerateRandom.String()
        };

        var resource = new Resource()
            .Data("strings", strings);

        //act
        var json = resource.ToHalJson();

        //assert
        var expectedJson = $"{{{Environment.NewLine}  \"strings\": [{Environment.NewLine}    \"{strings[0]}\",{Environment.NewLine}    \"{strings[1]}\",{Environment.NewLine}    \"{strings[2]}\"{Environment.NewLine}  ]{Environment.NewLine}}}";
        Assert.AreEqual(expectedJson, json);
    }

    [TestMethod]
    public void ArrayObjectsMustBeConvertedToJson() {
        //arrange
        var dataObjects = new List<TestObject> { new(), new(), new() };

        var resource = new Resource()
            .Data("dataObjects", dataObjects);

        //act
        var json = resource.ToHalJson();

        //assert
        var expectedJson = $"{{{Environment.NewLine}  \"dataObjects\": [{Environment.NewLine}    {{{Environment.NewLine}      \"stringValue\": \"{dataObjects[0].StringValue}\",{Environment.NewLine}      \"intValue\": \"{dataObjects[0].IntValue}\"{Environment.NewLine}    }},{Environment.NewLine}    {{{Environment.NewLine}      \"stringValue\": \"{dataObjects[1].StringValue}\",{Environment.NewLine}      \"intValue\": \"{dataObjects[1].IntValue}\"{Environment.NewLine}    }},{Environment.NewLine}    {{{Environment.NewLine}      \"stringValue\": \"{dataObjects[2].StringValue}\",{Environment.NewLine}      \"intValue\": \"{dataObjects[2].IntValue}\"{Environment.NewLine}    }}{Environment.NewLine}  ]{Environment.NewLine}}}";
        Assert.AreEqual(expectedJson, json);
    }

}