using System.Linq.Expressions;
using Slysoft.RestResource.Extensions;
using Slysoft.RestResource.Utils;

namespace Slysoft.RestResource.MappingConfiguration;

/// <summary>
/// A configuration class that will allow configuration of fields
/// </summary>
/// <typeparam name="T">Load data from this type of object</typeparam>
public interface IConfigureDataMap<T> {
    /// <summary>
    /// Load a data value
    /// </summary>
    /// <param name="mapAction">Expression to tell the data map which value to load- example: x => x.Name. The name will be the name of the property in camelCase</param>
    /// <param name="format">Optional parameter that will be used to format the value</param>
    /// <returns>The configuration class so more values can be configured</returns>
    IConfigureDataMap<T> Map(Expression<Func<T, object>> mapAction, string? format = null);

    /// <summary>
    /// Load a data value
    /// </summary>
    /// <param name="name">Name to use for the data- will be forced to camelCase</param>
    /// <param name="mapAction">Expression to tell the data map which value to load- example: x => x.Name</param>
    /// <param name="format">Optional parameter that will be used to format the value</param>
    /// <returns>The configuration class so more values can be configured</returns>
    IConfigureDataMap<T> Map(string name, Expression<Func<T, object>> mapAction, string? format = null);

    /// <summary>
    /// Automatically maps all properties in T- individual fields can be overridden or excluded
    /// </summary>
    /// <returns>The configuration class so more values can be configured</returns>
    IConfigureDataMap<T> MapAll();

    /// <summary>
    /// Do not include this property when mapping all (no, all does not mean all)
    /// </summary>
    /// <returns>The configuration class so more values can be configured</returns>
    IConfigureDataMap<T> Exclude(Expression<Func<T, object>> mapAction);

    /// <summary>
    /// Finish configuring the map
    /// </summary>
    /// <returns>The original resource so further elements can be configured</returns>
    Resource EndMap();
}

internal sealed class ConfigureDataMap<T> : IConfigureDataMap<T> {
    private readonly Resource _resource;
    private readonly T _source;
    private readonly IList<string> _excludedProperties = new List<string>();

    public ConfigureDataMap(Resource resource, T source) {
        _resource = resource;
        _source = source;
    }

    public IConfigureDataMap<T> Map(Expression<Func<T, object>> mapAction, string? format = null) {
        return Map(string.Empty, mapAction, format);
    }

    public IConfigureDataMap<T> Map(string name, Expression<Func<T, object>> mapAction, string? format = null) {
        if (_source == null) {
            return this;
        }

        var propertyName = mapAction.Evaluate();
        if (propertyName == null) {
            return this;
        }

        var property = _source.GetType().GetProperty(propertyName);
        if (property == null) {
            return this;
        }

        if (string.IsNullOrEmpty(name)) {
            name = property.Name;
        }

        _resource.Data(name, property.GetValue(_source), format);

        return this;
    }

    public IConfigureDataMap<T> MapAll() {
        foreach (var property in typeof(T).GetProperties()) {
            if (_excludedProperties.Contains(property.Name)) {
                continue;
            }

            if (_resource.Data.ContainsKey(property.Name.ToCamelCase())) {
                continue;
            }

            _resource.Data(property.Name, property.GetValue(_source));
        }

        return this;
    }

    public IConfigureDataMap<T> Exclude(Expression<Func<T, object>> mapAction) {
        if (_source == null) {
            return this;
        }

        var propertyName = mapAction.Evaluate();
        if (propertyName == null) {
            return this;
        }

        _excludedProperties.Add(propertyName);

        propertyName = propertyName.ToCamelCase();

        if (_resource.Data.ContainsKey(propertyName)) {
            _resource.Data.Remove(propertyName);
        }

        return this;
    }

    public Resource EndMap() {
        return _resource;
    }
}