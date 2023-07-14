using SlySoft.RestResource.MappingConfiguration;
using SlySoft.RestResource.Utils;

// ReSharper disable once CheckNamespace
namespace SlySoft.RestResource;

public static class PutExtensions {
    /// <summary>
    /// Create a PUT link with configurable query parameters to tell the consumer what values are expected
    /// </summary>
    /// <param name="resource">The PUT link will be added to this resource</param>
    /// <param name="name">Name of the link- will be converted to camelcase</param>
    /// <param name="href">HREF of the link</param>
    /// <param name="templated">Whether or not the URI is templated</param>
    /// <param name="timeout">A suggested timeout in seconds that the client should use when calling this link</param>
    /// <returns>A configuration class that will allow configuration of fields</returns>
    public static IConfigureBody Put(this Resource resource, string name, string href, bool templated = false, int timeout = 0) {
        var link = new Link(name.ToCamelCase(), href, verb: "PUT", templated: templated, timeout: timeout);
        resource.Links.Add(link);
        return new ConfigureBody(resource, link);
    }

    /// <summary>
    /// Create a PUT with a type safe configurable query parameters to tell the consumer what values are expected
    /// </summary>
    /// <typeparam name="T">The type of object to use for mapping properties</typeparam>
    /// <param name="resource">The PUT link will be added to this resource</param>
    /// <param name="name">Name of the link- will be converted to camelcase</param>
    /// <param name="href">HREF of the link</param>
    /// <param name="templated">Whether or not the URI is templated</param>
    /// <param name="timeout">A suggested timeout in seconds that the client should use when calling this link</param>
    /// <returns>A configuration class that will allow configuration of body fields</returns>
    public static IConfigureBody<T> Put<T>(this Resource resource, string name, string href, bool templated = false, int timeout = 0) {
        var link = new Link(name.ToCamelCase(), href, verb: "PUT", templated: templated, timeout: timeout);
        resource.Links.Add(link);
        return new ConfigureBody<T>(resource, link);
    }

    /// <summary>
    /// Create a PUT link creating fields for all fields properties on the generic parameter- individual field cannot be configured or excluded
    /// </summary>
    /// <typeparam name="T">The type of object to use for mapping properties</typeparam>
    /// <param name="resource">The PUT link will be added to this resource</param>
    /// <param name="name">Name of the link- will be converted to camelcase</param>
    /// <param name="href">HREF of the link</param>
    /// <param name="templated">Whether or not the URI is templated</param>
    /// <param name="timeout">A suggested timeout in seconds that the client should use when calling this link</param>
    /// <returns>The resource so further calls can be chained</returns>
    public static Resource PutWithAllFields<T>(this Resource resource, string name, string href, bool templated = false, int timeout = 0) {
        var link = new Link(name.ToCamelCase(), href, verb: "PUT", templated: templated, timeout: timeout);
        resource.Links.Add(link);
        var configBody = new ConfigureBody<T>(resource, link);
        configBody.AllFields();
        return resource;
    }
}