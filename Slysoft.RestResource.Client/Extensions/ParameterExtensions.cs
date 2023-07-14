using System;
using System.Collections.Generic;

namespace SlySoft.RestResource.Client.Extensions;

internal static class ParameterExtensions {
    public static object? GetValue(this IDictionary<string, object?> parameters, string parameterName) {
        foreach (var parameter in parameters) {
            if (parameter.Key.Equals(parameterName, StringComparison.CurrentCultureIgnoreCase)) {
                return parameter.Value;
            }

            if (parameter.Value == null) {
                continue;
            }

            foreach (var property in parameter.Value.GetType().GetAllProperties()) {
                if (property.Name.Equals(parameterName, StringComparison.CurrentCultureIgnoreCase)) {
                    return property.GetValue(parameter.Value);
                }
            }
        }

        return null;
    }
}