using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Slysoft.RestResource.Client.Extensions;
using Slysoft.RestResource.Extensions;

// ReSharper disable UnusedMember.Global

namespace Slysoft.RestResource.Client.Accessors;

public interface IResourceAccessor {
    Resource Resource { get; }
    T CallRestLink<T>(string name, IDictionary<string, object?> parameters);
    Task<T> CallRestLinkAsync<T>(string name, IDictionary<string, object?> parameters);
}

public abstract class ResourceAccessor : IResourceAccessor, INotifyPropertyChanged {
    private readonly IDictionary<string, object?> _cachedData = new Dictionary<string, object?>();
    private readonly IDictionary<string, object?> _updateValues = new Dictionary<string, object?>();

    protected ResourceAccessor(Resource resource, IRestClient restClient) {
        Resource = resource;
        RestClient = restClient;
    }

    public Resource Resource { get; }
    internal IRestClient RestClient { get; }

    protected T? GetData<T>(string name) {
        if (_updateValues.TryGetValue(name, out var value)) {
            return (T?)value;
        }

        if (_cachedData.TryGetValue(name, out value)) {
            return (T?)value;
        }

        _cachedData[name] = Resource.GetData<T>(name, RestClient);

        return (T?)_cachedData[name];
    }

    protected void SetData<T>(string name, T? value) {
        _updateValues[name] = value;
        OnPropertyChanged(name);
    }

    protected IParameterInfo GetParameterInfo(string linkName, string parameterName) {
        var link = Resource.GetLink(linkName);
        return link == null 
            ? new LinkParameterInfo() 
            : new LinkParameterInfo(link.GetParameter(parameterName));
    }

    protected bool LinkCheck(string name) {
        return Resource.GetLink(name) != null;
    }

    public T CallRestLink<T>(string name, IDictionary<string, object?> parameters) {
        var link = CreateLink(name, parameters);

        try {
            return RestClient.Call<T>(link.Url, verb: link.Verb, body: link.Body, timeout: link.Timeout);
        } catch (ResponseErrorCodeException) {
            throw; //rethrow ResponseErrorCodeException because it contains information the caller may be interested in- the code and the message from the server
        } catch (Exception e) {
            throw new CallLinkException($"Error calling link {name}.", e);
        }
    }

    public async Task<T> CallRestLinkAsync<T>(string name, IDictionary<string, object?> parameters) {
        var link = CreateLink(name, parameters);

        try {
            return await RestClient.CallAsync<T>(link.Url, verb: link.Verb, body: link.Body);
        } catch (ResponseErrorCodeException) {
            throw; //rethrow ResponseErrorCodeException because it contains information the caller may be interested in- the code and the message from the server
        } catch (Exception e) {
            throw new CallLinkException($"Error calling link {name}.", e);
        }
    }

    private readonly struct CallableLink {
        public CallableLink(string url, string verb, IDictionary<string, object?>? body = null, int timeout = 0) {
            Url = url;
            Verb = verb;
            Body = body;
            Timeout = timeout;
        }
        public readonly string Url;
        public readonly string Verb;
        public readonly IDictionary<string, object?>? Body;
        public readonly int Timeout;
    }

    private CallableLink CreateLink(string name, IDictionary<string, object?> parameters) {
        var link = Resource.GetLink(name) ?? throw new CallLinkException($"Link {name} not found in resource.");

        var url = link.Templated ? link.Href.ResolveTemplatedUrl(parameters) : link.Href;

        var linkParameters = GetLinkParameters(link.Parameters, parameters);

        if (link.Verb.SupportsQueryParameters()) {
            url = url.AppendQueryParameters(linkParameters);
        }

        if (!link.Verb.SupportsBody()) {
            return new CallableLink(url, link.Verb, timeout: link.Timeout);
        }

        var body = GetLinkParameters(link.Parameters, parameters);
        return new CallableLink(url, link.Verb, body, timeout: link.Timeout);
    }

    private static IDictionary<string, object?> GetLinkParameters(IEnumerable<LinkParameter> linkParameters, IDictionary<string, object?> parameters) {
        var dictionary = new Dictionary<string, object?>();
        foreach (var linkParameter in linkParameters) {
            var value = parameters.GetValue(linkParameter.Name);
            if (value == null) {
                if (linkParameter.DefaultValue == null) {
                    continue;
                }

                value = linkParameter.DefaultValue;
            }

            dictionary[linkParameter.Name] = value;
        }
        return dictionary;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}