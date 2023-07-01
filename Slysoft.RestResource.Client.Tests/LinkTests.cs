using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Slysoft.RestResource.Client.Tests.Common;
using Slysoft.RestResource.Extensions;

namespace Slysoft.RestResource.Client.Tests;

[TestClass]
public sealed class LinkTests {
    private Mock<IRestClient> _mockRestClient = null!;

    [TestInitialize]
    public void SetUp() {
        _mockRestClient = new Mock<IRestClient>();
    }

    private void VerifyCall<T>(string url, string? verb = null, string? body = null, int timeout = 0) {
        _mockRestClient.Verify(x => x.Call<T>(It.Is<string>(a => a == url), It.Is<string?>(b => b == verb), It.Is<string?>(c => c == body), It.Is<int>(d => d == timeout)), Times.Once);
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
        VerifyCall<IUser>("/user");
    }
}