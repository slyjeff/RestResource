using System.Linq.Expressions;
using System.Reflection;
using SlySoft.RestResource.Utils;

namespace SlySoft.RestResource.MappingConfiguration;

internal sealed class CopyPair<T> {
    public CopyPair(T source, IDictionary<string, object?> destination) {
        Source = source;
        Destination = destination;
    }
    public T Source { get; }
    public IDictionary<string, object?> Destination { get; }
}


internal sealed class ConfigureListMap<T, TParent> : IConfigureParametersMap<T, TParent> {
    private readonly IList<string> _excludedProperties = new List<string>();
    private readonly TParent _parent;
    private readonly IList<CopyPair<T>> _copyPairs;

    public ConfigureListMap(TParent parent, IList<CopyPair<T>> copyPairs) {
        _parent = parent;
        _copyPairs = copyPairs;
    }

    public IConfigureParametersMap<T, TParent> Map(Expression<Func<T, object>> mapAction, string? format = null) {
        return Map(string.Empty, mapAction, format);
    }

    public IConfigureParametersMap<T, TParent> Map(string name, Expression<Func<T, object>> mapAction, string? format = null) {
        foreach (var copyPair in _copyPairs) {
            copyPair.Destination.MapValue(copyPair.Source, name, mapAction, format);
        }

        return this;
    }

    public IConfigureParametersMap<TListItemType, IConfigureParametersMap<T, TParent>> MapListDataFrom<TListItemType>(Expression<Func<T, IEnumerable<TListItemType>>> mapAction) {
        return MapListDataFrom(string.Empty, mapAction);
    }

    public IConfigureParametersMap<TListItemType, IConfigureParametersMap<T, TParent>> MapListDataFrom<TListItemType>(string name, Expression<Func<T, IEnumerable<TListItemType>>> mapAction) {
        var subListCopyPairs = new List<CopyPair<TListItemType>>();

        var propertyName = mapAction.Evaluate();
        if (propertyName == null) {
            return new ConfigureListMap<TListItemType, IConfigureParametersMap<T, TParent>>(this, subListCopyPairs);
        }

        if (string.IsNullOrEmpty(name)) {
            name = propertyName;
        }

        PropertyInfo? property = null;

        foreach (var copyPair in _copyPairs) {
            var destinationList = new List<IDictionary<string, object?>>();
            var source = copyPair.Source;
            if (source == null) {
                continue;
            }

            if (property == null) {
                property = source.GetType().GetProperty(propertyName);
                if (property == null) {
                    continue;
                }
            }

            if (property.GetValue(source) is not IEnumerable<TListItemType> sourceList) {
                continue;
            }

            copyPair.Destination[name.ToCamelCase()] = destinationList;
            foreach (var sourceItem in sourceList) {
                var destination = new Dictionary<string, object?>();
                destinationList.Add(destination);
                subListCopyPairs.Add(new CopyPair<TListItemType>(sourceItem, destination));
            }

        }
        return new ConfigureListMap<TListItemType, IConfigureParametersMap<T, TParent>>(this, subListCopyPairs);
    }

    public IConfigureParametersMap<T, TParent> MapAllListDataFrom<TListItemType>(Expression<Func<T, IEnumerable<TListItemType>>> mapAction) {
        return MapListDataFrom(mapAction).MapAll().EndMap();
    }

    public IConfigureParametersMap<T, TParent> MapAll() {
        foreach (var copyPair in _copyPairs) {
            foreach (var property in typeof(T).GetAllProperties()) {
                if (_excludedProperties.Contains(property.Name)) {
                    continue;
                }

                if (copyPair.Destination.ContainsKey(property.Name.ToCamelCase())) {
                    continue;
                }

                copyPair.Destination.AddResourceData(property.Name, property.GetValue(copyPair.Source));
            }
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

        foreach (var copyPair in _copyPairs) {
            if (copyPair.Destination.ContainsKey(propertyName)) {
                copyPair.Destination.Remove(propertyName);
            }
        }

        return this;
    }

    public TParent EndMap() {
        return _parent;
    }
}