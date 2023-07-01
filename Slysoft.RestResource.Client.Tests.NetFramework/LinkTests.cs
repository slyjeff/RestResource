using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Slysoft.RestResource.Client.Tests.Common;
using Slysoft.RestResource.Extensions;
using System.Collections.Generic;
using Slysoft.RestResource.Client.Tests.Common.Extensions;
using TestUtils;

namespace Slysoft.RestResource.Client.Tests.NetFramework;

[TestClass]
public sealed class LinkTests {
    private Mock<IRestClient> _mockRestClient = null!;

    [TestInitialize]
    public void SetUp() {
        _mockRestClient = new Mock<IRestClient>();
    }


    [TestMethod]
    public void GetWithoutParametersMustMakeCall() {
        //arrange
        var linkTestResource = new Resource()
            .Get("getAllUsers", "/user");

        var linkTest = ResourceAccessorFactory.CreateAccessor<ILinkTest>(linkTestResource, _mockRestClient.Object);

        //act
        linkTest.GetAllUsers();

        //assert
        _mockRestClient.VerifyCall<IUserList>("/user");
    }

    [TestMethod]
    public void MustSupportAsyncCalls() {
        //arrange
        var linkTestResource = new Resource()
            .Get("getAllUsers", "/user");

        var linkTest = ResourceAccessorFactory.CreateAccessor<ILinkTestAsync>(linkTestResource, _mockRestClient.Object);

        //act
        linkTest.GetAllUsers().Wait();

        //assert
        _mockRestClient.VerifyAsyncCall<IUserList>("/user");
    }

    [TestMethod]
    public void GetMustReturnAccessor() {
        //arrange
        var user1Name = GenerateRandom.String();
        var user2Name = GenerateRandom.String();

        var linkTestResource = new Resource()
            .Get("getAllUsers", "/user");

        var linkTest = ResourceAccessorFactory.CreateAccessor<ILinkTest>(linkTestResource, _mockRestClient.Object);

        var userResourceList = new List<Resource> {
            new Resource().Data("name", user1Name),
            new Resource().Data("name", user2Name),
        };
        var userListResource = new Resource().Embedded("users", userResourceList);
        var userListAccessor = ResourceAccessorFactory.CreateAccessor<IUserList>(userListResource, _mockRestClient.Object);

        _mockRestClient.SetupCall<IUserList>("/user").Returns(userListAccessor);

        //act
        var userList = linkTest.GetAllUsers();

        //assert
        Assert.AreEqual(2, userList.Users.Count);
        Assert.AreEqual(user1Name, userList.Users[0].Name);
        Assert.AreEqual(user2Name, userList.Users[1].Name);
    }
}