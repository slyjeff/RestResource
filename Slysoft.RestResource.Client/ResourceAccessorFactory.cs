using System;
using System.Collections.Generic;
using System.Reflection;
using Slysoft.RestResource.Client.Generators;

namespace Slysoft.RestResource.Client;

/// <summary>
/// Create a runtime class that implements an interface to access a resource. The interface does not have to match the resource completely, but [LinkCheck] properties should be used
/// before executing links to make sure that they exist. Properties that do not exist in the resource will return default values.
/// </summary>
public static class ResourceAccessorFactory {
    private static readonly Dictionary<Type, Type> CreatedTypes = new();

    /// <summary>
    /// Create a runtime class that implements an interface to access a resource. The interface does not have to match the resource completely, but [LinkCheck] properties should be used
    /// before executing links to make sure that they exist. Properties that do not exist in the resource will return default values.
    /// </summary>
    /// <typeparam name="T">Interface to implement when creating the access class</typeparam>
    /// <param name="resource">Resource that will be accessed</param>
    /// <param name="restClient">RestClient so that links can be called</param>
    /// <returns>An object that implements T interface to access properties of the passed in resource</returns>
    public static T CreateAccessor<T>(Resource resource, IRestClient restClient) {
        var typeToCreate = typeof(T);

        Type accessorType;

        lock (CreatedTypes) {
            if (!CreatedTypes.ContainsKey(typeToCreate)) {
                var factory = new ResourceAccessorGenerator<T>();
                CreatedTypes[typeToCreate] = factory.GeneratedType();
            }

            accessorType = CreatedTypes[typeToCreate];
        }

        if (Activator.CreateInstance(accessorType, resource, restClient) is not T accessor) {
            throw new CreateAccessorException($"Create Instance for type {accessorType.Name} returned a null.");
        }

        return accessor;
    }

    private static MethodInfo? _genericCreateAccessorMethod;

    internal static object CreateAccessor(Type interfaceType, Resource resource, IRestClient restClient) {
        if (_genericCreateAccessorMethod == null) {
            var createAccessorMethod = typeof(ResourceAccessorFactory).GetMethod("CreateAccessor", BindingFlags.Public | BindingFlags.Static);
            if (createAccessorMethod == null) {
                throw new CreateAccessorException("Method 'CreateAccessor' not found in ResourceAccessorFactory.");
            }

            _genericCreateAccessorMethod = createAccessorMethod.MakeGenericMethod(interfaceType);
        }

        return _genericCreateAccessorMethod.Invoke(null, new object[] { resource, restClient });
    }
}