using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Slysoft.RestResource.Extensions;
using TestUtils;
#pragma warning disable IDE0037

// ReSharper disable RedundantAnonymousTypePropertyName

namespace Slysoft.RestResource.HalJson.Tests; 

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
        var expected = new {
            stringValue = stringValue,
            intValue = intValue
        };

        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }

    [TestMethod]
    public void FormattedNumberValuesMustBeConvertedToJsonWithoutQuotes() {
        //arrange
        var floatValue = GenerateRandom.Float();

        var resource = new Resource()
            .Data("formatted", floatValue, "#,0.000");

        //act
        var json = resource.ToHalJson();

        //assert
        //we can't use an anonymous object for this because it adds quotes when we format the number
        var expectedJson = $"{{{Environment.NewLine}  \"formatted\": {floatValue:#,0.000}{Environment.NewLine}}}";
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
        var expected = new {
            testObject = new { 
                stringValue = testObject.StringValue,
                intValue = testObject.IntValue,
            }
        }; 
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
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
        var expected = new {
            strings = new[] { strings[0], strings[1], strings[2] }
        };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);

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
        var expected = new {
            dataObjects = new [] {
                new { stringValue = dataObjects[0].StringValue, intValue = dataObjects[0].IntValue },
                new { stringValue = dataObjects[1].StringValue, intValue = dataObjects[1].IntValue },
                new { stringValue = dataObjects[2].StringValue, intValue = dataObjects[2].IntValue },
            }
        };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }
}