using Microsoft.VisualStudio.TestTools.UnitTesting;
using Slysoft.RestResource.Extensions;
using TestUtils;

namespace Slysoft.RestResource.Tests; 

[TestClass]
public sealed class ResourceMappedDataTests {
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
        Assert.AreEqual(testObject.IntValue.ToString(), resource.Data["intValue"]);
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
        Assert.AreEqual(testObject1.IntValue.ToString(), resource.Data["intValue"]);
        Assert.AreEqual(testObject2.DateValue.ToString(), resource.Data["dateValue"]);
        Assert.AreEqual(testObject2.FloatValue.ToString(), resource.Data["floatValue"]);
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
        Assert.AreEqual(testObject.DateValue.ToString("hh:mm"), resource.Data["dateValue"]);
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
        Assert.AreEqual(testObject.DateValue.ToString("hh:mm"), resource.Data["time"]);
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
        Assert.AreEqual(testObject.IntValue.ToString(), resource.Data["intValue"]);
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
        Assert.AreEqual(testObject.IntValue.ToString("c"), resource.Data["intValue"]);
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
        Assert.AreEqual(testObject.IntValue.ToString(), resource.Data["intValue"]);
        Assert.AreEqual(message, resource.Data["message"]);
    }
}