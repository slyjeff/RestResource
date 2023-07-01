﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Slysoft.RestResource.Client.Utils; 

internal static class TypeExtensions {
    public static IEnumerable<PropertyInfo> GetAllProperties(this Type type) {
        var properties = type.GetProperties().ToList();
        foreach (var interfaceType in type.GetInterfaces()) {
            var propertiesFromInterface = interfaceType.GetAllProperties();
            properties.AddRange(propertiesFromInterface);
        }

        return properties;
    }

    public static IEnumerable<MethodInfo> GetAllMethods(this Type type) {
        var methods = type.GetMethods().ToList();
        foreach (var interfaceType in type.GetInterfaces()) {
            var methodsFromInterface = interfaceType.GetAllMethods();
            methods.AddRange(methodsFromInterface);
        }

        return methods;
    }

}