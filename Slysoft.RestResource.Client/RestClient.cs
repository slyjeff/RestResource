using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Slysoft.RestResource.Client.Extensions;

namespace Slysoft.RestResource.Client;

/// <summary>
/// Injectable into ResourceAccessors so they can call links
/// </summary>
public interface IRestClient {
    /// <summary>
    /// Make a synchronous web service call, returning the result as the type specified
    /// </summary>
    /// <typeparam name="T">How to return the result. String returns the content as text, Resource returns a resource object, an interface will create a typed accessor to wrap the resource</typeparam>
    /// <param name="url">URL of the call</param>
    /// <param name="verb">Verb of the call</param>
    /// <param name="body">Body of the call</param>
    /// <param name="timeout">Timeout in seconds to wait for the call to complete</param>
    /// <returns>content of the service call</returns>
    T Call<T>(string url, string? verb = null, string? body = null, int timeout = 0);

    /// <summary>
    /// Make an asynchronous web service call, returning the result as the type specified
    /// </summary>
    /// <typeparam name="T">How to return the result. String returns the content as text, Resource returns a resource object, an interface will create a typed accessor to wrap the resource</typeparam>
    /// <param name="url">URL of the call</param>
    /// <param name="verb">Verb of the call</param>
    /// <param name="body">Body of the call</param>
    /// <param name="timeout">Timeout in seconds to wait for the call to complete</param>
    /// <returns>content of the service call</returns>
    Task<T> CallAsync<T>(string url, string? verb = null, string? body = null, int timeout = 0);
}

/// <summary>
/// Uses HttpClient to make calls from RestCall objects
/// </summary>
public sealed class RestClient : IRestClient {
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;

    public RestClient(string baseUrl) {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// Make a synchronous web service call
    /// </summary>
    /// <typeparam name="T">How to return the result. String returns the content as text, Resource returns a resource object, an interface will create a typed accessor to wrap the resource</typeparam>
    /// <param name="url">URL of the call</param>
    /// <param name="verb">Verb of the call</param>
    /// <param name="body">Body of the call</param>
    /// <param name="timeout">Timeout in seconds to wait for the call to complete</param>
    /// <returns>content of the service call</returns>
    public T Call<T>(string url, string? verb = null, string? body = null, int timeout = 0) {
        var response = Call(url, verb, body, timeout);
        if (typeof(T) == typeof(string)) {
            return (T)(object)response.GetContent();
        }

        var resource = response.ToResource();
        if (typeof(T) == typeof(Resource)) {
            return (T)(object)resource;
        }

        return ResourceAccessorFactory.CreateAccessor<T>(resource, this);
    }

    /// <summary>
    /// Make an asynchronous web service call
    /// </summary>
    /// <typeparam name="T">How to return the result. String returns the content as text, Resource returns a resource object, an interface will create a typed accessor to wrap the resource</typeparam>
    /// <param name="url">URL of the call</param>
    /// <param name="verb">Verb of the call</param>
    /// <param name="body">Body of the call</param>
    /// <param name="timeout">Timeout in seconds to wait for the call to complete</param>
    /// <returns>content of the service call</returns>
    public async Task<T> CallAsync<T>(string url, string? verb = null, string? body = null, int timeout = 0) {
        var response = await CallAsync(url, verb, body, timeout);
        if (typeof(T) == typeof(string)) {
            return (T)(object)await response.Content.ReadAsStringAsync();
        }

        var resource = await response.ToResourceAsync();
        if (typeof(T) == typeof(Resource)) {
            return (T)(object)resource;
        }

        return ResourceAccessorFactory.CreateAccessor<T>(resource, this);
    }

    private HttpResponseMessage Call(string url, string? verb = null, string? body = null, int timeout = 0) {
        var originalTimeout = _httpClient.Timeout;
        try {
            if (timeout > 0) {
                _httpClient.Timeout = new TimeSpan(0, 0, 0, timeout);
            }

            var request = CreateRequest(url, verb, body);
#if NET6_0_OR_GREATER
            return _httpClient.Send(request);
#else
            return _httpClient.SendAsync(request).Result;
#endif
        } finally {
            _httpClient.Timeout = originalTimeout;
        }
    }

    private async Task<HttpResponseMessage> CallAsync(string url, string? verb = null, string? body = null, int timeout = 0) {
        var originalTimeout = _httpClient.Timeout;
        try {
            if (timeout > 0) {
                _httpClient.Timeout = new TimeSpan(0, 0, 0, timeout);
            }

            var request = CreateRequest(url, verb, body);
            return await _httpClient.SendAsync(request);
        } finally {
            _httpClient.Timeout = originalTimeout;
        }
    }

    private HttpRequestMessage CreateRequest(string url, string? verb = null, string? body = null) {
        url = _baseUrl.AppendUrl(url);
        if (string.IsNullOrEmpty(verb)) {
            verb = "GET";
        }

        var request = new HttpRequestMessage(new HttpMethod(verb), url);
        if (!string.IsNullOrEmpty(body)) {
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
        }

        return request;
    }
}