using Slysoft.RestResource.Client.Generators;
using System;
using System.Collections.Generic;
using System.Data;

namespace Slysoft.RestResource.Client;

internal static class DictionaryAccessorFactory {
    private static readonly Dictionary<Type, Type> CreatedTypes = new();

    internal static object CreateAccessor(Type interfaceType, IDictionary<string, object?> dictionary) {
        Type accessorType;

        lock (CreatedTypes) {
            if (!CreatedTypes.ContainsKey(interfaceType)) {
                var factory = new DictionaryAccessorGenerator(interfaceType);
                CreatedTypes[interfaceType] = factory.GeneratedType();
            }

            accessorType = CreatedTypes[interfaceType];
        }

        var accessor = Activator.CreateInstance(accessorType, dictionary);
        if (accessor == null) {
            throw new CreateAccessorException($"Create Accessor for type {interfaceType.Name} returned a null.");
        }

        return accessor;
    }
}