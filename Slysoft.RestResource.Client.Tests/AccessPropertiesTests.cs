using Microsoft.VisualStudio.TestTools.UnitTesting;
using Slysoft.RestResource.Client.Tests.Common;
using Slysoft.RestResource.Extensions;

namespace Slysoft.RestResource.Client.Tests;

[TestClass]
public sealed class AccessPropertiesTests {
    [TestMethod]
    public void MustBeAbleToAccessAString() {
        //arrange
        var source = new SimpleResource();
        var resource = new Resource()
            .MapDataFrom(source)
                .Map(x => x.Message)
            .EndMap();

        //act
        var destination = ResourceAccessorFactory.CreateAccessor<ISimpleResource>(resource);

        //assert
        Assert.AreEqual(source.Message, destination.Message);
    }

    [TestMethod]
    public void MustBeAbleToAccessAnInt() {
        //arrange
        var source = new SimpleResource();
        var resource = new Resource()
            .Data("number", source.Number.ToString());

        //act
        var destination = ResourceAccessorFactory.CreateAccessor<ISimpleResource>(resource);

        //assert
        Assert.AreEqual(source.Number, destination.Number);
    }

    [TestMethod]
    public void MustBeAbleToAccessAnEnum() {
        //arrange
        var source = new SimpleResource();
        var resource = new Resource()
            .Data("option", source.Option.ToString());

        //act
        var destination = ResourceAccessorFactory.CreateAccessor<ISimpleResource>(resource);

        //assert
        Assert.AreEqual(source.Option, destination.Option);
    }

    [TestMethod]
    public void MustBeAbleToAccessAnNullable() {
        //arrange
        var source = new SimpleResource();
        var resource = new Resource()
            .Data("isOptional", source.IsOptional.ToString());

        //act
        var destination = ResourceAccessorFactory.CreateAccessor<ISimpleResource>(resource);

        //assert
        Assert.AreEqual(source.IsOptional, destination.IsOptional);
    }

    [TestMethod]
    public void MustBeAbleToAccessFormattedValue() {
        //arrange
        var source = new SimpleResource();
        var resource = new Resource()
            .MapDataFrom(source)
            .Map(x => x.Date, "yyyy-MM-dd hh:mm:ss tt")
            .EndMap();

        //act
        var destination = ResourceAccessorFactory.CreateAccessor<ISimpleResource>(resource);

        //assert
        Assert.AreEqual(source.Date.ToString(), destination.Date.ToString());
    }

    [TestMethod]
    public void MustBeAbleToAccessAListOfStrings() {
        //arrange
        var source = new SimpleResource();
        var resource = new Resource()
            .MapDataFrom(source)
                .Map(x => x.Strings)
            .EndMap();

        //act
        var destination = ResourceAccessorFactory.CreateAccessor<ISimpleResource>(resource);

        //assert
        Assert.AreEqual(source.Strings[0], destination.Strings[0]);
        Assert.AreEqual(source.Strings[1], destination.Strings[1]);
        Assert.AreEqual(source.Strings[2], destination.Strings[2]);
    }

    [TestMethod]
    public void MustBeAbleToAccessAListONumbers() {
        //arrange
        var source = new SimpleResource();
        var resource = new Resource()
            .MapDataFrom(source)
                .Map(x => x.Numbers)
            .EndMap();

        //act
        var destination = ResourceAccessorFactory.CreateAccessor<ISimpleResource>(resource);

        //assert
        Assert.AreEqual(source.Numbers[0], destination.Numbers[0]);
        Assert.AreEqual(source.Numbers[1], destination.Numbers[1]);
        Assert.AreEqual(source.Numbers[2], destination.Numbers[2]);
    }

    [TestMethod]
    public void MustBeAbleToAccessAnObject() {
        //arrange
        var source = new SimpleResource();
        var resource = new Resource()
            .MapDataFrom(source)
                .Map(x => x.Child)
            .EndMap();

        //act
        var destination = ResourceAccessorFactory.CreateAccessor<ISimpleResource>(resource);

        //assert
        Assert.AreEqual(source.Child.ChildMessage, destination.Child.ChildMessage);
        Assert.AreEqual(source.Child.ChildNumber, destination.Child.ChildNumber);
    }

    [TestMethod]
    public void MustBeAbleToAccessAListOfObjects() {
        //arrange
        var source = new SimpleResource();
        var resource = new Resource()
            .MapDataFrom(source)
            .Map(x => x.Children)
            .EndMap();

        //act
        var destination = ResourceAccessorFactory.CreateAccessor<ISimpleResource>(resource);

        //assert
        Assert.AreEqual(source.Children[0].ChildMessage, destination.Children[0].ChildMessage);
        Assert.AreEqual(source.Children[0].ChildNumber, destination.Children[0].ChildNumber);
        Assert.AreEqual(source.Children[1].ChildMessage, destination.Children[1].ChildMessage);
        Assert.AreEqual(source.Children[1].ChildNumber, destination.Children[1].ChildNumber);
        Assert.AreEqual(source.Children[2].ChildMessage, destination.Children[2].ChildMessage);
        Assert.AreEqual(source.Children[2].ChildNumber, destination.Children[2].ChildNumber);
    }

    [TestMethod]
    public void MustBeAbleToAccessAnInterface() {
        //arrange
        var source = new SimpleResource();
        var resource = new Resource()
            .MapDataFrom(source)
                .Map(x => x.ChildInterface)
            .EndMap();

        //act
        var destination = ResourceAccessorFactory.CreateAccessor<ISimpleResource>(resource);

        //assert
        Assert.AreEqual(source.ChildInterface.ChildMessage, destination.ChildInterface.ChildMessage);
        Assert.AreEqual(source.ChildInterface.ChildNumber, destination.ChildInterface.ChildNumber);
    }

    [TestMethod]
    public void MustBeAbleToAccessAListOfInterfaces() {
        //arrange
        var source = new SimpleResource();
        var resource = new Resource()
            .MapDataFrom(source)
                .Map(x => x.ChildInterfaces)
            .EndMap();

        //act
        var destination = ResourceAccessorFactory.CreateAccessor<ISimpleResource>(resource);

        //assert
        Assert.AreEqual(source.ChildInterfaces[0].ChildMessage, destination.ChildInterfaces[0].ChildMessage);
        Assert.AreEqual(source.ChildInterfaces[0].ChildNumber, destination.ChildInterfaces[0].ChildNumber);
        Assert.AreEqual(source.ChildInterfaces[1].ChildMessage, destination.ChildInterfaces[1].ChildMessage);
        Assert.AreEqual(source.ChildInterfaces[1].ChildNumber, destination.ChildInterfaces[1].ChildNumber);
        Assert.AreEqual(source.ChildInterfaces[2].ChildMessage, destination.ChildInterfaces[2].ChildMessage);
        Assert.AreEqual(source.ChildInterfaces[2].ChildNumber, destination.ChildInterfaces[2].ChildNumber);
    }
}