using System.Collections;
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
    /// <param name="value">Value to be added to the resource. Objects will be converted to dictionaries; lists will be stored as lists; lists of objects will be stored as lists of dictionaries</param>
    /// <returns>The resource so further calls can be chained</returns>
    public static Resource Data(this Resource resource, string name, object? value) {
        var dataName = name.ToCamelCase();
        resource.Data[dataName] = ConvertValueToResourceData(value);
        return resource;
    }

    private static object? ConvertValueToResourceData(object? value) {
        if (value == null) {
            return null;
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
}