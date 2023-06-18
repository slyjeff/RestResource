namespace Slysoft.RestResource.Extensions; 

public static class UriExtensions {
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
}