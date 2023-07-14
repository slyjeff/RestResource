using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtils;

namespace SlySoft.RestResource.Tests;

[TestClass]
public sealed class MappedDataTests {
    [TestMethod]
    public void MustBeAbleToMapDataFromObject() {
        //arrange
        var testObject = new TestObject();

        //act
        var resource = new Resource()
            .MapDataFrom(testObject)
                .Map(x => x.StringValue)
                .Map(x => x.IntValue)
            .EndMap();

        //assert
        Assert.AreEqual(testObject.StringValue, resource.Data["stringValue"]);
        Assert.AreEqual(testObject.IntValue, resource.Data["intValue"]);
    }

    [TestMethod]
    public void MustBeAbleToMapDataFromMultipleObjects() {
        //arrange
        var testObject1 = new TestObject();
        var testObject2 = new TestObject2();

        //act
        var resource = new Resource()
            .MapDataFrom(testObject1)
                .Map(x => x.StringValue)
                .Map(x => x.IntValue)
            .EndMap()
            .MapDataFrom(testObject2)
                .Map(x => x.DateValue)
                .Map(x => x.FloatValue)
            .EndMap();

        //assert
        Assert.AreEqual(testObject1.StringValue, resource.Data["stringValue"]);
        Assert.AreEqual(testObject1.IntValue, resource.Data["intValue"]);
        Assert.AreEqual(testObject2.DateValue, resource.Data["dateValue"]);
        Assert.AreEqual(testObject2.FloatValue, resource.Data["floatValue"]);
    }

    [TestMethod]
    public void MustBeAbleToFormatData() {
        //arrange
        var testObject = new TestObject2();

        //act
        var resource = new Resource()
            .MapDataFrom(testObject)
                .Map(x => x.DateValue, "hh:mm")
            .EndMap();

        //assert
        Assert.AreEqual(testObject.DateValue.ToString("hh:mm"), resource.Data["dateValue"]?.ToString());
    }

    [TestMethod]
    public void MustBeAbleToSetName() {
        //arrange
        var testObject = new TestObject2();

        //act
        var resource = new Resource()
            .MapDataFrom(testObject)
                .Map("time", x => x.DateValue, "hh:mm")
            .EndMap();

        //assert
        Assert.AreEqual(testObject.DateValue.ToString("hh:mm"), resource.Data["time"]?.ToString());
    }

    [TestMethod]
    public void MustSupportMapAll() {
        //arrange
        var testObject = new TestObject();

        //act
        var resource = new Resource()
            .MapDataFrom(testObject)
                .MapAll()
            .EndMap();

        //assert
        Assert.AreEqual(testObject.StringValue, resource.Data["stringValue"]);
        Assert.AreEqual(testObject.IntValue, resource.Data["intValue"]);
    }

    [TestMethod]
    public void MapAllMustNotOverwriteFormatting() {
        //arrange
        var testObject = new TestObject();

        //act
        var resource = new Resource()
            .MapDataFrom(testObject)
                .Map(x => x.IntValue, "c")
                .MapAll()
            .EndMap();

        //assert
        Assert.AreEqual(testObject.StringValue, resource.Data["stringValue"]);
        Assert.AreEqual(testObject.IntValue.ToString("c"), resource.Data["intValue"]?.ToString());
    }

    [TestMethod]
    public void MapAllMustNotIncludeExcludedFields() {
        //arrange
        var testObject = new TestObject();

        //act
        var resource = new Resource()
            .MapDataFrom(testObject)
                .Exclude(x => x.IntValue)
                .MapAll()
            .EndMap();

        //assert
        Assert.AreEqual(testObject.StringValue, resource.Data["stringValue"]);
        Assert.IsFalse(resource.Data.ContainsKey("intValue"));
    }

    [TestMethod]
    public void MapAllMustNotIncludeExcludedFieldsEvenIfMapAllIsCalledFirst() {
        //arrange
        var testObject = new TestObject();

        //act
        var resource = new Resource()
            .MapDataFrom(testObject)
                .MapAll()
                .Exclude(x => x.IntValue)
            .EndMap();

        //assert
        Assert.AreEqual(testObject.StringValue, resource.Data["stringValue"]);
        Assert.IsFalse(resource.Data.ContainsKey("intValue"));
    }

    [TestMethod]
    public void MapAllMustOnlyIncludeFieldsDefinedInAnInterface() {
        //arrange
        IHasStringValue testObject = new TestObject();

        //act
        var resource = new Resource()
            .MapDataFrom(testObject)
                .MapAll()
            .EndMap();

        //assert
        Assert.AreEqual(testObject.StringValue, resource.Data["stringValue"]);
        Assert.IsFalse(resource.Data.ContainsKey("intValue"));
    }

    [TestMethod]
    public void MustSupportMapAllFrom() {
        //arrange
        var testObject = new TestObject();
        var message = GenerateRandom.String();

        //act
        var resource = new Resource()
            .MapAllDataFrom(testObject)
            .Data("message", message); //mapping data to show that this chains without using EndMap()

        //assert
        Assert.AreEqual(testObject.StringValue, resource.Data["stringValue"]);
        Assert.AreEqual(testObject.IntValue, resource.Data["intValue"]);
        Assert.AreEqual(message, resource.Data["message"]);
    }

    [TestMethod]
    public void MustSupportMappingLists() {
        //arrange
        var testObjects = new List<TestObject> { new(), new(), new() };

        //act
        var resource = new Resource()
            .MapListDataFrom("value", testObjects)
                .Map(x => x.StringValue)
                .Map(x => x.IntValue)
            .EndMap();

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

    [TestMethod]
    public void MustBeAbleToFormatDataWhenMappingList() {
        var testObjects = new List<TestObject2> { new(), new(), new() };

        //act
        var resource = new Resource()
            .MapListDataFrom("value", testObjects)
                .Map(x => x.DateValue, "hh:mm")
            .EndMap();

        //assert
        var storedObject = resource.Data["value"] as IList<IDictionary<string, object?>>;
        Assert.IsNotNull(storedObject);
        Assert.AreEqual(testObjects[0].DateValue.ToString("hh:mm"), storedObject[0]["dateValue"]?.ToString());
        Assert.AreEqual(testObjects[1].DateValue.ToString("hh:mm"), storedObject[1]["dateValue"]?.ToString());
        Assert.AreEqual(testObjects[2].DateValue.ToString("hh:mm"), storedObject[2]["dateValue"]?.ToString());
    }

    [TestMethod]
    public void MustBeAbleToSetNameWhenMappingList() {
        var testObjects = new List<TestObject2> { new(), new(), new() };

        //act
        var resource = new Resource()
            .MapListDataFrom("value", testObjects)
                .Map("time", x => x.DateValue, "hh:mm")
            .EndMap();

        //assert
        var storedObject = resource.Data["value"] as IList<IDictionary<string, object?>>;
        Assert.IsNotNull(storedObject);
        Assert.AreEqual(testObjects[0].DateValue.ToString("hh:mm"), storedObject[0]["time"]?.ToString());
        Assert.AreEqual(testObjects[1].DateValue.ToString("hh:mm"), storedObject[1]["time"]?.ToString());
        Assert.AreEqual(testObjects[2].DateValue.ToString("hh:mm"), storedObject[2]["time"]?.ToString());
    }

    [TestMethod]
    public void MustBeAbleToMapAllWhenMappingLists() {
        //arrange
        var testObjects = new List<TestObject> { new(), new(), new() };

        //act
        var resource = new Resource()
            .MapListDataFrom("value", testObjects)
                .MapAll()
            .EndMap();

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

    [TestMethod]
    public void MustBeAbleToExcludeFromMapAllWhenMappingLists() {
        //arrange
        var testObjects = new List<TestObject> { new(), new(), new() };

        //act
        var resource = new Resource()
            .MapListDataFrom("value", testObjects)
                .Exclude(x => x.IntValue)
                .MapAll()
            .EndMap();

        //assert
        var storedObject = resource.Data["value"] as IList<IDictionary<string, object?>>;
        Assert.IsNotNull(storedObject);
        Assert.AreEqual(testObjects[0].StringValue, storedObject[0]["stringValue"]);
        Assert.IsFalse(storedObject[0].ContainsKey("intValue"));
        Assert.AreEqual(testObjects[1].StringValue, storedObject[1]["stringValue"]);
        Assert.IsFalse(storedObject[1].ContainsKey("intValue"));
        Assert.AreEqual(testObjects[2].StringValue, storedObject[2]["stringValue"]);
        Assert.IsFalse(storedObject[2].ContainsKey("intValue"));
    }

    [TestMethod]
    public void MustBeAbleToMapAllFromList() {
        //arrange
        var testObjects = new List<TestObject> { new(), new(), new() };
        var message = GenerateRandom.String();

        //act
        var resource = new Resource()
            .MapAllListDataFrom("value", testObjects)
            .Data("message", message); //mapping data to show that this chains without using EndMap()


        //assert
        var storedObject = resource.Data["value"] as IList<IDictionary<string, object?>>;
        Assert.IsNotNull(storedObject);
        Assert.AreEqual(testObjects[0].StringValue, storedObject[0]["stringValue"]);
        Assert.AreEqual(testObjects[0].IntValue, storedObject[0]["intValue"]);
        Assert.AreEqual(testObjects[1].StringValue, storedObject[1]["stringValue"]);
        Assert.AreEqual(testObjects[1].IntValue, storedObject[1]["intValue"]);
        Assert.AreEqual(testObjects[2].StringValue, storedObject[2]["stringValue"]);
        Assert.AreEqual(testObjects[2].IntValue, storedObject[2]["intValue"]);
        Assert.AreEqual(message, resource.Data["message"]);
    }

    [TestMethod]
    public void MustBeAbleToMapListWhenMappingAnObject() {
        //arrange
        var testObject = new TestObjectWithList();

        //act
        var resource = new Resource()
            .MapDataFrom(testObject)
                .Map(x => x.StringValue)
                .Map(x => x.IntValue)
                .MapListDataFrom(x => x.TestObjects)
                    .Map(x => x.StringValue)
                    .Map(x => x.IntValue)
                .EndMap()
            .EndMap();

        //assert
        var storedObject = resource.Data["testObjects"] as IList<IDictionary<string, object?>>;
        Assert.IsNotNull(storedObject);
        Assert.AreEqual(testObject.StringValue, resource.Data["stringValue"]);
        Assert.AreEqual(testObject.IntValue, resource.Data["intValue"]);
        Assert.AreEqual(testObject.TestObjects[0].StringValue, storedObject[0]["stringValue"]);
        Assert.AreEqual(testObject.TestObjects[0].IntValue, storedObject[0]["intValue"]);
        Assert.AreEqual(testObject.TestObjects[1].StringValue, storedObject[1]["stringValue"]);
        Assert.AreEqual(testObject.TestObjects[1].IntValue, storedObject[1]["intValue"]);
        Assert.AreEqual(testObject.TestObjects[2].StringValue, storedObject[2]["stringValue"]);
        Assert.AreEqual(testObject.TestObjects[2].IntValue, storedObject[2]["intValue"]);
    }

    [TestMethod]
    public void MustBeAbleToMapListsInList() {
        //arrange
        var testObjects = new List<TestObjectWithList>{ new(), new(), new() };

        //act
        var resource = new Resource()
            .MapListDataFrom("testObjects", testObjects)
                .Map(x => x.StringValue)
                .Map(x => x.IntValue)
                .MapListDataFrom(x => x.TestObjects)
                    .Map(x => x.StringValue)
                    .Map(x => x.IntValue)
                .EndMap()
            .EndMap();

        //assert
        var storedParent = resource.Data["testObjects"] as IList<IDictionary<string, object?>>;
        Assert.IsNotNull(storedParent);
        Assert.AreEqual(testObjects[0].StringValue, storedParent[0]["stringValue"]);
        Assert.AreEqual(testObjects[0].IntValue, storedParent[0]["intValue"]);

        var storedChild = storedParent[0]["testObjects"] as IList<IDictionary<string, object?>>;
        Assert.IsNotNull(storedChild);
        Assert.AreEqual(testObjects[0].TestObjects[0].StringValue, storedChild[0]["stringValue"]);
        Assert.AreEqual(testObjects[0].TestObjects[0].IntValue, storedChild[0]["intValue"]);
        Assert.AreEqual(testObjects[0].TestObjects[1].StringValue, storedChild[1]["stringValue"]);
        Assert.AreEqual(testObjects[0].TestObjects[1].IntValue, storedChild[1]["intValue"]);
        Assert.AreEqual(testObjects[0].TestObjects[2].StringValue, storedChild[2]["stringValue"]);
        Assert.AreEqual(testObjects[0].TestObjects[2].IntValue, storedChild[2]["intValue"]);
    }
}
