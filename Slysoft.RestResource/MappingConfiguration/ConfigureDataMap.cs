using System.Linq.Expressions;
using Slysoft.RestResource.Utils;

namespace Slysoft.RestResource.MappingConfiguration;

internal sealed class ConfigureDataMap<T, TParent> : IConfigureMap<T, TParent> {
    private readonly TParent _parent;
    private readonly T _source;
    private readonly IDictionary<string, object?> _dictionary;
    private readonly IList<string> _excludedProperties = new List<string>();

    public ConfigureDataMap(TParent parent, T source, IDictionary<string, object?> dictionary) {
        _parent = parent;
        _source = source;
        _dictionary = dictionary;
    }

    public IConfigureMap<T, TParent> Map(Expression<Func<T, object>> mapAction, string? format = null) {
        return Map(string.Empty, mapAction, format);
    }

    public IConfigureMap<T, TParent> Map(string name, Expression<Func<T, object>> mapAction, string? format = null) {
        _dictionary.MapValue(_source, name, mapAction, format);
        return this;
    }

    public IConfigureMap<TListItemType, IConfigureMap<T, TParent>> MapListDataFrom<TListItemType>(Expression<Func<T, IEnumerable<TListItemType>>> mapAction) {
        return MapListDataFrom(string.Empty, mapAction);
    }

    public IConfigureMap<TListItemType, IConfigureMap<T, TParent>> MapListDataFrom<TListItemType>(string name, Expression<Func<T, IEnumerable<TListItemType>>> mapAction) {
        var destinationList = new List<IDictionary<string, object?>>();

        if (_source == null) {
            return new ConfigureListMap<TListItemType, IConfigureMap<T, TParent>>(this, new List<CopyPair<TListItemType>>());
        }

        var propertyName = mapAction.Evaluate();
        if (propertyName == null) {
            return new ConfigureListMap<TListItemType, IConfigureMap<T, TParent>>(this, new List<CopyPair<TListItemType>>());
        }

        if (string.IsNullOrEmpty(name)) {
            name = propertyName;
        }

        var property = _source.GetType().GetProperty(propertyName);
        if (property == null) {
            return new ConfigureListMap<TListItemType, IConfigureMap<T, TParent>>(this, new List<CopyPair<TListItemType>>());
        }

        if (property.GetValue(_source) is not IEnumerable<TListItemType> sourceList) {
            return new ConfigureListMap<TListItemType, IConfigureMap<T, TParent>>(this, new List<CopyPair<TListItemType>>());
        }

        _dictionary[name.ToCamelCase()] = destinationList;

        var copyPairs = new List<CopyPair<TListItemType>>();
        foreach (var sourceItem in sourceList) {
            var destination = new Dictionary<string, object?>();
            destinationList.Add(destination);
            copyPairs.Add(new CopyPair<TListItemType>(sourceItem, destination));
        }

        return new ConfigureListMap<TListItemType, IConfigureMap<T, TParent>>(this, copyPairs);
    }

    public IConfigureMap<T, TParent> MapAllListDataFrom<TListItemType>(Expression<Func<T, IEnumerable<TListItemType>>> mapAction) {
        throw new NotImplementedException();
        //return MapListDataFrom(mapAction).MapAll().EndMap();
    }

    public IConfigureMap<T, TParent> MapAll() {
        foreach (var property in typeof(T).GetProperties()) {
            if (_excludedProperties.Contains(property.Name)) {
                continue;
            }

            if (_dictionary.ContainsKey(property.Name.ToCamelCase())) {
                continue;
            }

            _dictionary.AddResourceData(property.Name, property.GetValue(_source));
        }

        return this;
    }

    public IConfigureMap<T, TParent> Exclude(Expression<Func<T, object>> mapAction) {
        var propertyName = mapAction.Evaluate();
        if (propertyName == null) {
            return this;
        }

        _excludedProperties.Add(propertyName);

        propertyName = propertyName.ToCamelCase();

        if (_dictionary.ContainsKey(propertyName)) {
            _dictionary.Remove(propertyName);
        }

        return this;
    }

    public TParent EndMap() {
        return _parent;
    }
}