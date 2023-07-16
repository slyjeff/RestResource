using System.Collections;
using System.Linq.Expressions;

namespace SlySoft.RestResource.Utils; 

internal static class ObjectDataExtensions {
    internal static void AddResourceData(this ObjectData objectData, string name, object? value, string? format = null) {
        var dataName = name.ToCamelCase();
        objectData[dataName] = ConvertValueToResourceData(value, format);
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
            return ConvertValueToObjectData(value);
        }

        if (!type.IsGenericType) {
            return (from object? item in enumerableValue select ConvertValueToResourceData(item, null)).ToList();
        }

        var genericArgumentType = type.GetGenericArguments()[0];
        if (genericArgumentType.IsValueType || genericArgumentType == typeof(string)) {
            return (from object? item in enumerableValue select item?.ToString()).Cast<object?>().ToList();
        }

        var listData = new ListData();
        listData.AddRange(from object? enumeratedValue in enumerableValue select ConvertValueToObjectData(enumeratedValue));

        return listData;
    }

    private static ObjectData ConvertValueToObjectData(object value) {
        var objectData = new ObjectData();
        var properties = value.GetType().GetProperties();
        foreach (var property in properties) {
            //ignore lists in child objects
            if (property.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(property.PropertyType)) {
                continue;
            }

            objectData[property.Name.ToCamelCase()] = ConvertValueToResourceData(property.GetValue(value), null);
        }
        return objectData;
    }

    public static void MapValue<T>(this ObjectData objectData, T source, string name, Expression<Func<T, object>> mapAction, string? format) {
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

        objectData.AddResourceData(name, property.GetValue(source), format);
    }
}