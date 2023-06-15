using RestResource.Utils;

namespace RestResource.Extensions; 

public static class DataExtensions {
    /// <summary>
    /// Assign the URI of the resource
    /// </summary>
    /// <param name="resource">The URI will be added to this resource</param>
    /// <param name="uri">URI of the resource that will be used to construct a "self" link</param>
    /// <returns>The resource so further calls can be chained</returns>
    public static Resource Uri(this Resource resource, string uri) {
        resource.Uri = uri;
        return resource;
    }

    /// <summary>
    /// Add a data element to the resource
    /// </summary>
    /// <param name="resource">The data will be added to this resource</param>
    /// <param name="name">Name of the element- will be converted to camelcase</param>
    /// <param name="value">Value to be added to the resource</param>
    /// <returns>The resource so further calls can be chained</returns>
    public static Resource Data(this Resource resource, string name, object value) {
        resource.Data[name.ToCamelCase()] = value;
        return resource;
    }
}