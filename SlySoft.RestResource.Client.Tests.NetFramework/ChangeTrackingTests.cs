using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SlySoft.RestResource;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using SlySoft.RestResource.Client;
using SlySoft.RestResource.Client.Tests.Common;
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
        var accessor = CreateAccessor(resource);

        //act
        var newMessage = GenerateRandom.String();
        accessor.Message = newMessage;

        //assert
        Assert.AreEqual(newMessage, accessor.Message);
    }


    [TestMethod]
    public void ChangingValueMustNotifyPropertyChanged() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var resource = new Resource().Data("message", originalMessage);
        var accessor = CreateAccessor(resource);

        var propertyChanged = false;
        accessor.PropertyChanged += (_, e) => {
            if (e.PropertyName == nameof(accessor.Message)) {
                propertyChanged = true;
            }
        };

        //act
        accessor.Message = GenerateRandom.String();

        //assert
        Assert.IsTrue(propertyChanged);
    }

    [TestMethod]
    public void HasDataChangesPropertyMustBeFalseIfNoDataChanged() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var resource = new Resource().Data("message", originalMessage);
        var accessor = CreateAccessor(resource);

        //act

        //assert
        Assert.IsFalse(accessor.IsChanged);
    }


    [TestMethod]
    public void IsChangedPropertyMustBeTrueIfDataChanged() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var resource = new Resource().Data("message", originalMessage);
        var accessor = CreateAccessor(resource);

        //act
        accessor.Message = GenerateRandom.String();

        //assert
        Assert.IsTrue(accessor.IsChanged);
    }

    [TestMethod]
    public void IsChangedChangingMustNotifyPropertyChanged() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var resource = new Resource().Data("message", originalMessage);
        var accessor = CreateAccessor(resource);

        var isChangedChanged = false;
        accessor.PropertyChanged += (_, e) => {
            if (e.PropertyName == nameof(accessor.IsChanged)) {
                isChangedChanged = true;
            }
        };

        //act
        accessor.Message = GenerateRandom.String();

        //assert
        Assert.IsTrue(isChangedChanged);
    }

    [TestMethod]
    public void IsChangedShouldNotBeChangedIfPropertyWasAlreadyChanged() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var resource = new Resource().Data("message", originalMessage);
        var accessor = CreateAccessor(resource);
        accessor.Message = GenerateRandom.String();

        var isChangedChanged = false;
        accessor.PropertyChanged += (_, e) => {
            if (e.PropertyName == nameof(accessor.IsChanged)) {
                isChangedChanged = true;
            }
        };

        //act
        accessor.Message = GenerateRandom.String();

        //assert
        Assert.IsFalse(isChangedChanged);
    }

    [TestMethod]
    public void IsChangedMustBeFalseIfDataChangeReverted() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var resource = new Resource().Data("message", originalMessage);
        var accessor = CreateAccessor(resource);
        accessor.Message = GenerateRandom.String();

        //act
        accessor.Message = originalMessage;

        //assert
        Assert.IsFalse(accessor.IsChanged);
    }

    [TestMethod]
    public void RevertDataChangesMustClearAllChangedValues() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var resource = new Resource().Data("message", originalMessage);
        var accessor = CreateAccessor(resource);
        accessor.Message = GenerateRandom.String();

        var messageChanged = false;
        var isChangedChanged = false;
        accessor.PropertyChanged += (_, e) => {
            switch (e.PropertyName) {
                case nameof(accessor.Message):
                    messageChanged = true;
                    break;
                case nameof(accessor.IsChanged):
                    isChangedChanged = true;
                    break;
            }
        };

        //act
        accessor.RejectChanges();

        //assert
        Assert.IsFalse(accessor.IsChanged);
        Assert.IsTrue(messageChanged);
        Assert.IsTrue(isChangedChanged);
        Assert.AreEqual(originalMessage, accessor.Message);
    }

    [TestMethod]
    public void MustBeAbleToChangeValueOfChildAccessors() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var child = new ChildResource { ChildMessage = originalMessage };

        var resource = new Resource().Data("childInterface", child);
        var parent = CreateAccessor(resource);

        //act
        var newMessage = GenerateRandom.String();
        parent.ChildInterface.ChildMessage = newMessage;

        //assert
        Assert.AreEqual(newMessage, parent.ChildInterface.ChildMessage);
    }

    [TestMethod]
    public void ChangingValueOnAChildAccessorMustRaisePropertyChangedEvent() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var child = new ChildResource { ChildMessage = originalMessage };

        var resource = new Resource().Data("childInterface", child);
        var parent = CreateAccessor(resource);

        var childMessageChanged = false;
        // ReSharper disable once SuspiciousTypeConversion.Global
        ((INotifyPropertyChanged)parent.ChildInterface).PropertyChanged += (_, e) => {
            if (e.PropertyName == nameof(parent.ChildInterface.ChildMessage)) {
                childMessageChanged = true;
            }
        };

        //act
        var newMessage = GenerateRandom.String();
        parent.ChildInterface.ChildMessage = newMessage;

        //assert
        Assert.IsTrue(childMessageChanged);
    }

    [TestMethod]
    public void ChangingValueOnAChildAccessorMustChangeIsChangedOfParent() {
        //arrange
        var child = new ChildResource();

        var resource = new Resource().Data("childInterface", child);
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

    [TestMethod]
    public void RejectChangesOnParentMustRevertChildAccessor() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var child = new ChildResource { ChildMessage = originalMessage };

        var resource = new Resource().Data("childInterface", child);
        var parent = CreateAccessor(resource);

        var newMessage = GenerateRandom.String();
        parent.ChildInterface.ChildMessage = newMessage;

        //act
        parent.RejectChanges();

        //assert
        Assert.AreEqual(originalMessage, parent.ChildInterface.ChildMessage);
    }

    [TestMethod]
    public void ChangingListValueMustNotifyOfCollectionChanged() {
        //arrange
        var originalStrings = new List<string> { GenerateRandom.String(), GenerateRandom.String(), GenerateRandom.String() };
        var resource = new Resource().Data("strings", originalStrings);
        var accessor = CreateAccessor(resource);

        var collectionChanged = false;
        ((INotifyCollectionChanged)accessor.Strings).CollectionChanged += (_, _) => {
            collectionChanged = true;
        };

        //act
        accessor.Strings[1] = GenerateRandom.String();

        //assert
        Assert.IsTrue(collectionChanged);
    }

    [TestMethod]
    public void ChangingListValueMustIsChangedToFalse() {
        //arrange
        var originalStrings = new List<string> { GenerateRandom.String(), GenerateRandom.String(), GenerateRandom.String() };
        var resource = new Resource().Data("strings", originalStrings);
        var accessor = CreateAccessor(resource);

        var isChangedChanged = false;
        accessor.PropertyChanged += (_, e) => {
            if (e.PropertyName == nameof(accessor.IsChanged)) {
                isChangedChanged = true;
            }
        };

        //act
        accessor.Strings[1] = GenerateRandom.String();

        //assert
        Assert.IsTrue(isChangedChanged);
        Assert.IsTrue(accessor.IsChanged);
    }

    [TestMethod]
    public void RevertingListValueMustSetIsChangedToFalse() {
        //arrange
        var originalStrings = new List<string> { GenerateRandom.String(), GenerateRandom.String(), GenerateRandom.String() };
        var resource = new Resource().Data("strings", originalStrings);
        var accessor = CreateAccessor(resource);

        //act
        accessor.Strings[1] = originalStrings[1];

        //assert
        Assert.IsFalse(accessor.IsChanged);
    }

    [TestMethod]
    public void RejectChangesMustRevertList() {
        //arrange
        var originalStrings = new List<string> { GenerateRandom.String(), GenerateRandom.String(), GenerateRandom.String() };
        var resource = new Resource().Data("strings", originalStrings);
        var accessor = CreateAccessor(resource);

        //act
        accessor.RejectChanges();

        //assert
        Assert.AreEqual(originalStrings[1], accessor.Strings[1]);
    }

    [TestMethod]
    public void MustBeAbleToChangeValueOfChildAccessorInAList() {
        //arrange
        var originalChildren = new List<ChildResource> { new(), new(), new() };

        var resource = new Resource().Data("ChildInterfaces", originalChildren);
        var parent = CreateAccessor(resource);

        //act
        var newMessage = GenerateRandom.String();
        parent.ChildInterfaces[1].ChildMessage = newMessage;

        //assert
        Assert.AreEqual(newMessage, parent.ChildInterfaces[1].ChildMessage);
    }

    [TestMethod]
    public void ChangingValueOnAChildAccessorInAListMustRaisePropertyChangedEvent() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var originalChildren = new List<ChildResource> { new(), new() { ChildMessage = originalMessage}, new() };

        var resource = new Resource().Data("ChildInterfaces", originalChildren);
        var parent = CreateAccessor(resource);

        var childMessageChanged = false;
        // ReSharper disable once SuspiciousTypeConversion.Global
        ((INotifyPropertyChanged)parent.ChildInterfaces[1]).PropertyChanged += (_, e) => {
            if (e.PropertyName == nameof(IChildResource.ChildMessage)) {
                childMessageChanged = true;
            }
        };

        //act
        var newMessage = GenerateRandom.String();
        parent.ChildInterfaces[1].ChildMessage = newMessage;

        //assert
        Assert.IsTrue(childMessageChanged);
    }

    [TestMethod]
    public void ChangingValueOnAChildAccessorInAListMustChangeIsChangedOfParent() {
        //arrange
        var originalChildren = new List<ChildResource> { new(), new(), new() };
        var resource = new Resource().Data("ChildInterfaces", originalChildren);
        var parent = CreateAccessor(resource);

        var parentIsChangedChanged = false;
        parent.PropertyChanged += (_, e) => {
            if (e.PropertyName == nameof(parent.IsChanged)) {
                parentIsChangedChanged = true;
            }
        };

        //act
        var newMessage = GenerateRandom.String();
        parent.ChildInterfaces[1].ChildMessage = newMessage;

        //assert
        Assert.IsTrue(parent.IsChanged);
        Assert.IsTrue(parentIsChangedChanged);
    }

    [TestMethod]
    public void RejectChangesOnParentMustRevertChildAccessorInAList() {
        //arrange
        var originalMessage = GenerateRandom.String();
        var originalChildren = new List<ChildResource> { new(), new() { ChildMessage = originalMessage }, new() };

        var resource = new Resource().Data("ChildInterfaces", originalChildren);
        var parent = CreateAccessor(resource);

        var newMessage = GenerateRandom.String();
        parent.ChildInterfaces[1].ChildMessage = newMessage;

        //act
        parent.RejectChanges();

        //assert
        Assert.AreEqual(originalMessage, parent.ChildInterfaces[1].ChildMessage);
    }
}