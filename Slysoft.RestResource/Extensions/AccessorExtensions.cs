using System.Collections;
using System.Reflection;

namespace Slysoft.RestResource.Extensions;

public static class AccessorExtensions {
    /// <summary>
    /// Get data from the resource, converting it to the specified generic type
    /// </summary>
    /// <typeparam name="T">The data will be converted to this type, if possible</typeparam>
    /// <param name="resource">Resource containing the data</param>
    /// <param name="name">Case insensitive name of the data</param>
    /// <returns>The data, converted to the specified type</returns>
    public static T? GetData<T>(this Resource resource, string name) {
        foreach (var data in resource.Data) {
            if (data.Key.Equals(name, StringComparison.CurrentCultureIgnoreCase)) {
                return (T?)ParseValue(data.Value, typeof(T));
            }
        }

        return default;
    }

    private static object? ParseValue(object? value, Type type) {
        if (value == null) {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        if (value.GetType() == type) {
            return value;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>) && value is IEnumerable enumerable) {
            return ParseList(enumerable, type);
        }

        if (type.IsClass && value is IDictionary<string, object?> dictionary) {
            return ParseObject(dictionary, type);
        }

        if (type == typeof(string)) {
            return value;
        }

        if (type.IsEnum) {
            return Enum.Parse(type, value.ToString());
        }

        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != null) {
            return string.IsNullOrEmpty(value.ToString()) ? null : ParseValue(value, underlyingType);
        }

        var parseMethod = type.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(string) }, null);
        if (parseMethod != null) {
            return parseMethod.Invoke(null, new[] { value.ToString() });
        }

        return default;
    }

    private static object? ParseList(IEnumerable enumerable, Type type) {
        var genericArgumentType = type.GetGenericArguments()[0];
        var listType = typeof(List<>);
        var genericListType = listType.MakeGenericType(genericArgumentType);
        var instance = Activator.CreateInstance(genericListType);
        if (instance is IList list) {
            foreach (var item in enumerable) {
                list.Add(ParseValue(item, genericArgumentType));
            }
        }
        return instance;
    }

    private static object? ParseObject(IDictionary<string, object?> dictionary, Type type) {
        var newObject = Activator.CreateInstance(type);
        foreach (var property in type.GetProperties()) {
            foreach (var dictionaryItem in dictionary) {
                if (dictionaryItem.Key.Equals(property.Name, StringComparison.CurrentCultureIgnoreCase)) {
                    property.SetValue(newObject, ParseValue(dictionaryItem.Value, property.PropertyType));
                    break;
                }
            }
        }
        return newObject;
    }


    /// <summary>
    /// Get a single embedded resource by name
    /// </summary>
    /// <param name="resource">Resource to search</param>
    /// <param name="embeddedName">Name of the embedded resource to find= case insensitive</param>
    /// <returns>A resource matching the passed in name</returns>
    public static Resource? GetEmbedded(this Resource resource, string embeddedName) {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var embedded in resource.EmbeddedResources) {
            if (embedded.Key.Equals(embeddedName, StringComparison.CurrentCultureIgnoreCase)) {
                return embedded.Value as Resource;
            }
        }

        return null;
    }

    /// <summary>
    /// Get an embedded resource list by name
    /// </summary>
    /// <param name="resource">Resource to search</param>
    /// <param name="embeddedName">Name of the embedded resource list to find= case insensitive</param>
    /// <returns>A resource matching the passed in name</returns>
    public static IList<Resource>? GetEmbeddedList(this Resource resource, string embeddedName) {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var embedded in resource.EmbeddedResources) {
            if (embedded.Key.Equals(embeddedName, StringComparison.CurrentCultureIgnoreCase)) {
                return embedded.Value as IList<Resource>;
            }
        }

        return null;
    }

    /// <summary>
    /// Find a link in a resource
    /// </summary>
    /// <param name="resource">Resource to search</param>
    /// <param name="linkName">Name of the link to find= case insensitive</param>
    /// <returns>Link matching the name, if one exists</returns>
    public static Link? GetLink(this Resource resource, string linkName) {
        return resource.Links.FirstOrDefault(x => x.Name.Equals(linkName, StringComparison.CurrentCultureIgnoreCase));
    }

    /// <summary>
    /// Find an input item in a link
    /// </summary>
    /// <param name="link">link to search</param>
    /// <param name="inputItemName">Name of the input item to find= case insensitive</param>
    /// <returns>Input item matching the name, if one exists</returns>
    public static InputItem? GetInputItem(this Link link, string inputItemName) {
        return link.InputItems.FirstOrDefault(x =>
            x.Name.Equals(inputItemName, StringComparison.CurrentCultureIgnoreCase));
    }

    /// <summary>
    /// Get the type of input item this link supports (parameter, field)
    /// </summary>
    /// <param name="link">link with the input item</param>
    /// <returns>Type of input item this link supports (parameter, field)</returns>
    public static string GetInputItemName(this Link link) {
        return link.Verb.Equals("GET", StringComparison.CurrentCultureIgnoreCase) ? "parameter" : "field";
    }

    /// <summary>
    /// Get a list of parameters in a templated link href
    /// </summary>
    /// <param name="link">Link containing the parameters</param>
    /// <returns>List of parameters</returns>
    public static IEnumerable<string> GetParameters(this Link link) {
        var parameters = new List<string>();
        if (!link.Templated) {
            return parameters;
        }

        for (var index = 0; index < link.Href.Length; index++) {
            if (link.Href[index] != '{') {
                continue;
            }

            var closingBracketIndex = link.Href.IndexOf('}', index);
            if (closingBracketIndex < index) {
                continue;
            }

            var parameterStart = index + 1;
            var parameterEnd = closingBracketIndex - 1;
            parameters.Add(link.Href.Substring(parameterStart, parameterEnd - parameterStart + 1));
        }

        return parameters;
    }
}