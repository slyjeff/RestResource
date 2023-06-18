using Slysoft.RestResource.Utils;

namespace Slysoft.RestResource.Extensions; 

public static class EmbeddedExtensions {
    /// <summary>
    /// Add an embedded resource to a parent resource
    /// </summary>
    /// <param name="resource">Parent resource which will contain the resource</param>
    /// <param name="name">Name of the resource- will be converted to camelcase</param>
    /// <param name="embeddedResource">Embedded resource to add to the parent resource</param>
    /// <returns>The parent resource so further calls can be chained</returns>
    public static Resource Embedded(this Resource resource, string name, Resource embeddedResource) {
        resource.EmbeddedResources[name.ToCamelCase()] = embeddedResource;
        return resource;
    }

    /// <summary>
    /// Add a list of embedded resources to a parent resource
    /// </summary>
    /// <param name="resource">Parent resource which will contain the list of resources</param>
    /// <param name="name">Name of the list of resources- will be converted to camelcase</param>
    /// <param name="embeddedResource">List of embedded resource to add to the parent resource</param>
    /// <returns>The parent resource so further calls can be chained</returns>
    public static Resource Embedded(this Resource resource, string name, IList<Resource> embeddedResource) {
        resource.EmbeddedResources[name.ToCamelCase()] = embeddedResource;
        return resource;
    }
}