using Slysoft.RestResource.MappingConfiguration;
using Slysoft.RestResource.Utils;

namespace Slysoft.RestResource.Extensions; 

public static class DataExtensions {
    /// <summary>
    /// Add a data element to the resource
    /// </summary>
    /// <param name="resource">The data will be added to this resource</param>
    /// <param name="name">Name of the element- will be converted to camelcase</param>
    /// <param name="value">Value to be added to the resource. Objects will be converted to dictionaries; lists will be stored as lists; lists of objects will be stored as lists of dictionaries</param>
    /// <param name="format">Optional parameter that will be used to format the value</param>
    /// <returns>The resource so further calls can be chained</returns>
    public static Resource Data(this Resource resource, string name, object? value, string? format = null) {
        resource.Data.AddResourceData(name, value, format);
        return resource;
    }

    /// <summary>
    /// Start configuration to map data from an object in a type safe way, determining which fields to map
    /// </summary>
    /// <typeparam name="T">The type of object to map</typeparam>
    /// <param name="resource">The data will be added to this resource</param>
    /// <param name="source">Data will be read from this object</param>
    /// <returns>A configuration class that will allow configuration of fields</returns>
    public static IConfigureParametersMap<T, Resource> MapDataFrom<T>(this Resource resource, T source) {
        return new ConfigureDataMap<T, Resource>(resource, source, resource.Data);
    }

    /// <summary>
    /// Map all data from an object- does not allow for exclusion, formatting of individual properties, or lists. If any of these are required, use MapDataFrom instead
    /// </summary>
    /// <typeparam name="T">The type of object to map</typeparam>
    /// <param name="resource">The data will be added to this resource</param>
    /// <param name="source">Data will be read from this object</param>
    /// <returns>The resource so further calls can be chained</returns>
    public static Resource MapAllDataFrom<T>(this Resource resource, T source) {
        return resource.MapDataFrom(source).MapAll().EndMap();
    }

    /// <summary>
    /// Start configuration to map data from a list in a type safe way, determining which fields to map
    /// </summary>
    /// <typeparam name="T">The type of object contained in the list to map</typeparam>
    /// <param name="resource">The data will be added to this resource</param>
    /// <param name="name">Name of the element- will be converted to camelcase</param>
    /// <param name="sourceList">List of items to be mapped</param>
    /// <returns>A configuration class that will allow configuration of fields</returns>
    public static IConfigureParametersMap<T, Resource> MapListDataFrom<T>(this Resource resource, string name, IEnumerable<T> sourceList) {
        var destinationList = new List<IDictionary<string, object?>>();

        var dataName = name.ToCamelCase();
        resource.Data[dataName] = destinationList;

        var copyPairs = new List<CopyPair<T>>();
        foreach (var sourceItem in sourceList) {
            var destination = new Dictionary<string, object?>();
            destinationList.Add(destination);
            copyPairs.Add(new CopyPair<T>(sourceItem, destination));
        }

        return new ConfigureListMap<T, Resource>(resource, copyPairs);
    }

    /// <summary>
    /// Map all data from a list of objects- does not allow for exclusion, formatting of individual properties, or lists. If any of these are required, use MapListDataFrom instead
    /// </summary>
    /// <typeparam name="T">The type of object contained in the list to map</typeparam>
    /// <param name="resource">The data will be added to this resource</param>
    /// <param name="name">Name of the element- will be converted to camelcase</param>
    /// <param name="sourceList">List of items to be mapped</param>
    /// <returns>The resource so further calls can be chained</returns>
    public static Resource MapAllListDataFrom<T>(this Resource resource, string name, IEnumerable<T> sourceList) {
        return resource.MapListDataFrom(name, sourceList).MapAll().EndMap();
    }
}