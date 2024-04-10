using System.Linq.Expressions;
using SlySoft.RestResource.Utils;

namespace SlySoft.RestResource.MappingConfiguration;

internal sealed class ConfigureDataMap<T, TParent> : IConfigureParametersMap<T, TParent> {
    private readonly TParent _parent;
    private readonly T _source;
    private readonly ObjectData _objectData;
    private readonly IList<string> _excludedProperties = new List<string>();

    public ConfigureDataMap(TParent parent, T source, ObjectData objectData) {
        _parent = parent;
        _source = source;
        _objectData = objectData;
    }

    public IConfigureParametersMap<T, TParent> Map(Expression<Func<T, object>> mapAction, string? format = null) {
        return Map(string.Empty, mapAction, format);
    }

    public IConfigureParametersMap<T, TParent> Map(string name, Expression<Func<T, object>> mapAction, string? format = null) {
        _objectData.MapValue(_source, name, mapAction, format);
        return this;
    }

    public IConfigureParametersMap<TListItemType, IConfigureParametersMap<T, TParent>> MapListDataFrom<TListItemType>(Expression<Func<T, IEnumerable<TListItemType>>> mapAction) {
        return MapListDataFrom(string.Empty, mapAction);
    }

    public IConfigureParametersMap<TListItemType, IConfigureParametersMap<T, TParent>> MapListDataFrom<TListItemType>(string name, Expression<Func<T, IEnumerable<TListItemType>>> mapAction) {
        var destinationList = new ListData();

        if (_source == null) {
            return new ConfigureListMap<TListItemType, IConfigureParametersMap<T, TParent>>(this, new List<CopyPair<TListItemType>>());
        }

        var propertyName = mapAction.Evaluate();
        if (propertyName == null) {
            return new ConfigureListMap<TListItemType, IConfigureParametersMap<T, TParent>>(this, new List<CopyPair<TListItemType>>());
        }

        if (string.IsNullOrEmpty(name)) {
            name = propertyName;
        }

        var property = _source.GetType().GetProperty(propertyName);
        if (property == null) {
            return new ConfigureListMap<TListItemType, IConfigureParametersMap<T, TParent>>(this, new List<CopyPair<TListItemType>>());
        }

        if (property.GetValue(_source) is not IEnumerable<TListItemType> sourceList) {
            return new ConfigureListMap<TListItemType, IConfigureParametersMap<T, TParent>>(this, new List<CopyPair<TListItemType>>());
        }

        _objectData[name.ToCamelCase()] = destinationList;

        var copyPairs = new List<CopyPair<TListItemType>>();
        foreach (var sourceItem in sourceList) {
            var destination = new ObjectData();
            destinationList.Add(destination);
            copyPairs.Add(new CopyPair<TListItemType>(sourceItem, destination));
        }

        return new ConfigureListMap<TListItemType, IConfigureParametersMap<T, TParent>>(this, copyPairs);
    }

    public IConfigureParametersMap<T, TParent> MapAllListDataFrom<TListItemType>(Expression<Func<T, IEnumerable<TListItemType>>> mapAction) {
        throw new NotImplementedException();
        //return MapListDataFrom(mapAction).MapAll().EndMap();
    }

    public IConfigureParametersMap<T, TParent> MapAll() {
        foreach (var property in typeof(T).GetAllProperties()) {
            if (_excludedProperties.Contains(property.Name)) {
                continue;
            }

            if (_objectData.ContainsKey(property.Name.ToCamelCase())) {
                continue;
            }

            _objectData.AddResourceData(property.Name, property.GetValue(_source));
        }

        return this;
    }

    public IConfigureParametersMap<T, TParent> Exclude(Expression<Func<T, object>> mapAction) {
        var propertyName = mapAction.Evaluate();
        if (propertyName == null) {
            return this;
        }

        _excludedProperties.Add(propertyName);

        propertyName = propertyName.ToCamelCase();

        if (_objectData.ContainsKey(propertyName)) {
            _objectData.Remove(propertyName);
        }

        return this;
    }

    public TParent EndMap() {
        return _parent;
    }
}