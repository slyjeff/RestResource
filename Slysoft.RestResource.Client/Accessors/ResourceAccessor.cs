using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Slysoft.RestResource.Client.Extensions;
using Slysoft.RestResource.Extensions;

// ReSharper disable UnusedMember.Global

namespace Slysoft.RestResource.Client.Accessors;

/// Inheriting from IResourceAccessor allows a resource accessor to report changes to properties, whether the resource has changes, and revert changes
public interface IEditableResource : INotifyPropertyChanged {
    /// <summary>
    /// Whether changes have been made to any properties in the resource
    /// </summary>
    bool IsChanged { get; }

    /// <summary>
    /// Revert changes back to the original values received from the service call
    /// </summary>
    void RejectChanges();
}


/// <summary>
/// Inheriting from IResourceAccessor allows a resource accessor to use the raw resource and execute calls- use in situations when you do not know the structure of the resource in advance
/// </summary>
public interface IResourceAccessor {
    /// <summary>
    /// Deserialized Resource received from a call
    /// </summary>
    Resource Resource { get; }

    /// <summary>
    /// Make a REST call passing in a link name and parameters as a dictionary
    /// </summary>
    /// <typeparam name="T">How to return the result. String returns the content as text, Resource returns a resource object, an interface will create a typed accessor to wrap the resource</typeparam>
    /// <param name="name">Name of the link- must be a link that exists in the Resource</param>
    /// <param name="parameters">Dictionary containing name/value pairs of the values to use when calling the link for the body/query parameters</param>
    /// <returns>content of the service call</returns>
    T CallRestLink<T>(string name, IDictionary<string, object?> parameters);

    /// <summary>
    /// Make an asynchronous REST call passing in a link name and parameters as a dictionary
    /// </summary>
    /// <typeparam name="T">How to return the result. String returns the content as text, Resource returns a resource object, an interface will create a typed accessor to wrap the resource</typeparam>
    /// <param name="name">Name of the link- must be a link that exists in the Resource</param>
    /// <param name="parameters">Dictionary containing name/value pairs of the values to use when calling the link for the body/query parameters</param>
    /// <returns>content of the service call</returns>
    Task<T> CallRestLinkAsync<T>(string name, IDictionary<string, object?> parameters);
}

public abstract class ResourceAccessor : IResourceAccessor, IEditableResource {
    private readonly IDictionary<string, object?> _cachedData = new Dictionary<string, object?>();
    private readonly IDictionary<string, object?> _updateValues = new Dictionary<string, object?>();

    protected ResourceAccessor(Resource resource, IRestClient restClient) {
        Resource = resource;
        RestClient = restClient;
    }

    public Resource Resource { get; }
    public bool IsChanged => _updateValues.Any();
    public void RejectChanges() {
        var propertiesToBeReverted = _updateValues.Keys.ToList();
        _updateValues.Clear();
        foreach (var revertedProperty in propertiesToBeReverted) {
            OnPropertyChanged(revertedProperty);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    internal IRestClient RestClient { get; }

    protected T? GetData<T>(string name) {
        if (_updateValues.TryGetValue(name, out var value)) {
            return (T?)value;
        }

        return GetOriginalData<T>(name);
    }

    protected void SetData<T>(string name, T? value) {
        //if this is the original value, remove our Update Value
        var originalValue = GetOriginalData<T>(name);
        if (ValuesAreEqual(originalValue, value)) {
            if (_updateValues.ContainsKey(name)) {
                _updateValues.Remove(name);
            }

            OnPropertyChanged(name);
            return;
        }
        
        _updateValues[name] = value;
        OnPropertyChanged(name);
    }

    private T? GetOriginalData<T>(string name) {
        if (_cachedData.TryGetValue(name, out var value)) {
            return (T?)value;
        }

        _cachedData[name] = Resource.GetData<T>(name, RestClient);

        return (T?)_cachedData[name];
    }

    private static bool ValuesAreEqual<T>(T? v1, T? v2) {
        if (v1 == null && v2 == null) {
            return true;
        }

        return v1 != null && v1.Equals(v2);
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
}