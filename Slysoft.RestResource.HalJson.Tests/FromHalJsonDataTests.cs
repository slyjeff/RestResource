using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlySoft.RestResource.Hal;
using TestUtils;

namespace SlySoft.RestResource.HalJson.Tests;

[TestClass]
public sealed class FromHalJsonDataTests {
    [TestMethod]
    public void MustBeAbleToReadDataFromResource() {
        //arrange
        var message = GenerateRandom.String();
        var resource = new Resource()
            .Data("message", message);

        var json = resource.ToSlySoftHalJson();

        //act
        var deserializedResource = new Resource().FromSlySoftHalJson(json);

        //assert
        Assert.AreEqual(message, deserializedResource.Data["message"]);
    }

    [TestMethod]
    public void MustBeAbleToReadDictionaryDataFromResource() {
        //arrange
        var testObject = new TestObject();
        var resource = new Resource()
            .Data("value", testObject);

        var json = resource.ToSlySoftHalJson();

        //act
        var deserializedResource = new Resource().FromSlySoftHalJson(json);

        //assert
        var storedObject = deserializedResource.Data["value"] as ObjectData;
        Assert.IsNotNull(storedObject);
        Assert.AreEqual(testObject.StringValue, storedObject["stringValue"]);
        Assert.AreEqual(testObject.IntValue, Convert.ToInt32(storedObject["intValue"]));
    }

    [TestMethod]
    public void MustBeAbleToReadAListOfStringsFromResource() {
        //arrange
        var strings = new List<string> {
            "value1",
            "value2",
            "value3"
        };

        var resource = new Resource()
            .Data("value", strings);

        var json = resource.ToSlySoftHalJson();

        //act
        var deserializedResource = new Resource().FromSlySoftHalJson(json);

        //assert
        var storedObject = deserializedResource.Data["value"] as IList<object?>;
        Assert.IsNotNull(storedObject);
        Assert.AreEqual(strings[0], storedObject[0]);
        Assert.AreEqual(strings[1], storedObject[1]);
        Assert.AreEqual(strings[2], storedObject[2]);
    }

    [TestMethod]
    public void MustBeAbleToReadAListOfObjectsFromResource() {
        //arrange
        var testObjects = new List<TestObject> { new(), new(), new() };

        var resource = new Resource()
            .Data("value", testObjects);

        var json = resource.ToSlySoftHalJson();

        //act
        var deserializedResource = new Resource().FromSlySoftHalJson(json);

        //assert
        var storedObject = deserializedResource.Data["value"] as ListData;
        Assert.IsNotNull(storedObject);
        Assert.AreEqual(testObjects[0].StringValue, storedObject[0]["stringValue"]);
        Assert.AreEqual(testObjects[0].IntValue, Convert.ToInt32(storedObject[0]["intValue"]));
        Assert.AreEqual(testObjects[1].StringValue, storedObject[1]["stringValue"]);
        Assert.AreEqual(testObjects[1].IntValue, Convert.ToInt32(storedObject[1]["intValue"]));
        Assert.AreEqual(testObjects[2].StringValue, storedObject[2]["stringValue"]);
        Assert.AreEqual(testObjects[2].IntValue, Convert.ToInt32(storedObject[2]["intValue"]));
    }

    [TestMethod]
    public void MustBeAbleToReadListsOfListsFromResource() {
        //arrange
        var testObjects = new List<TestObjectWithList> { new(), new(), new() };

        var resource = new Resource()
            .MapListDataFrom("testObjects", testObjects)
                .Map(x => x.StringValue)
                .Map(x => x.IntValue)
                .MapListDataFrom(x => x.TestObjects)
                    .Map(x => x.StringValue)
                    .Map(x => x.IntValue)
                .EndMap()
            .EndMap();

        var json = resource.ToSlySoftHalJson();

        //act
        var deserializedResource = new Resource().FromSlySoftHalJson(json);

        //assert
        var storedParent = deserializedResource.Data["testObjects"] as ListData;
        Assert.IsNotNull(storedParent);
        Assert.AreEqual(testObjects[0].StringValue, storedParent[0]["stringValue"]);
        Assert.AreEqual(testObjects[0].IntValue, Convert.ToInt32(storedParent[0]["intValue"]));

        var storedChild = storedParent[0]["testObjects"] as ListData;
        Assert.IsNotNull(storedChild);
        Assert.AreEqual(testObjects[0].TestObjects[0].StringValue, storedChild[0]["stringValue"]);
        Assert.AreEqual(testObjects[0].TestObjects[0].IntValue, Convert.ToInt32(storedChild[0]["intValue"]));
        Assert.AreEqual(testObjects[0].TestObjects[1].StringValue, storedChild[1]["stringValue"]);
        Assert.AreEqual(testObjects[0].TestObjects[1].IntValue, Convert.ToInt32(storedChild[1]["intValue"]));
        Assert.AreEqual(testObjects[0].TestObjects[2].StringValue, storedChild[2]["stringValue"]);
        Assert.AreEqual(testObjects[0].TestObjects[2].IntValue, Convert.ToInt32(storedChild[2]["intValue"]));
    }
}