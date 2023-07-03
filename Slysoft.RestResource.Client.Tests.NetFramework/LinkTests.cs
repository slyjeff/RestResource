using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Slysoft.RestResource.Client.Tests.Common;
using Slysoft.RestResource.Extensions;
using Slysoft.RestResource.Client.Tests.Common.Extensions;
using TestUtils;

namespace Slysoft.RestResource.Client.Tests.NetFramework;

[TestClass]
public sealed class LinkTests {
    private Mock<IRestClient> _mockRestClient = null!;
    private ILinkTest _linkTest = null!;
    private ILinkTestAsync _linkTestAsync = null!;
    private readonly string _defaultValue = GenerateRandom.String();


    [TestInitialize]
    public void SetUp() {
        _mockRestClient = new Mock<IRestClient>();
        var linkTestResource = new Resource()
            .Get("getAllUsers", "/user")
            .Get("getAllUsersTemplated", "/user/{id1}/{id2}", templated: true)
            .Query("searchUsers", "/user")
                .Parameter("lastName", defaultValue: _defaultValue)
                .Parameter("firstName")
            .EndQuery();

        _linkTest = ResourceAccessorFactory.CreateAccessor<ILinkTest>(linkTestResource, _mockRestClient.Object);
        _linkTestAsync = ResourceAccessorFactory.CreateAccessor<ILinkTestAsync>(linkTestResource, _mockRestClient.Object);
    }

    [TestMethod]
    public void GetWithoutParametersMustMakeCall() {
        //act
        _linkTest.GetAllUsers();

        //assert
        _mockRestClient.VerifyCall<IUserList>("/user");
    }

    [TestMethod]
    public void MustSupportAsyncCalls() {
        //act
        _linkTestAsync.GetAllUsers().Wait();

        //assert
        _mockRestClient.VerifyAsyncCall<IUserList>("/user");
    }

    [TestMethod]
    public void GetMustReturnAccessor() {
        //arrange
        var user1Name = GenerateRandom.String();
        var user2Name = GenerateRandom.String();

        var userListResource = TestData.CreateUserListResource(user1Name, user2Name);
        var userListAccessor = ResourceAccessorFactory.CreateAccessor<IUserList>(userListResource, _mockRestClient.Object);
        _mockRestClient.SetupCall<IUserList>("/user").Returns(userListAccessor);

        //act
        var userList = _linkTest.GetAllUsers();

        //assert
        Assert.AreEqual(2, userList.Users.Count);
        Assert.AreEqual(user1Name, userList.Users[0].Name);
        Assert.AreEqual(user2Name, userList.Users[1].Name);
    }

    [TestMethod]
    public void GetAsyncMustReturnAccessor() {
        //arrange
        var user1Name = GenerateRandom.String();
        var user2Name = GenerateRandom.String();
        var userListResource = TestData.CreateUserListResource(user1Name, user2Name);

        var userListAccessor = ResourceAccessorFactory.CreateAccessor<IUserList>(userListResource, _mockRestClient.Object);
        _mockRestClient.SetupCallAsync<IUserList>("/user").Returns(userListAccessor);

        //act
        var userList = _linkTestAsync.GetAllUsers().Result;

        //assert
        Assert.AreEqual(2, userList.Users.Count);
        Assert.AreEqual(user1Name, userList.Users[0].Name);
        Assert.AreEqual(user2Name, userList.Users[1].Name);
    }

    [TestMethod]
    public void MustBeAbleFillInTemplatedUrlsFromParameters() {
        //arrange
        var id1 = GenerateRandom.Int();
        var id2 = GenerateRandom.Int();

        //act
        _linkTest.GetAllUsersTemplated(id1, id2);

        //assert
        _mockRestClient.VerifyCall<IUserList>($"/user/{id1}/{id2}");
    }

    [TestMethod]
    public void MustBeAbleFillInTemplatedUrlsFromAnObject() {
        //arrange
        var id1 = GenerateRandom.Int();
        var id2 = GenerateRandom.Int();

        //act
        var ids = new { id1, id2 };
        _linkTest.GetAllUsersTemplated(ids);

        //assert
        _mockRestClient.VerifyCall<IUserList>($"/user/{id1}/{id2}");
    }

    [TestMethod]
    public void MustProvideQueryParametersFromParameters() {
        //arrange
        var lastName = GenerateRandom.String();
        var firstName = GenerateRandom.String();

        //act
        _linkTest.SearchUsers(lastName, firstName);

        //assert
        _mockRestClient.VerifyCall<IUserList>($"/user?lastName={lastName}&firstName={firstName}");
    }

    [TestMethod]
    public void MustProvideQueryParametersFromObject() {
        //arrange
        var lastName = GenerateRandom.String();
        var firstName = GenerateRandom.String();

        //act
        var parameters = new { lastName, firstName };
        _linkTest.SearchUsers(parameters);

        //assert
        _mockRestClient.VerifyCall<IUserList>($"/user?lastName={lastName}&firstName={firstName}");
    }

    [TestMethod]
    public void MustUseDefaultValuesForQueryParametersIfNotSupplied() {
        //arrange
        //act
        _linkTest.SearchUsers();

        //assert
        _mockRestClient.VerifyCall<IUserList>($"/user?lastName={_defaultValue}");
    }

    [TestMethod]
    public void MustBeAbleToCheckLinksByConvention() {
        //assert
        Assert.IsTrue(_linkTest.CanGetAllUsers);
        Assert.IsFalse(_linkTest.CanLinkThatDoesNotExist);
    }

    [TestMethod]
    public void MustBeAbleToCheckLinksBySpecificName() {
        //assert
        Assert.IsTrue(_linkTest.LinkCheckGetTemplated);
    }
}