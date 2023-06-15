using System.Reflection;

namespace Slysoft.RestResource.Utils; 

internal static class TypeExtensions {
    public static IEnumerable<PropertyInfo> GetAllProperties(this Type type) {
        var properties = type.GetProperties().ToList();
        foreach (var interfaceType in type.GetInterfaces()) {
            var propertiesFromInterface = interfaceType.GetAllProperties();
            properties.AddRange(propertiesFromInterface);
        }

        return properties;
    }
}