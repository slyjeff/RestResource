using System;
using System.Collections.Generic;

namespace Slysoft.RestResource.Client;

/// <summary>
/// Create a runtime class that implements an interface to access a resource. The interface does not have to match the resource completely, but [LinkCheck] properties should be used
/// before executing links to make sure that they exist. Properties that do not exist in the resource will return default values.
/// </summary>
public interface IResourceAccessorFactory {
    /// <summary>
    /// Create a runtime class that implements an interface to access a resource. The interface does not have to match the resource completely, but [LinkCheck] properties should be used
    /// before executing links to make sure that they exist. Properties that do not exist in the resource will return default values.
    /// </summary>
    /// <typeparam name="T">Interface to implement when creating the access class</typeparam>
    /// <param name="resource">Resource that will be accessed</param>
    /// <returns>An object that implements T interface to access properties of the passed in resource</returns>
    T CreateAccessor<T>(Resource resource) where T : class;
}

/// <summary>
/// Create a runtime class that implements an interface to access a resource. The interface does not have to match the resource completely, but [LinkCheck] properties should be used
/// before executing links to make sure that they exist. Properties that do not exist in the resource will return default values.
/// </summary>
public class ResourceAccessorFactory : IResourceAccessorFactory {
    private static readonly Dictionary<Type, Type> CreatedTypes = new();

    /// <summary>
    /// Create a runtime class that implements an interface to access a resource. The interface does not have to match the resource completely, but [LinkCheck] properties should be used
    /// before executing links to make sure that they exist. Properties that do not exist in the resource will return default values.
    /// </summary>
    /// <typeparam name="T">Interface to implement when creating the access class</typeparam>
    /// <param name="resource">Resource that will be accessed</param>
    /// <returns>An object that implements T interface to access properties of the passed in resource</returns>
    public T CreateAccessor<T>(Resource resource) where T : class {
        var typeToCreate = typeof(T);

        Type accessorType;

        lock (CreatedTypes) {
            if (!CreatedTypes.ContainsKey(typeToCreate)) {
                var factory = new TypedResourceGenerator<T>();

                CreatedTypes[typeToCreate] = factory.GeneratedType();
            }

            accessorType = CreatedTypes[typeToCreate];
        }

        if (Activator.CreateInstance(accessorType) is not T accessor) {
            throw new CreateAccessorException($"Create Instance for type {accessorType.Name} returned a null.");
        }

        return accessor;
    }
}