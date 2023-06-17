using System.Linq.Expressions;
using System.Xml;
using Slysoft.RestResource.Extensions;
using Slysoft.RestResource.Utils;

namespace Slysoft.RestResource.MappingConfiguration;

public interface IConfigureQuery {
    /// <summary>
    /// Add a parameter that a user can provide in the url when calling the link
    /// </summary>
    /// <param name="parameterName">name of the parameter</param>
    /// <param name="type">Data type of this parameter (text, number, date, etc.)</param>
    /// <param name="defaultValue">Default value for this parameter</param>
    /// <param name="listOfValues">List of values that are acceptable for this parameter</param>
    /// <returns>The configuration class so more values can be configured</returns>
    public IConfigureQuery Parameter(string parameterName, string? type = null, string? defaultValue = null, IList<string>? listOfValues = null);

    /// <summary>
    /// Finish configuring the query
    /// </summary>
    /// <returns>The resource so further elements can be configured</returns>
    public Resource EndQuery();
}

internal sealed class ConfigureQuery : IConfigureQuery {
    private readonly Resource _resource;
    private readonly Link _link;

    public ConfigureQuery(Resource resource, Link link) {
        _resource = resource;
        _link = link;
    }

    public IConfigureQuery Parameter(string parameterName, string? type = null, string? defaultValue = null, IList<string>? listOfValues = null) {
        _link.AddInputSpec(parameterName, type, defaultValue, listOfValues);
        return this;
    }

    public Resource EndQuery() {
        return _resource;
    }
}

public interface IConfigureQuery<T> {
    /// <summary>
    /// Add a parameter that a user can provide in the url when calling the link
    /// </summary>
    /// <param name="mapAction">Expression to tell the name of the parameter- example: x => x.Name</param>
    /// <param name="type">Data type of this parameter (text, number, date, etc.)</param>
    /// <param name="defaultValue">Default value for this parameter</param>
    /// <param name="listOfValues">List of values that are acceptable for this parameter</param>
    /// <returns>The configuration class so more values can be configured</returns>
    public IConfigureQuery<T> Parameter(Expression<Func<T, object>> mapAction, string? type = null, string? defaultValue = null, IList<string>? listOfValues = null);

    /// <summary>
    /// Automatically maps all parameters in T- individual fields can be overridden or excluded
    /// </summary>
    /// <returns>The configuration class so more values can be configured</returns>
    public IConfigureQuery<T> MapAll();
    
    /// <summary>
    /// Do not include this property when mapping all (no, all does not mean all)
    /// </summary>
    /// <param name="mapAction">Expression to tell the data map which value to exclude- example: x => x.Name</param>
    /// <returns>The configuration class so more values can be configured</returns>
    public IConfigureQuery<T> Exclude(Expression<Func<T, object>> mapAction);
    
    /// <summary>
    /// Finish configuring the query
    /// </summary>
    /// <returns>The resource so further elements can be configured</returns>
    public Resource EndQuery();
}

internal sealed class ConfigureQuery<T> : IConfigureQuery<T> {
    private readonly Resource _resource;
    private readonly Link _link;
    private readonly IList<string> _excludedParameters = new List<string>();

    public ConfigureQuery(Resource resource, Link link) {
        _resource = resource;
        _link = link;
    }

    public IConfigureQuery<T> Parameter(Expression<Func<T, object>> mapAction, string? type = null, string? defaultValue = null, IList<string>? listOfValues = null) {
        var parameterName = mapAction.Evaluate();
        if (parameterName == null) {
            return this;
        }

        AddParameter(parameterName, type, defaultValue, listOfValues);

        return this;
    }

    public IConfigureQuery<T> MapAll() {
        foreach (var property in typeof(T).GetAllProperties()) {
            if (_excludedParameters.Any(x => x.Equals(property.Name, StringComparison.CurrentCultureIgnoreCase))) {
                continue;
            }
            AddParameter(property.Name);
        }

        return this;
    }

    public IConfigureQuery<T> Exclude(Expression<Func<T, object>> mapAction) {
        var parameterName = mapAction.Evaluate();
        if (parameterName == null) {
            return this;
        }

        _excludedParameters.Add(parameterName);

        var parameter = _link.GetInputSpec(parameterName);
        if (parameter != null) {
            _link.InputSpecs.Remove(parameter);
        }

        return this;
    }

    private void AddParameter(string parameterName, string? type = null, string? defaultValue = null, IList<string>? listOfValues = null) {
        if (listOfValues == null) {
            var property = typeof(T).GetProperty(parameterName);
            if (property?.PropertyType == typeof(bool)) {
                listOfValues = new List<string> { bool.TrueString, bool.FalseString };
            }
        }

        _link.AddInputSpec(parameterName, type, defaultValue, listOfValues);

    }

    public Resource EndQuery() {
        return _resource;
    }
}