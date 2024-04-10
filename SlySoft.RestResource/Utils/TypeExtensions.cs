using System.Reflection;

namespace SlySoft.RestResource.Utils; 

internal static class TypeExtensions {
    public static IEnumerable<PropertyInfo> GetAllProperties(this Type type) {
        var properties = type.GetProperties().ToList();
        foreach (var interfaceType in type.GetInterfaces()) {
            var propertiesFromInterface = interfaceType.GetAllProperties();
            properties.AddRange(propertiesFromInterface);
        }

        return properties;
    }

    public static IList<string>? GetListOfValues(this Type type, string propertyName) {
        var property = type.GetProperty(propertyName);
        if (property == null) {
            return null;
        }


        if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?)) {
            return new List<string> { bool.TrueString, bool.FalseString };
        }

        var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
        var propertyType = underlyingType ?? property.PropertyType;

        if (propertyType.IsEnum) {
            return Enum.GetNames(propertyType);
        }

        return null;
    }
}