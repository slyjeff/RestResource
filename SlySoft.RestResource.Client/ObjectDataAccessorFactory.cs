using System;
using System.Collections.Generic;
using SlySoft.RestResource.Client.Generators;

namespace SlySoft.RestResource.Client;

internal static class ObjectDataAccessorFactory {
    private static readonly Dictionary<Type, Type> CreatedTypes = new();

    internal static object CreateAccessor(Type interfaceType, ObjectData objectData) {
        Type accessorType;

        lock (CreatedTypes) {
            if (!CreatedTypes.ContainsKey(interfaceType)) {
                var factory = new ObjectDataAccessorGenerator(interfaceType);
                CreatedTypes[interfaceType] = factory.GeneratedType();
            }

            accessorType = CreatedTypes[interfaceType];
        }

        var accessor = Activator.CreateInstance(accessorType, objectData);
        if (accessor == null) {
            throw new CreateAccessorException($"Create Accessor for type {interfaceType.Name} returned a null.");
        }

        return accessor;
    }
}