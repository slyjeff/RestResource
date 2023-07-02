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

public delegate HttpContent CreateBodyDelegate(IDictionary<string, object?> body);

/// <summary>
/// Injectable into ResourceAccessors so they can call links
/// </summary>
public interface IRestClient {
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
    /// <param name="inputItems">Dictionary of objects to use as query parameters or to serialize for the body of the call</param>
    /// <param name="timeout">Timeout in seconds to wait for the call to complete</param>
    /// <returns>content of the service call</returns>
    T Call<T>(string url, string? verb = null, IDictionary<string, object?>? inputItems = null, int timeout = 0);

    /// <summary>
    /// Make an asynchronous web service call, returning the result as the type specified
    /// </summary>
    /// <typeparam name="T">How to return the result. String returns the content as text, Resource returns a resource object, an interface will create a typed accessor to wrap the resource</typeparam>
    /// <param name="url">URL of the call</param>
    /// <param name="verb">Verb of the call</param>
    /// <param name="inputItems">Dictionary of objects to use as query parameters or to serialize for the body of the call</param>
    /// <param name="timeout">Timeout in seconds to wait for the call to complete</param>
    /// <returns>content of the service call</returns>
    Task<T> CallAsync<T>(string url, string? verb = null, IDictionary<string, object?>? inputItems = null, int timeout = 0);
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
        _httpClient.Timeout = Timeout.InfiniteTimeSpan;
        _httpClient.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
    public CreateBodyDelegate CreateBody { get; set; } = body => {
        var content = JsonConvert.SerializeObject(body);
        return new StringContent(content, Encoding.UTF8, "application/json");
    };

    /// <summary>
    /// Make a synchronous web service call
    /// </summary>
    /// <typeparam name="T">How to return the result. String returns the content as text, Resource returns a resource object, an interface will create a typed accessor to wrap the resource</typeparam>
    /// <param name="url">URL of the call</param>
    /// <param name="verb">Verb of the call</param>
    /// <param name="inputItems">Dictionary of objects to use as query parameters or to serialize for the body of the call</param>
    /// <param name="timeout">Timeout in seconds to wait for the call to complete</param>
    /// <returns>content of the service call</returns>
    public T Call<T>(string url, string? verb = null, IDictionary<string, object?>? inputItems = null, int timeout = 0) {
        var response = Call(url, verb, inputItems, timeout);

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
    /// <param name="inputItems">Dictionary of objects to use as query parameters or to serialize for the body of the call</param>
    /// <param name="timeout">Timeout in seconds to wait for the call to complete</param>
    /// <returns>content of the service call</returns>
    public async Task<T> CallAsync<T>(string url, string? verb = null, IDictionary<string, object?>? inputItems = null, int timeout = 0) {
        var response = await CallAsync(url, verb, inputItems, timeout);
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

    private HttpResponseMessage Call(string url, string? verb = null, IDictionary<string, object?>? inputItems = null, int timeout = 0) {
            var request = CreateRequest(url, verb, inputItems);
            var timeoutToken = CreateTimeoutToken(timeout);
#if NET6_0_OR_GREATER
            return _httpClient.Send(request, timeoutToken);
#else
            return _httpClient.SendAsync(request, timeoutToken).Result;
#endif
    }

    private async Task<HttpResponseMessage> CallAsync(string url, string? verb = null, IDictionary<string, object?>? inputItems = null, int timeout = 0) {
        var request = CreateRequest(url, verb, inputItems);
        var timeoutToken = CreateTimeoutToken(timeout);
        return await _httpClient.SendAsync(request, timeoutToken);
    }

    private HttpRequestMessage CreateRequest(string url, string? verb = null, IDictionary<string, object?>? inputItems = null) {
        url = _baseUrl.AppendUrl(url);
        if (string.IsNullOrEmpty(verb)) {
            verb = "GET";
        }

        if (verb == "GET") {
            url = url.AppendQueryParameters(inputItems);
        }

        var request = new HttpRequestMessage(new HttpMethod(verb), url);
        if (verb != "GET" && verb != "DELETE" && inputItems != null && inputItems.Any()) {
            request.Content = CreateBody(inputItems);
        }

        return request;
    }

    private CancellationToken CreateTimeoutToken(int timeout) {
        var timeoutTokenSource = new CancellationTokenSource();
        timeoutTokenSource.CancelAfter(TimeSpan.FromSeconds(timeout == 0 ? DefaultTimeout : timeout));
        return timeoutTokenSource.Token;
    }
}