﻿using Slysoft.RestResource.Extensions;

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

    public IConfigureQuery Parameter(string parameterName, string? type, string? defaultValue = null, IList<string>? listOfValues = null) {
        _link.AddInputSpec(parameterName, type, defaultValue, listOfValues);
        return this;
    }

    public Resource EndQuery() {
        return _resource;
    }
}