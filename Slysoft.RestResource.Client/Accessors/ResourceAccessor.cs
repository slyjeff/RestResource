using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Slysoft.RestResource.Client.Extensions;
using Slysoft.RestResource.Extensions;

// ReSharper disable UnusedMember.Global

namespace Slysoft.RestResource.Client.Accessors;

public abstract class ResourceAccessor {
    private readonly IDictionary<string, object?> _cachedData = new Dictionary<string, object?>();

    protected ResourceAccessor(Resource resource, IRestClient restClient) {
        Resource = resource;
        RestClient = restClient;
    }

    internal Resource Resource { get; }
    internal IRestClient RestClient { get; }

    protected T? GetData<T>(string name) {
        if (_cachedData.TryGetValue(name, out var value)) {
            return (T?)value;
        }

        _cachedData[name] = Resource.GetData<T>(name, RestClient);

        return (T?)_cachedData[name];
    }

    public T CallRestLink<T>(string name, IDictionary<string, object?> parameters) {
        var link = CreateLink(name, parameters);

        try {
            return RestClient.Call<T>(link.Url, body: link.Body);
        } catch (ResponseErrorCodeException) {
            throw; //rethrow ResponseErrorCodeException because it contains information the caller may be interested in- the code and the message from the server
        } catch (Exception e) {
            throw new CallLinkException($"Error calling link {name}.", e);
        }
    }

    public async Task<T> CallRestLinkAsync<T>(string name, IDictionary<string, object?> parameters) {
        var link = CreateLink(name, parameters);

        try {
            return await RestClient.CallAsync<T>(link.Url, body: link.Body);
        } catch (ResponseErrorCodeException) {
            throw; //rethrow ResponseErrorCodeException because it contains information the caller may be interested in- the code and the message from the server
        } catch (Exception e) {
            throw new CallLinkException($"Error calling link {name}.", e);
        }
    }

    private readonly struct CallableLink {
        public CallableLink(string url, IDictionary<string, object?>? body = null) {
            Url = url;
            Body = body;
        }
        public readonly string Url;
        public readonly IDictionary<string, object?>? Body;
    }

    private CallableLink CreateLink(string name, IDictionary<string, object?> parameters) {
        var link = Resource.GetLink(name) ?? throw new CallLinkException($"Link {name} not found in resource.");

        var url = link.Templated ? link.Href.ResolveTemplatedUrl(parameters) : link.Href;

        var inputElements = GetInputElements(link.InputItems, parameters);

        if (link.Verb.SupportsQueryParameters()) {
            url = url.AppendQueryParameters(inputElements);
        }

        if (!link.Verb.SupportsBody()) {
            return new CallableLink(url);
        }

        var body = GetInputElements(link.InputItems, parameters);
        return new CallableLink(url, body);
    }

    private static IDictionary<string, object?> GetInputElements(IList<InputItem> inputItems, IDictionary<string, object?> parameters) {
        var dictionary = new Dictionary<string, object?>();
        foreach (var inputItem in inputItems) {
            var value = parameters.GetValue(inputItem.Name);
            if (value == null) {
                continue;
            }

            dictionary[inputItem.Name] = value;
        }
        return dictionary;
    }
}