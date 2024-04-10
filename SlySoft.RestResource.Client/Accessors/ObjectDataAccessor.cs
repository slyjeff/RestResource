using SlySoft.RestResource.Client.Extensions;

namespace SlySoft.RestResource.Client.Accessors;

public abstract class ObjectDataAccessor : Accessor {
    private readonly ObjectData _objectData;

    protected ObjectDataAccessor(ObjectData objectData) {
        _objectData = objectData;
    }

    protected override T? CreateData<T>(string name) where T : default {
        return _objectData.GetData<T>(name);
    }
}