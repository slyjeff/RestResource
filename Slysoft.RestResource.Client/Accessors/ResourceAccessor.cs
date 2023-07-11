using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slysoft.RestResource.Client.Extensions;
using Slysoft.RestResource.Extensions;

// ReSharper disable UnusedMember.Global

namespace Slysoft.RestResource.Client.Accessors;

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

public abstract class ResourceAccessor : Accessor, IResourceAccessor {
    protected ResourceAccessor(Resource resource, IRestClient restClient) {
        Resource = resource;
        RestClient = restClient;
    }

    public Resource Resource { get; }
    internal IRestClient RestClient { get; }

    protected override T? CreateData<T>(string name) where T : default {
        return Resource.GetData<T>(name, RestClient);
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


        if (link.Verb.SupportsQueryParameters()) {
            var linkParameters = GetLinkParameters(link.Parameters, parameters);
            url = url.AppendQueryParameters(linkParameters);
        }

        if (!link.Verb.SupportsBody()) {
            return new CallableLink(url, link.Verb, timeout: link.Timeout);
        }

        var body = parameters.Any()
            ? GetLinkParameters(link.Parameters, parameters)
            : GetParametersFromThis(link);
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

    private IDictionary<string, object?> GetParametersFromThis(Link link) {
        return link.Verb == "PATCH"
            ? GetChangedParametersFromThis(link.Parameters)
            : GetAllParametersFromThis(link.Parameters);
    }

    private IDictionary<string, object?> GetChangedParametersFromThis(IEnumerable<LinkParameter> parameters) {
        var dictionary = new Dictionary<string, object?>();
        foreach (var parameter in parameters) {
            foreach (var item in UpdateValues) {
                if (item.Key.Equals(parameter.Name, StringComparison.CurrentCultureIgnoreCase)) {
                    dictionary[parameter.Name] = item.Value;
                }
            }

        }
        return dictionary;
    }

    private IDictionary<string, object?> GetAllParametersFromThis(IEnumerable<LinkParameter> parameters) {
        var dictionary = new Dictionary<string, object?>();
        foreach (var parameter in parameters) {
            object? value = parameter.DefaultValue;
            foreach (var property in GetType().GetAllProperties()) {
                if (property.Name.Equals(parameter.Name, StringComparison.CurrentCultureIgnoreCase)) {
                    value = property.GetValue(this);
                    break;
                }
            }

            dictionary[parameter.Name] = value;
        }
        return dictionary;
    }
}