using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Slysoft.RestResource.Client.Tests.Common;
using Slysoft.RestResource.Extensions;
using Slysoft.RestResource.Client.Tests.Common.Extensions;
using TestUtils;

namespace Slysoft.RestResource.Client.Tests;

//note: PUT, DELETE, and PATCH work the same as GET and POST, so not writing tests for them.

[TestClass]
public sealed class LinkTests {
    private Mock<IRestClient> _mockRestClient = null!;
    private ILinkTest _linkTest = null!;
    private ILinkTestAsync _linkTestAsync = null!;
    private readonly int _timeout = GenerateRandom.Int();
    private readonly string _type = GenerateRandom.String();
    private readonly string _defaultValue = GenerateRandom.String();
    private readonly IList<string> _listOfValues = new List<string> { GenerateRandom.String(), GenerateRandom.String(), GenerateRandom.String() };

    [TestInitialize]
    public void SetUp() {
        _mockRestClient = new Mock<IRestClient>();
        var linkTestResource = new Resource()
            .Get("getAllUsers", "/user")
            .Get("getAllUsersWithTimeout", "/user", timeout: _timeout)
            .Get("getAllUsersTemplated", "/user/{id1}/{id2}", templated: true)
            .Query<IUser>("searchUsers", "/user")
                .Parameter(x => x.LastName, type: _type, defaultValue: _defaultValue, listOfValues: _listOfValues)
                .Parameter(x => x.FirstName)
            .EndQuery()
            .Post<IUser>("createUser", "/user")
                .Field(x => x.LastName)
                .Field(x => x.FirstName)
            .EndBody()
            .Post<IUser>("CreateUserWithTimeout", "/user", timeout: _timeout)
                .Field(x => x.LastName)
                .Field(x => x.FirstName)
            .EndBody();

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
    public void GetWithTimeoutMustIncludeItInCall() {
        //act
        _linkTest.GetAllUsersWithTimeout();

        //assert
        _mockRestClient.VerifyCall<IUserList>("/user", timeout: _timeout);
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
        var user1LastName = GenerateRandom.String();
        var user2LastName = GenerateRandom.String();
        var userListResource = TestData.CreateUserListResource(user1LastName, user2LastName);

        var userListAccessor = ResourceAccessorFactory.CreateAccessor<IUserList>(userListResource, _mockRestClient.Object);
        _mockRestClient.SetupCall<IUserList>("/user").Returns(userListAccessor);

        //act
        var userList = _linkTest.GetAllUsers();

        //assert
        Assert.AreEqual(2, userList.Users.Count);
        Assert.AreEqual(user1LastName, userList.Users[0].LastName);
        Assert.AreEqual(user2LastName, userList.Users[1].LastName);
    }

    [TestMethod]
    public void GetAsyncMustReturnAccessor() {
        //arrange
        var user1LastName = GenerateRandom.String();
        var user2LastName = GenerateRandom.String();
        var userListResource = TestData.CreateUserListResource(user1LastName, user2LastName);

        var userListAccessor = ResourceAccessorFactory.CreateAccessor<IUserList>(userListResource, _mockRestClient.Object);
        _mockRestClient.SetupCallAsync<IUserList>("/user").Returns(userListAccessor);

        //act
        var userList = _linkTestAsync.GetAllUsers().Result;

        //assert
        Assert.AreEqual(2, userList.Users.Count);
        Assert.AreEqual(user1LastName, userList.Users[0].LastName);
        Assert.AreEqual(user2LastName, userList.Users[1].LastName);
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

    [TestMethod]
    public void MustBeAbleToGetParameterInfo() {
        //assert
        Assert.AreEqual(_type, _linkTest.SearchLastNameInfo.Type);
        Assert.AreEqual(_defaultValue, _linkTest.SearchLastNameInfo.DefaultValue);
        Assert.AreEqual(_listOfValues[0], _linkTest.SearchLastNameInfo.ListOfValues[0]);
        Assert.AreEqual(_listOfValues[1], _linkTest.SearchLastNameInfo.ListOfValues[1]);
        Assert.AreEqual(_listOfValues[2], _linkTest.SearchLastNameInfo.ListOfValues[2]);
    }

    [TestMethod]
    public void PostMustMakePostCall() {
        //arrange
        var lastName = GenerateRandom.String();
        var firstName = GenerateRandom.String();

        //act
        _linkTest.CreateUser(lastName, firstName);

        //assert
        var expectedBody = new Dictionary<string, object?>() {
            { "lastName", lastName },
            { "firstName", firstName }
        };
        _mockRestClient.VerifyCall<IUser>("/user", verb: "POST", expectedBody);
    }

    [TestMethod]
    public void PostWithTimeoutMustIncludeItInCall() {
        //arrange
        var lastName = GenerateRandom.String();
        var firstName = GenerateRandom.String();

        //act
        _linkTest.CreateUserWithTimeout(lastName, firstName);

        //assert
        var expectedBody = new Dictionary<string, object?>() {
            { "lastName", lastName },
            { "firstName", firstName }
        };
        _mockRestClient.VerifyCall<IUser>("/user", verb: "POST", expectedBody, timeout: _timeout);
    }

    [TestMethod]
    public void PostMustReturnAccessor() {
        //arrange
        var lastName = GenerateRandom.String();
        var firstName = GenerateRandom.String();
        var userResource = new Resource()
            .Data("lastName", lastName)
            .Data("firstName", firstName);

        var userAccessor = ResourceAccessorFactory.CreateAccessor<IUser>(userResource, _mockRestClient.Object);
        _mockRestClient.SetupCall<IUser>("/user", verb: "POST").Returns(userAccessor);

        //act
        var user = _linkTest.CreateUser(lastName, firstName);

        //assert
        Assert.AreEqual(lastName, user.LastName);
        Assert.AreEqual(firstName, user.FirstName);
    }

    [TestMethod]
    public void MustBeAbleToMakePostAsyncCall() {
        //arrange
        var lastName = GenerateRandom.String();
        var firstName = GenerateRandom.String();
        var userResource = new Resource()
            .Data("lastName", lastName)
            .Data("firstName", firstName);

        var userAccessor = ResourceAccessorFactory.CreateAccessor<IUser>(userResource, _mockRestClient.Object);
        _mockRestClient.SetupCallAsync<IUser>("/user", verb: "POST").Returns(userAccessor);

        //act
        var user = _linkTestAsync.CreateUser(lastName, firstName).Result;

        //assert
        Assert.AreEqual(lastName, user.LastName);
        Assert.AreEqual(firstName, user.FirstName);
    }
}