using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Slysoft.RestResource.Client.Extensions;

public static class AccessorExtensions {
    /// <summary>
    /// Get data from the resource, converting it to the specified generic type
    /// </summary>
    /// <typeparam name="T">The data will be converted to this type, if possible</typeparam>
    /// <param name="resource">Resource containing the data</param>
    /// <param name="name">Case insensitive name of the data</param>
    /// <returns>The data, converted to the specified type</returns>
    public static T? GetData<T>(this Resource resource, string name) {
        return resource.Data.GetData<T>(name);
    }

    internal static T? GetData<T>(this IDictionary<string, object?> dictionary, string name) {
        foreach (var data in dictionary) {
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

        if (type.IsInterface && value is IDictionary<string, object?> interfaceDictionary) {
            return DictionaryAccessorFactory.CreateAccessor(type, interfaceDictionary);
        }

        if (type.IsClass && value is IDictionary<string, object?> classDictionary) {
            return ParseObject(classDictionary, type);
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
            return parseMethod.Invoke(null, new object[] { value.ToString() });
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
}
