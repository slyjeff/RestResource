using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Slysoft.RestResource.Client.Accessors;

namespace Slysoft.RestResource.Client.Extensions;

internal static class AccessorExtensions {
    internal static T? GetData<T>(this Resource resource, string name, IRestClient restClient) {
        var data = resource.EmbeddedResources.GetEmbeddedAccessor<T>(name, restClient);
        if (data != null) {
            return (T?)data;
        }
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

    private static object? GetEmbeddedAccessor<T>(this IDictionary<string, object> dictionary, string name, IRestClient restClient) {
        foreach (var data in dictionary) {
            if (!data.Key.Equals(name, StringComparison.CurrentCultureIgnoreCase)) {
                continue;
            }
            
            if (data.Value is IList<Resource> resourceList) {
                var type = typeof(T);
                if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(IList<>)) {
                    throw new CreateAccessorException($"Specified property {name} must be an IList<>.");
                }

                var interfaceType = typeof(T).GetGenericArguments()[0];

                var list = CreateListOfType(interfaceType);
                foreach (var resourceItem in resourceList) {
                    list.Add(ResourceAccessorFactory.CreateAccessor(interfaceType, resourceItem, restClient));
                }

                return CreateEditableAccessorList(interfaceType, list);
            }

            if (data.Value is Resource resource) {
                return ResourceAccessorFactory.CreateAccessor<T>(resource, restClient);
            }
        }

        return null;
    }
    
    private static IList CreateListOfType(Type type) {
        var listType = typeof(List<>);
        var genericListType = listType.MakeGenericType(type);
        if (Activator.CreateInstance(genericListType) is not IList list) {
            throw new CreateAccessorException($"Error creating list of type {type.Name}");
        }

        return list;
    }

    private static object CreateEditableAccessorList(Type type, object sourceList) {
        var listType = typeof(EditableAccessorList<>);
        var genericListType = listType.MakeGenericType(type);
        return Activator.CreateInstance(genericListType, sourceList)
               ?? throw new CreateAccessorException($"Error creating editable accessor list of type {type.Name}");
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
            return Enum.Parse(type, value.ToString() ?? string.Empty);
        }

        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != null) {
            return string.IsNullOrEmpty(value.ToString()) ? null : ParseValue(value, underlyingType);
        }

        var parseMethod = type.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(string) }, null);
        if (parseMethod != null) {
            return parseMethod.Invoke(null, new object[] { value.ToString() ?? string.Empty });
        }

        return default;
    }

    private static object ParseList(IEnumerable enumerable, Type type) {
        var genericArgumentType = type.GetGenericArguments()[0];
        var list = CreateListOfType(genericArgumentType);
        foreach (var item in enumerable) {
            list.Add(ParseValue(item, genericArgumentType));
        }

        return CreateEditableAccessorList(genericArgumentType, list);
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
