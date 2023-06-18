using Slysoft.RestResource.MappingConfiguration;
using Slysoft.RestResource.Utils;

namespace Slysoft.RestResource.Extensions; 

public static class PostExtensions {
    /// <summary>
    /// Create a POST link with configurable query parameters to tell the consumer what values are expected
    /// </summary>
    /// <param name="resource">The POST link will be added to this resource</param>
    /// <param name="name">Name of the link- will be converted to camelcase</param>
    /// <param name="href">HREF of the link</param>
    /// <param name="templated">Whether or not the URI is templated</param>
    /// <returns>A configuration class that will allow configuration of fields</returns>
    public static IConfigureBody Post(this Resource resource, string name, string href, bool templated = false) {
        var link = new Link(name.ToCamelCase(), href, verb: "POST", templated: templated);
        resource.Links.Add(link);
        return new ConfigureBody(resource, link);
    }

    /// <summary>
    /// Create a GET POST with a type safe configurable query parameters to tell the consumer what values are expected
    /// </summary>
    /// <typeparam name="T">The type of object to use for mapping properties</typeparam>
    /// <param name="resource">The GET link will be added to this resource</param>
    /// <param name="name">Name of the link- will be converted to camelcase</param>
    /// <param name="href">HREF of the link</param>
    /// <param name="templated">Whether or not the URI is templated</param>
    /// <returns>A configuration class that will allow configuration of body fields</returns>
    public static IConfigureBody<T> Post<T>(this Resource resource, string name, string href, bool templated = false) {
        var link = new Link(name.ToCamelCase(), href, verb: "POST", templated: templated);
        resource.Links.Add(link);
        return new ConfigureBody<T>(resource, link);
    }

    /// <summary>
    /// Create a POST link creating fields for all fields properties on the generic parameter- individual field cannot be configured or excluded
    /// </summary>
    /// <typeparam name="T">The type of object to use for mapping properties</typeparam>
    /// <param name="resource">The POST link will be added to this resource</param>
    /// <param name="name">Name of the link- will be converted to camelcase</param>
    /// <param name="href">HREF of the link</param>
    /// <param name="templated">Whether or not the URI is templated</param>
    /// <returns>The resource so further calls can be chained</returns>
    public static Resource PostWithAllFields<T>(this Resource resource, string name, string href, bool templated = false) {
        var link = new Link(name.ToCamelCase(), href, verb: "POST", templated: templated);
        resource.Links.Add(link);
        var configBody = new ConfigureBody<T>(resource, link);
        configBody.MapAll();
        return resource;
    }
}