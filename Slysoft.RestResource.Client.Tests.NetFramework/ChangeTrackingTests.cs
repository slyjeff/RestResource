using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Slysoft.RestResource.Client.Tests.Common;
using Slysoft.RestResource.Extensions;
using TestUtils;

namespace Slysoft.RestResource.Client.Tests.NetFramework;

[TestClass]
public class ChangeTrackingTests {
    private static ISimpleResource CreateAccessor(Resource resource) {
        return ResourceAccessorFactory.CreateAccessor<ISimpleResource>(resource, new Mock<IRestClient>().Object);
    }

    [TestMethod]
    public void MustBeAbleToUpdateAValue() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var resource = new Resource().Data("message", originalMessage);
        var destination = CreateAccessor(resource);

        //act
        var newMessage = GenerateRandom.String();
        destination.Message = newMessage;

        //assert
        Assert.AreEqual(newMessage, destination.Message);
    }

    [TestMethod]
    public void ChangingValueMustNotifyPropertyChanged() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var resource = new Resource().Data("message", originalMessage);
        var destination = CreateAccessor(resource);

        var propertyChanged = false;
        destination.PropertyChanged += (_, e) => {
            propertyChanged = e.PropertyName == nameof(destination.Message);
        };

        //act
        destination.Message = GenerateRandom.String();

        //assert
        Assert.IsTrue(propertyChanged);
    }

    [TestMethod]
    public void HasDataChangesPropertyMustBeFalseIfNoDataChanged() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var resource = new Resource().Data("message", originalMessage);
        var destination = CreateAccessor(resource);

        //act

        //assert
        Assert.IsFalse(destination.IsChanged);
    }


    [TestMethod]
    public void HasDataChangesPropertyMustBeTrueIfDataChanged() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var resource = new Resource().Data("message", originalMessage);
        var destination = CreateAccessor(resource);

        //act
        destination.Message = GenerateRandom.String();

        //assert
        Assert.IsTrue(destination.IsChanged);
    }

    [TestMethod]
    public void HasDataChangesPropertyMustBeFalseIfDataChangeReverted() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var resource = new Resource().Data("message", originalMessage);
        var destination = CreateAccessor(resource);
        destination.Message = GenerateRandom.String();

        //act
        destination.Message = originalMessage;

        //assert
        Assert.IsFalse(destination.IsChanged);
    }

    [TestMethod]
    public void RevertDataChangesMustClearAllChangedValues() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var resource = new Resource().Data("message", originalMessage);
        var destination = CreateAccessor(resource);
        destination.Message = GenerateRandom.String();

        var propertyChanged = false;
        destination.PropertyChanged += (_, e) => {
            propertyChanged = e.PropertyName == nameof(destination.Message);
        };


        //act
        destination.RejectChanges();

        //assert
        Assert.IsFalse(destination.IsChanged);
        Assert.IsTrue(propertyChanged);
        Assert.AreEqual(originalMessage, destination.Message);
    }
}