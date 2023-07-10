using System.Collections.Generic;
using Slysoft.RestResource.Client.Extensions;

namespace Slysoft.RestResource.Client.Accessors;

public abstract class DictionaryAccessor : Accessor {
    private readonly IDictionary<string, object?> _dictionary;

    protected DictionaryAccessor(IDictionary<string, object?> dictionary) {
        _dictionary = dictionary;
    }

    protected override T? CreateData<T>(string name) where T : default {
        return _dictionary.GetData<T>(name);
    }
}