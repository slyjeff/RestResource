using System.Collections;
using System.Linq.Expressions;

namespace SlySoft.RestResource.Utils; 

internal static class DictionaryExtensions {
    internal static void AddResourceData(this IDictionary<string, object?> dictionary, string name, object? value, string? format = null) {
        var dataName = name.ToCamelCase();
        dictionary[dataName] = ConvertValueToResourceData(value, format);
    }

    private static object? ConvertValueToResourceData(object? value, string? format) {
        if (value == null) {
            return null;
        }

        if (format != null) {
            return new FormattedValue(value, format);
        }

        var type = value.GetType();

        if (type == typeof(string)) {
            return value;
        }

        if (type.IsValueType) {
            return value;
        }

        if (value is not IEnumerable enumerableValue) {
            return ConvergeObjectToDictionary(value);
        }

        if (!type.IsGenericType) {
            return (from object? item in enumerableValue select ConvertValueToResourceData(item, null)).ToList();
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
            //ignore lists in child objects
            if (property.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(property.PropertyType)) {
                continue;
            }

            dictionary[property.Name.ToCamelCase()] = ConvertValueToResourceData(property.GetValue(value), null);
        }
        return dictionary;
    }

    public static void MapValue<T>(this IDictionary<string, object?> dictionary, T source, string name, Expression<Func<T, object>> mapAction, string? format) {
        if (source == null) {
            return;
        }

        var propertyName = mapAction.Evaluate();
        if (propertyName == null) {
            return;
        }

        var property = source.GetType().GetProperty(propertyName);
        if (property == null) {
            return;
        }

        if (string.IsNullOrEmpty(name)) {
            name = property.Name;
        }

        dictionary.AddResourceData(name, property.GetValue(source), format);
    }
}