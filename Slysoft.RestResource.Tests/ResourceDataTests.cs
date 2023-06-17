using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Slysoft.RestResource.Extensions;
using TestUtils;

namespace Slysoft.RestResource.Tests;

[TestClass]
public sealed class ResourceDataTests {
    [TestMethod]
    public void MustBeAbleToAddDataToResource() {
        //arrange
        var message = GenerateRandom.String();

        //act
        var resource = new Resource()
            .Data("message", message);

        //assert
        Assert.AreEqual(message, resource.Data["message"]);
    }

    [TestMethod]
    public void DataNameMustBeCamelCased() {
        //arrange
        var message = GenerateRandom.String();

        //act
        var resource = new Resource()
            .Data("Message", message);

        //assert
        Assert.AreEqual(message, resource.Data["message"]);
    }

    [TestMethod]
    public void DataMustConvertSimpleTypesToString() {
        //arrange
        var number = GenerateRandom.Int();

        //act
        var resource = new Resource()
            .Data("Message", number);

        //assert
        Assert.AreEqual(number, resource.Data["message"]);
    }

    [TestMethod]
    public void DataMustAllowFormatting() {
        //arrange
        var dateTime = GenerateRandom.DateTime();

        //act
        var resource = new Resource()
            .Data("date", dateTime, "hh:mm");

        //assert
        Assert.AreEqual(dateTime.ToString("hh:mmm"), resource.Data["date"]?.ToString());
    }

    [TestMethod]
    public void DataMustBeChainable() {
        //arrange
        var value1 = GenerateRandom.String();
        var value2 = GenerateRandom.Int();

        //act
        var resource = new Resource()
            .Data("value1", value1)
            .Data("value2", value2);

        //assert
        Assert.AreEqual(value1, resource.Data["value1"]);
        Assert.AreEqual(value2, resource.Data["value2"]);
    }

    [TestMethod]
    public void MappingAnObjectMustResultInADictionary() {
        //arrange
        var testObject = new TestObject();

        //act
        var resource = new Resource()
            .Data("value", testObject);

        //assert
        var storedObject = resource.Data["value"] as IDictionary<string, object?>;
        Assert.IsNotNull(storedObject);
        Assert.AreEqual(testObject.StringValue, storedObject["stringValue"]);
        Assert.AreEqual(testObject.IntValue, storedObject["intValue"]);
    }

    [TestMethod]
    public void MappingAListOfStringsMustResultInAListOfStrings() {
        //arrange
        var strings = new List<string> {
            "value1",
            "value2",
            "value3"
        };

        //act
        var resource = new Resource()
            .Data("value", strings);

        //assert
        var storedObject = resource.Data["value"] as IList<object?>;
        Assert.IsNotNull(storedObject);
        Assert.AreEqual(strings[0], storedObject[0]);
        Assert.AreEqual(strings[1], storedObject[1]);
        Assert.AreEqual(strings[2], storedObject[2]);
    }

    [TestMethod]
    public void MappingAListOfObjectsMustResultInAListOfDictionaries() {
        //arrange
        var testObjects = new List<TestObject> { new(), new(), new() };

        //act
        var resource = new Resource()
            .Data("value", testObjects);

        //assert
        var storedObject = resource.Data["value"] as IList<IDictionary<string, object?>>;
        Assert.IsNotNull(storedObject);
        Assert.AreEqual(testObjects[0].StringValue, storedObject[0]["stringValue"]);
        Assert.AreEqual(testObjects[0].IntValue, storedObject[0]["intValue"]);
        Assert.AreEqual(testObjects[1].StringValue, storedObject[1]["stringValue"]);
        Assert.AreEqual(testObjects[1].IntValue, storedObject[1]["intValue"]);
        Assert.AreEqual(testObjects[2].StringValue, storedObject[2]["stringValue"]);
        Assert.AreEqual(testObjects[2].IntValue, storedObject[2]["intValue"]);
    }

    //we don't want to map forever without control- go one level deep. If you need to go deeper, you have to use the "map" methods
    [TestMethod]
    public void MappingAListOfListsMustNotMapTheSecondList() {
        //arrange
        var testObjects = new List<TestObjectWithList> { new(), new(), new() };

        //act
        var resource = new Resource()
            .Data("value", testObjects);

        //assert
        var storedObject = resource.Data["value"] as IList<IDictionary<string, object?>>;
        Assert.IsNotNull(storedObject);
        Assert.AreEqual(testObjects[0].StringValue, storedObject[0]["stringValue"]);
        Assert.AreEqual(testObjects[0].IntValue, storedObject[0]["intValue"]);
        Assert.AreEqual(testObjects[1].StringValue, storedObject[1]["stringValue"]);
        Assert.AreEqual(testObjects[1].IntValue, storedObject[1]["intValue"]);
        Assert.AreEqual(testObjects[2].StringValue, storedObject[2]["stringValue"]);
        Assert.AreEqual(testObjects[2].IntValue, storedObject[2]["intValue"]);
        Assert.IsFalse(storedObject[0].ContainsKey("testObjects"));
    }
}