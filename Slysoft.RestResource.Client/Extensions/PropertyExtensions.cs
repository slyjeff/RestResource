using System;
using System.Reflection;

namespace Slysoft.RestResource.Client.Extensions; 

internal static class PropertyExtensions {
    public static bool IsLinkCheck(this PropertyInfo property) {
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
}