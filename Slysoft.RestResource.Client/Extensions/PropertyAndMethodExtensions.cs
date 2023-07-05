using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Slysoft.RestResource.Client.Accessors;

namespace Slysoft.RestResource.Client.Extensions; 

internal static class PropertyAndMethodExtensions {
    public static bool IsFromResourceAccessorInterface(this PropertyInfo property) {
        return property.Name is nameof(IResourceAccessor.Resource)
            or nameof(IEditableResource.IsChanged);
    }

    public static bool IsFromResourceAccessorInterfaces(this MethodInfo method) {
        return method.Name is nameof(IResourceAccessor.CallRestLink)
            or nameof(IResourceAccessor.CallRestLinkAsync)
            or nameof(IEditableResource.RejectChanges);
    }

    public static bool IsFromProperty(this MethodInfo method) {
        return method.Name.StartsWith("get_") || method.Name.StartsWith("set_");
    }

    public static bool IsFromEvent(this MethodInfo method) {
        return method.Name.StartsWith("add_") || method.Name.StartsWith("remove_");
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