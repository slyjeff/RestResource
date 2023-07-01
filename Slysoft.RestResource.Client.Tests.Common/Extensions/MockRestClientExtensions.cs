using Moq;

namespace Slysoft.RestResource.Client.Tests.Common.Extensions; 

public static class MockRestClientExtensions {
    public static CallSetup<T> SetupCall<T>(this Mock<IRestClient> mockRestMock, string url, string? verb = null, string? body = null, int timeout = 0) {
        return new CallSetup<T>(mockRestMock, url, verb, body, timeout);
    }

    //private void SetupAsyncCall<T>(T accessor, string url, string? verb = null, string? body = null, int timeout = 0) {
    //    _mockRestClient.Setup(x => x.CallAsync<T>(It.Is<string>(a => a == url), It.Is<string?>(b => b == verb), It.Is<string?>(c => c == body), It.Is<int>(d => d == timeout))).ReturnsAsync(accessor);
    //}

    public static void VerifyCall<T>(this Mock<IRestClient> mockRestMock, string url, string? verb = null, string? body = null, int timeout = 0) {
        mockRestMock.Verify(x => x.Call<T>(It.Is<string>(a => a == url), It.Is<string?>(b => b == verb), It.Is<string?>(c => c == body), It.Is<int>(d => d == timeout)), Times.Once);
    }

    public static void VerifyAsyncCall<T>(this Mock<IRestClient> mockRestMock, string url, string? verb = null, string? body = null, int timeout = 0) {
        mockRestMock.Verify(x => x.CallAsync<T>(It.Is<string>(a => a == url), It.Is<string?>(b => b == verb), It.Is<string?>(c => c == body), It.Is<int>(d => d == timeout)), Times.Once);
    }
}

public class CallSetup<T> {
    private readonly Mock<IRestClient> _mockRestClient;
    private readonly string _url;
    private readonly string? _verb;
    private readonly string? _body;
    private readonly int _timeout;

    public CallSetup(Mock<IRestClient> mockRestClient, string url, string? verb = null, string? body = null, int timeout = 0) {
        _mockRestClient = mockRestClient;
        _url = url;
        _verb = verb;
        _body = body;
        _timeout = timeout;
    }

    public void Returns(T accessor) {
        _mockRestClient.Setup(x => x.Call<T>(It.Is<string>(a => a == _url), It.Is<string?>(b => b == _verb), It.Is<string?>(c => c == _body), It.Is<int>(d => d == _timeout))).Returns(accessor);
    }
}