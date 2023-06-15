using System.Collections;
using Slysoft.RestResource.MappingConfiguration;
using Slysoft.RestResource.Utils;

namespace Slysoft.RestResource.Extensions; 

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
    /// <param name="value">Value to be added to the resource. Objects will be converted to dictionaries; lists will be stored as lists; lists of objects will be stored as lists of dictionaries</param>
    /// <param name="format">Optional parameter that will be used to format the value</param>
    /// <returns>The resource so further calls can be chained</returns>
    public static Resource Data(this Resource resource, string name, object? value, string? format = null) {
        var dataName = name.ToCamelCase();
        resource.Data[dataName] = ConvertValueToResourceData(value, format);
        return resource;
    }

    private static object? ConvertValueToResourceData(object? value, string? format = null) {
        if (value == null) {
            return null;
        }

        if (format != null) {
            return string.Format($"{{0:{format}}}", value);
        }

        var type = value.GetType();

        if (type == typeof(string)) {
            return value;
        }

        if (type.IsValueType) {
            return value.ToString();
        }

        if (value is not IEnumerable enumerableValue) {
            return ConvergeObjectToDictionary(value);
        }

        if (!type.IsGenericType) {
            return (from object? item in enumerableValue select ConvertValueToResourceData(item)).ToList();
        }

        var genericArgumentType = type.GetGenericArguments()[0];
        if (genericArgumentType.IsValueType || genericArgumentType == typeof(string)) {
            return (from object? item in enumerableValue select item?.ToString()).Cast<object?>().ToList();
        }

        return (from object? item in enumerableValue select ConvergeObjectToDictionary(item)).ToList();
    }

    private static IDictionary<string, object?> ConvergeObjectToDictionary(object value) {
        IDictionary<string, object?> dictionary = new Dictionary<string, object?>();
        var properties = value.GetType().GetProperties();
        foreach (var property in properties) {
            dictionary[property.Name.ToCamelCase()] = ConvertValueToResourceData(property.GetValue(value));
        }
        return dictionary;
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
         /// Start configuration to map data from an object in a type safe way, determining which fields to map
         /// </summary>
         /// <typeparam name="T">The type of object to map</typeparam>
         /// <param name="resource">The data will be added to this resource</param>
         /// <param name="source">Data will be read from this object</param>
         /// <returns>A configuration class that will allow configuration of fields</returns>
    public static IConfigureDataMap<T> MapDataFrom<T>(this Resource resource, T source) {
        return new ConfigureDataMap<T>(resource, source);
    }
}