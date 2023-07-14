using SlySoft.RestResource.MappingConfiguration;
using SlySoft.RestResource.Utils;

// ReSharper disable once CheckNamespace
namespace SlySoft.RestResource;

public static class GetExtensions {
    /// <summary>
    /// Create a GET link with no query parameters- user Query to create a link with query parameters
    /// </summary>
    /// <param name="resource">The GET link will be added to this resource</param>
    /// <param name="name">Name of the element- will be converted to camelcase</param>
    /// <param name="href">HREF of the link</param>
    /// <param name="templated">Whether or not the URI is templated</param>
    /// <param name="timeout">A suggested timeout in seconds that the client should use when calling this link</param>
    /// <returns>The resource so further calls can be chained</returns>
    public static Resource Get(this Resource resource, string name, string href, bool templated = false, int timeout = 0) {
        resource.Links.Add(new Link(name.ToCamelCase(), href, templated: templated, timeout: timeout));
        return resource;
    }

    /// <summary>
    /// Create a GET link with configurable query parameters to tell the consumer what values are expected
    /// </summary>
    /// <param name="resource">The GET link will be added to this resource</param>
    /// <param name="name">Name of the link- will be converted to camelcase</param>
    /// <param name="href">HREF of the link</param>
    /// <param name="templated">Whether or not the URI is templated</param>
    /// <param name="timeout">A suggested timeout in seconds that the client should use when calling this link</param>
    /// <returns>A configuration class that will allow configuration of query parameters</returns>
    public static IConfigureQuery Query(this Resource resource, string name, string href, bool templated = false, int timeout = 0) {
        var link = new Link(name.ToCamelCase(), href, templated: templated, timeout: timeout);
        resource.Links.Add(link);
        return new ConfigureQuery(resource, link);
    }

    /// <summary>
    /// Create a GET link with a type safe configurable query parameters to tell the consumer what values are expected
    /// </summary>
    /// <typeparam name="T">The type of object to use for mapping properties</typeparam>
    /// <param name="resource">The GET link will be added to this resource</param>
    /// <param name="name">Name of the link- will be converted to camelcase</param>
    /// <param name="href">HREF of the link</param>
    /// <param name="templated">Whether or not the URI is templated</param>
    /// <param name="timeout">A suggested timeout in seconds that the client should use when calling this link</param>
    /// <returns>A configuration class that will allow configuration of query parameters</returns>
    public static IConfigureQuery<T> Query<T>(this Resource resource, string name, string href, bool templated = false, int timeout = 0) {
        var link = new Link(name.ToCamelCase(), href, templated: templated, timeout: timeout);
        resource.Links.Add(link);
        return new ConfigureQuery<T>(resource, link);
    }

    /// <summary>
    /// Create a GET link creating parameters for all properties based on the generic parameter- individual parameters cannot be configured or excluded
    /// </summary>
    /// <typeparam name="T">The type of object to use for mapping properties</typeparam>
    /// <param name="resource">The GET link will be added to this resource</param>
    /// <param name="name">Name of the link- will be converted to camelcase</param>
    /// <param name="href">HREF of the link</param>
    /// <param name="templated">Whether or not the URI is templated</param>
    /// <param name="timeout">A suggested timeout in seconds that the client should use when calling this link</param>
    /// <returns>The resource so further calls can be chained</returns>
    public static Resource QueryWithAllParameters<T>(this Resource resource, string name, string href, bool templated = false, int timeout = 0) {
        var link = new Link(name.ToCamelCase(), href, templated: templated, timeout: timeout);
        resource.Links.Add(link);
        var configureQuery = new ConfigureQuery<T>(resource, link);
        configureQuery.AllParameters();
        return resource;
    }
}