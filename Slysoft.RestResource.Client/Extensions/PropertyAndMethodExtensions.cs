using System;
using System.Reflection;
using Slysoft.RestResource.Client.Accessors;

namespace Slysoft.RestResource.Client.Extensions; 

internal static class PropertyAndMethodExtensions {
    public static bool IsFromResourceAccessorInterface(this PropertyInfo property) {
        //return false;
        return property.Name == nameof(IResourceAccessor.Resource);
    }

    public static bool IsFromResourceAccessorInterface(this MethodInfo method) {
        return method.Name is "get_" + nameof(IResourceAccessor.Resource) 
            or nameof(IResourceAccessor.CallRestLink) 
            or nameof(IResourceAccessor.CallRestLinkAsync);
    }

    public static bool IsLinkCheck(this PropertyInfo property) {
        if (property.PropertyType != typeof(bool)) {
            return false;
        }

        var attribute = property.GetCustomAttribute<LinkCheckAttribute>();
        return attribute != null;
    }

    public static string GetLinkCheckName(this PropertyInfo property) {
        var linkCheck = property.GetCustomAttribute<LinkCheckAttribute>();
        if (linkCheck == null) {
            return string.Empty;
        }

        if (linkCheck.LinkName != null) {
            return linkCheck.LinkName;
        }

        return property.Name.StartsWith("Can", StringComparison.CurrentCultureIgnoreCase)
            ? property.Name.Substring("Can".Length)
            : property.Name;
    }

    public static bool IsParameterInfo(this PropertyInfo property) {
        if (property.PropertyType != typeof(IParameterInfo)) {
            return false;
        }

        var attribute = property.GetCustomAttribute<ParameterInfoAttribute>();
        return attribute != null;
    }
}