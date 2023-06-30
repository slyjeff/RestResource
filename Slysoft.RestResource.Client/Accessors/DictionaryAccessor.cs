using System.Collections.Generic;
using Slysoft.RestResource.Client.Extensions;

namespace Slysoft.RestResource.Client.Accessors;

public abstract class DictionaryAccessor {
    private readonly IDictionary<string, object?> _dictionary;
    private readonly IDictionary<string, object?> _cachedData = new Dictionary<string, object?>();

    protected DictionaryAccessor(IDictionary<string, object?> dictionary) {
        _dictionary = dictionary;
    }

    protected T? GetData<T>(string name) {
        if (_cachedData.TryGetValue(name, out var value)) {
            return (T?)value;
        }

        _cachedData[name] = _dictionary.GetData<T>(name);

        return (T?)_cachedData[name];
    }
}