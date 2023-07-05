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
            if (e.PropertyName == nameof(destination.Message)) {
                propertyChanged = true;
            }
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
    public void IsChangedPropertyMustBeTrueIfDataChanged() {
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
    public void IsChangedChangingMustNotifyPropertyChanged() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var resource = new Resource().Data("message", originalMessage);
        var destination = CreateAccessor(resource);

        var isChangedChanged = false;
        destination.PropertyChanged += (_, e) => {
            if (e.PropertyName == nameof(destination.IsChanged)) {
                isChangedChanged = true;
            }
        };

        //act
        destination.Message = GenerateRandom.String();

        //assert
        Assert.IsTrue(isChangedChanged);
    }

    [TestMethod]
    public void IsChangedShouldNotBeChangedIfProperyWasAlreadyChanged() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var resource = new Resource().Data("message", originalMessage);
        var destination = CreateAccessor(resource);
        destination.Message = GenerateRandom.String();

        var isChangedChanged = false;
        destination.PropertyChanged += (_, e) => {
            if (e.PropertyName == nameof(destination.IsChanged)) {
                isChangedChanged = true;
            }
        };

        //act
        destination.Message = GenerateRandom.String();

        //assert
        Assert.IsFalse(isChangedChanged);
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

        var messageChanged = false;
        var isChangedChanged = false;
        destination.PropertyChanged += (_, e) => {
            switch (e.PropertyName) {
                case nameof(destination.Message):
                    messageChanged = true;
                    break;
                case nameof(destination.IsChanged):
                    isChangedChanged = true;
                    break;
            }
        };

        //act
        destination.RejectChanges();

        //assert
        Assert.IsFalse(destination.IsChanged);
        Assert.IsTrue(messageChanged);
        Assert.IsTrue(isChangedChanged);
        Assert.AreEqual(originalMessage, destination.Message);
    }

    [TestMethod]
    public void MustBeAbleToChangeValueOfChildAccessors() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var child = new ChildResource { ChildMessage = originalMessage };
        var resource = new Resource()
            .Data("childInterface", child);
        var parent = CreateAccessor(resource);

        //act
        var newMessage = GenerateRandom.String();
        parent.ChildInterface.ChildMessage = newMessage;

        //assert
        Assert.AreEqual(newMessage, parent.ChildInterface.ChildMessage);
    }

    [TestMethod]
    public void ChangingValueOnAChildAccessorMustSetIsChangedAndRaisePropertyChangedEvents() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var child = new ChildResource { ChildMessage = originalMessage };
        var resource = new Resource()
            .Data("childInterface", child);
        var parent = CreateAccessor(resource);

        var childMessageChanged = false;
        var childIsChangedChanged = false;
        parent.ChildInterface.PropertyChanged += (_, e) => {
            switch (e.PropertyName) {
                case nameof(parent.ChildInterface.ChildMessage):
                    childMessageChanged = true;
                    break;
                case nameof(parent.ChildInterface.IsChanged):
                    childIsChangedChanged = true;
                    break;
            }
        };

        //act
        var newMessage = GenerateRandom.String();
        parent.ChildInterface.ChildMessage = newMessage;

        //assert
        Assert.IsTrue(parent.ChildInterface.IsChanged);
        Assert.IsTrue(childMessageChanged);
        Assert.IsTrue(childIsChangedChanged);
    }

    [TestMethod]
    public void ChangingValueOnAChildAccessorMustChangeIsChangedOfParent() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var child = new ChildResource { ChildMessage = originalMessage };
        var resource = new Resource()
            .Data("childInterface", child);
        var parent = CreateAccessor(resource);

        var parentIsChangedChanged = false;
        parent.PropertyChanged += (_, e) => {
            if (e.PropertyName == nameof(parent.IsChanged)) {
                parentIsChangedChanged = true;
            }
        };

        //act
        var newMessage = GenerateRandom.String();
        parent.ChildInterface.ChildMessage = newMessage;

        //assert
        Assert.IsTrue(parent.IsChanged);
        Assert.IsTrue(parentIsChangedChanged);
    }
}