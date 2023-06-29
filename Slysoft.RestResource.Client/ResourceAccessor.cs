using System.Collections.Generic;
using Slysoft.RestResource.Extensions;

namespace Slysoft.RestResource.Client; 

public abstract class ResourceAccessor  {
    private readonly Resource _resource;
    private readonly IDictionary<string, object?> _cachedData = new Dictionary<string, object?>();

    protected ResourceAccessor(Resource resource) {
        _resource = resource;
    }

    protected T? GetData<T>(string name) {
        if (_cachedData.TryGetValue(name, out var value)) {
            return (T?)value;
        }

        _cachedData[name] = _resource.GetData<T>(name);

        return (T?)_cachedData[name];
    }
}