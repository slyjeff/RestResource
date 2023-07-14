using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Slysoft.RestResource.Client.Extensions;
using Slysoft.RestResource.Client.ResourceDeserializers;

// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace Slysoft.RestResource.Client;

/// <summary>
/// Provide custom serialization
/// </summary>
/// <param name="body">Body to serialized</param>
/// <param name="verb">The verb of this call</param>
/// <returns></returns>
public delegate HttpContent CreateBodyDelegate(IDictionary<string, object?> body, string verb);

/// <summary>
/// Injectable into ResourceAccessors so they can call links
/// </summary>
public interface IRestClient {
    /// <summary>
    /// Set the content type that this client desires from the server
    /// </summary>
    /// <param name="contentTypes">Content type to set in the accept header</param>
    void SetDefaultAcceptHeader(params string[] contentTypes);

    /// <summary>
    /// Set authorization header value for all requests
    /// </summary>
    /// <param name="scheme">The scheme to use for authorization</param>
    /// <param name="value">The credentials containing the authorization information of the user agent</param>
    void SetAuthorizationHeaderValue(string scheme, string value);

    /// <summary>
    /// Timeout (in seconds) to use if the server doesn't specify a timeout- defaults to 100 seconds;
    /// </summary>
    int DefaultTimeout { get; set; }

    /// <summary>
    /// Deserializers that can convert a HttpResponseMessage to a Resource- slysoft.json+hal and slysoft.xml+hal are already registered, but can be removed
    /// </summary>
    IList<IResourceDeserializer> ResourceDeserializers { get; }

    /// <summary>
    /// Method used to serialize content- defaults to converting to json
    /// </summary>
    CreateBodyDelegate CreateBody { get; set; }

    /// <summary>
    /// Make a synchronous web service call, returning the result as the type specified
    /// </summary>
    /// <typeparam name="T">How to return the result. String returns the content as text, Resource returns a resource object, an interface will create a typed accessor to wrap the resource</typeparam>
    /// <param name="url">URL of the call</param>
    /// <param name="verb">Verb of the call</param>
    /// <param name="body">Dictionary of objects to use as query parameters or to serialize for the body of the call</param>
    /// <param name="timeout">Timeout in seconds to wait for the call to complete</param>
    /// <returns>content of the service call</returns>
    T Call<T>(string? url, string? verb = null, IDictionary<string, object?>? body = null, int timeout = 0);

    /// <summary>
    /// Make an asynchronous web service call, returning the result as the type specified
    /// </summary>
    /// <typeparam name="T">How to return the result. String returns the content as text, Resource returns a resource object, an interface will create a typed accessor to wrap the resource</typeparam>
    /// <param name="url">URL of the call</param>
    /// <param name="verb">Verb of the call</param>
    /// <param name="body">Dictionary of objects to use as query parameters or to serialize for the body of the call</param>
    /// <param name="timeout">Timeout in seconds to wait for the call to complete</param>
    /// <returns>content of the service call</returns>
    Task<T> CallAsync<T>(string? url, string? verb = null, IDictionary<string, object?>? body = null, int timeout = 0);
}

/// <summary>
/// Uses HttpClient to make calls from RestCall objects
/// </summary>
public sealed class RestClient : IRestClient {
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Create a RestClient
    /// </summary>
    /// <param name="baseAddress">the base URL of the application, used to resolve all URLS returned from the service</param>
    public RestClient(string baseAddress) {
        _httpClient = new HttpClient {
            BaseAddress = new Uri(baseAddress),
            Timeout = Timeout.InfiniteTimeSpan
        };
        SetDefaultAcceptHeader("application/slysoft.hal+json", "application/hal+json", "application/json");
    }

    /// <summary>
    /// Create a RestClient 
    /// </summary>
    /// <param name="httpClient">HttpClient to use for making network calls</param>
    public RestClient(HttpClient httpClient) {
        _httpClient = httpClient;
        _httpClient.Timeout = Timeout.InfiniteTimeSpan;
        SetDefaultAcceptHeader("application/slysoft.hal+json", "application/hal+json", "application/json");
    }

    /// <summary>
    /// Set the content type that this client desires from the server
    /// </summary>
    /// <param name="contentTypes">Content type to set in the accept header</param>
    public void SetDefaultAcceptHeader(params string[] contentTypes) {
        var acceptHeader = _httpClient.DefaultRequestHeaders.Accept;
        foreach (var contentType in contentTypes) {
            acceptHeader.Add(new MediaTypeWithQualityHeaderValue(contentType));
        }
    }

    /// <summary>
    /// Set authorization header value for all requests
    /// </summary>
    /// <param name="scheme">The scheme to use for authorization</param>
    /// <param name="value">The credentials containing the authorization information of the user agent</param>
    public void SetAuthorizationHeaderValue(string scheme, string value) {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, value);
    }

    /// <summary>
    /// Timeout (in seconds) to use if the server doesn't specify a timeout- defaults to 100 seconds;
    /// </summary>
    public int DefaultTimeout { get; set; } = 100;

    /// <summary>
    /// Deserializers that can convert a HttpResponseMessage to a Resource- slysoft.json+hal and slysoft.xml+hal are already registered, but can be removed
    /// </summary>
    public IList<IResourceDeserializer> ResourceDeserializers { get; } = new List<IResourceDeserializer> {
        new HalJsonDeserializer(),
        new XmlDeserializer(),
    };

    /// <summary>
    /// Method used to serialize content- defaults to converting to json
    /// </summary>
    public CreateBodyDelegate CreateBody { get; set; } = (body, verb) => {
        var contentType = verb == "PATCH" 
            ? "application/merge-patch+json" 
            : "application/json";
        
        var content = JsonConvert.SerializeObject(body);
        
        return new StringContent(content, Encoding.UTF8, contentType);
    };

    /// <summary>
    /// Make a synchronous web service call
    /// </summary>
    /// <typeparam name="T">How to return the result. String returns the content as text, Resource returns a resource object, an interface will create a typed accessor to wrap the resource</typeparam>
    /// <param name="url">URL of the call</param>
    /// <param name="verb">Verb of the call</param>
    /// <param name="body">Dictionary of objects to use as query parameters or to serialize for the body of the call</param>
    /// <param name="timeout">Timeout in seconds to wait for the call to complete</param>
    /// <returns>content of the service call</returns>
    public T Call<T>(string? url = null, string? verb = null, IDictionary<string, object?>? body = null, int timeout = 0) {
        var response = Call(url ?? string.Empty, verb, body, timeout);

        if (!response.IsSuccessStatusCode) {
            throw new ResponseErrorCodeException(response);
        }

        if (typeof(T) == typeof(string)) {
            return (T)(object)response.GetContent().RemoveOuterQuotes();
        }

        var resource = ConvertResponseToResource(response);
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
    /// <param name="body">Dictionary of objects to use as query parameters or to serialize for the body of the call</param>
    /// <param name="timeout">Timeout in seconds to wait for the call to complete</param>
    /// <returns>content of the service call</returns>
    public async Task<T> CallAsync<T>(string? url = null, string? verb = null, IDictionary<string, object?>? body = null, int timeout = 0) {
        var response = await CallAsync(url ?? string.Empty, verb, body, timeout);
        if (!response.IsSuccessStatusCode) {
            throw new ResponseErrorCodeException(response);
        }

        if (typeof(T) == typeof(string)) {
            return (T)(object)(await response.Content.ReadAsStringAsync()).RemoveOuterQuotes();
        }

        var resource = ConvertResponseToResource(response);
        if (typeof(T) == typeof(Resource)) {
            return (T)(object)resource;
        }

        return ResourceAccessorFactory.CreateAccessor<T>(resource, this);
    }

    internal Resource ConvertResponseToResource(HttpResponseMessage response) {
        foreach (var deserializer in ResourceDeserializers) {
            if (deserializer.CanDeserialize(response)) {
                return deserializer.Deserialize(response);
            }
        }

        throw new RestCallException($"content type {response.GetContentType()} not supported by registered deserializers");
    }

    private HttpResponseMessage Call(string url, string? verb = null, IDictionary<string, object?>? body = null, int timeout = 0) {
            var request = CreateRequest(url, verb, body);
            var timeoutToken = CreateTimeoutToken(timeout);
#if NET6_0_OR_GREATER
            return _httpClient.Send(request, timeoutToken);
#else
            return _httpClient.SendAsync(request, timeoutToken).Result;
#endif
    }

    private async Task<HttpResponseMessage> CallAsync(string url, string? verb = null, IDictionary<string, object?>? body = null, int timeout = 0) {
        var request = CreateRequest(url, verb, body);
        var timeoutToken = CreateTimeoutToken(timeout);
        return await _httpClient.SendAsync(request, timeoutToken);
    }

    private HttpRequestMessage CreateRequest(string url, string? verb = null, IDictionary<string, object?>? body = null) {
        verb ??= "GET";
        var request = new HttpRequestMessage(new HttpMethod(verb), url);
        if (body != null && body.Any()) {
            request.Content = CreateBody(body, verb);
        }

        return request;
    }

    private CancellationToken CreateTimeoutToken(int timeout) {
        var timeoutTokenSource = new CancellationTokenSource();
        timeoutTokenSource.CancelAfter(TimeSpan.FromSeconds(timeout == 0 ? DefaultTimeout : timeout));
        return timeoutTokenSource.Token;
    }
}