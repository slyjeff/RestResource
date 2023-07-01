using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Slysoft.RestResource.Client.Extensions;
using Slysoft.RestResource.Extensions;

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
        var link = Resource.GetLink(name);
        if (link == null) {
            throw new CallLinkException($"Link {name} not found in resource.");
        }

        try {
            return RestClient.Call<T>(link.Href);
        } catch (Exception e) {
            throw new CallLinkException($"Error calling link {name}.", e);
        }
    }

    public async Task<T> CallRestLinkAsync<T>(string name, IDictionary<string, object?> parameters) {
        var link = Resource.GetLink(name);
        if (link == null) {
            throw new CallLinkException($"Link {name} not found in resource.");
        }

        try {
            return await RestClient.CallAsync<T>(link.Href);
        } catch (Exception e) {
            throw new CallLinkException($"Error calling link {name}.", e);
        }
    }
}