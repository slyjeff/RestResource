using System.Collections.Generic;
using Moq;

namespace Slysoft.RestResource.Client.Tests.Common.Extensions; 

public static class MockRestClientExtensions {
    public static CallSetup<T> SetupCall<T>(this Mock<IRestClient> mockRestMock, string url, string? verb = null, IDictionary<string, object?>? body = null, int timeout = 0) {
        verb ??= "GET";
        return new CallSetup<T>(mockRestMock, url, verb, body, timeout);
    }

    public static CallAsyncSetup<T> SetupCallAsync<T>(this Mock<IRestClient> mockRestMock, string url, string? verb = null, IDictionary<string, object?>? body = null, int timeout = 0) {
        verb ??= "GET";
        return new CallAsyncSetup<T>(mockRestMock, url, verb, body, timeout);
    }

    public static void VerifyCall<T>(this Mock<IRestClient> mockRestMock, string url, string? verb = null, IDictionary<string, object?>? body = null, int timeout = 0) {
        verb ??= "GET";
        mockRestMock.Verify(x => x.Call<T>(url, verb, It.Is<IDictionary<string, object?>?>(c => body.Verify(c)), timeout), Times.Once);
    }

    public static void VerifyAsyncCall<T>(this Mock<IRestClient> mockRestMock, string url, string? verb = null, IDictionary<string, object?>? body = null, int timeout = 0) {
        verb ??= "GET";
        mockRestMock.Verify(x => x.CallAsync<T>(url, verb, It.Is<IDictionary<string, object?>?>(c => body.Verify(c)), timeout), Times.Once);
    }
}

public class CallSetup<T> {
    private readonly Mock<IRestClient> _mockRestClient;
    private readonly string _url;
    private readonly string _verb;
    private readonly IDictionary<string, object?>? _body;
    private readonly int _timeout;

    public CallSetup(Mock<IRestClient> mockRestClient, string url, string verb, IDictionary<string, object?>? body = null, int timeout = 0) {
        _mockRestClient = mockRestClient;
        _url = url;
        _verb = verb;
        _body = body;
        _timeout = timeout;
    }

    public void Returns(T accessor) {
        _mockRestClient.Setup(x => x.Call<T>(_url, _verb, It.Is<IDictionary<string, object?>?>(c => _body.Verify(c)), _timeout)).Returns(accessor);
    }
}

public class CallAsyncSetup<T> {
    private readonly Mock<IRestClient> _mockRestClient;
    private readonly string _url;
    private readonly string _verb;
    private readonly IDictionary<string, object?>? _body;
    private readonly int _timeout;

    public CallAsyncSetup(Mock<IRestClient> mockRestClient, string url, string verb, IDictionary<string, object?>? body = null, int timeout = 0) {
        _mockRestClient = mockRestClient;
        _url = url;
        _verb = verb;
        _body = body;
        _timeout = timeout;
    }

    public void Returns(T accessor) {
        _mockRestClient.Setup(x => x.CallAsync<T>(_url, _verb, It.Is<IDictionary<string, object?>?>(c => _body.Verify(c)), _timeout)).ReturnsAsync(accessor);
    }
}

internal static class VerifyExtensions {
    public static bool Verify(this IDictionary<string, object?>? expected, IDictionary<string, object?>? actual) {
        if (expected == null) {
            return true;
        }

        if (actual == null) {
            return false;
        }

        if (expected.Count != actual.Count) {
            return false;
        }

        foreach (var item in expected) {
            if (!actual.ContainsKey(item.Key)) {
                return false;
            }

            if (actual[item.Key] != item.Value) {
                return false;
            }
        }

        return true;
    }
}
