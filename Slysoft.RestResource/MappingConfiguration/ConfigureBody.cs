using System.Linq.Expressions;
using Slysoft.RestResource.Extensions;
using Slysoft.RestResource.Utils;

namespace Slysoft.RestResource.MappingConfiguration;

public interface IConfigureBody {
    /// <summary>
    /// Add a field that the user can provide in the body of the request
    /// </summary>
    /// <param name="fieldName">name of the field</param>
    /// <param name="type">Data type of this field (text, number, date, etc.)</param>
    /// <param name="defaultValue">Default value for this field</param>
    /// <param name="listOfValues">List of values that are acceptable for this field</param>
    /// <returns>The configuration class so more values can be configured</returns>
    public IConfigureBody Field(string fieldName, string? type = null, string? defaultValue = null, IList<string>? listOfValues = null);

    /// <summary>
    /// Finish configuring the body
    /// </summary>
    /// <returns>The resource so further elements can be configured</returns>
    public Resource EndBody();
}

internal sealed class ConfigureBody : IConfigureBody {
    private readonly Resource _resource;
    private readonly Link _link;

    public ConfigureBody(Resource resource, Link link) {
        _resource = resource;
        _link = link;
    }

    public IConfigureBody Field(string fieldName, string? type = null, string? defaultValue = null, IList<string>? listOfValues = null) {
        _link.AddInputItem(fieldName, type, defaultValue, listOfValues);
        return this;
    }

    public Resource EndBody() {
        return _resource;
    }
}

public interface IConfigureBody<T> {
    /// <summary>
    /// Add a field that the user can provide in the body of the request
    /// </summary>
    /// <param name="mapAction">Expression to tell the name of the field- example: x => x.Name</param>
    /// <param name="type">Data type of this field (text, number, date, etc.)</param>
    /// <param name="defaultValue">Default value for this field</param>
    /// <param name="listOfValues">List of values that are acceptable for this field</param>
    /// <returns>The configuration class so more values can be configured</returns>
    public IConfigureBody<T> Field(Expression<Func<T, object>> mapAction, string? type = null, string? defaultValue = null, IList<string>? listOfValues = null);

    /// <summary>
    /// Automatically maps all field in T- individual fields can be overridden or excluded
    /// </summary>
    /// <returns>The configuration class so more values can be configured</returns>
    public IConfigureBody<T> AllFields();
    
    /// <summary>
    /// Do not include this field when mapping all (no, all does not mean all)
    /// </summary>
    /// <param name="mapAction">Expression to tell the data map which value to exclude- example: x => x.Name</param>
    /// <returns>The configuration class so more values can be configured</returns>
    public IConfigureBody<T> Exclude(Expression<Func<T, object>> mapAction);
    
    /// <summary>
    /// Finish configuring the body
    /// </summary>
    /// <returns>The resource so further elements can be configured</returns>
    public Resource EndBody();
}

internal sealed class ConfigureBody<T> : IConfigureBody<T> {
    private readonly Resource _resource;
    private readonly Link _link;
    private readonly IList<string> _excludedParameters = new List<string>();

    public ConfigureBody(Resource resource, Link link) {
        _resource = resource;
        _link = link;
    }

    public IConfigureBody<T> Field(Expression<Func<T, object>> mapAction, string? type = null, string? defaultValue = null, IList<string>? listOfValues = null) {
        var fieldName = mapAction.Evaluate();
        if (fieldName == null) {
            return this;
        }

        AddField(fieldName, type, defaultValue, listOfValues);

        return this;
    }

    public IConfigureBody<T> AllFields() {
        foreach (var property in typeof(T).GetAllProperties()) {
            if (_excludedParameters.Any(x => x.Equals(property.Name, StringComparison.CurrentCultureIgnoreCase))) {
                continue;
            }
            AddField(property.Name);
        }

        return this;
    }

    public IConfigureBody<T> Exclude(Expression<Func<T, object>> mapAction) {
        var fieldName = mapAction.Evaluate();
        if (fieldName == null) {
            return this;
        }

        _excludedParameters.Add(fieldName);

        var parameter = _link.GetInputItem(fieldName);
        if (parameter != null) {
            _link.InputItems.Remove(parameter);
        }

        return this;
    }

    private void AddField(string fieldName, string? type = null, string? defaultValue = null, IList<string>? listOfValues = null) {
        listOfValues ??= typeof(T).GetListOfValues(fieldName);

        _link.AddInputItem(fieldName, type, defaultValue, listOfValues);
    }

    public Resource EndBody() {
        return _resource;
    }
}