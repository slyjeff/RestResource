using System.Linq.Expressions;

namespace SlySoft.RestResource.MappingConfiguration {
    /// <summary>
    /// A configuration class that will allow configuration of parameters to be mapped into a resource
    /// </summary>
    /// <typeparam name="T">Load data from this type of object</typeparam>
    /// <typeparam name="TParent">The parent configurationType which EndMap will return to</typeparam>
    public interface IConfigureParametersMap<T, out TParent> {
        /// <summary>
        /// Load a data value
        /// </summary>
        /// <param name="mapAction">Expression to tell the data map which value to load- example: x => x.Name. The name will be the name of the property in camelCase</param>
        /// <param name="format">Optional parameter that will be used to format the value</param>
        /// <returns>The configuration class so more values can be configured</returns>
        IConfigureParametersMap<T, TParent> Map(Expression<Func<T, object>> mapAction, string? format = null);

        /// <summary>
        /// Load a data value
        /// </summary>
        /// <param name="name">Name to use for the data- will be forced to camelCase</param>
        /// <param name="mapAction">Expression to tell the data map which value to load- example: x => x.Name</param>
        /// <param name="format">Optional parameter that will be used to format the value</param>
        /// <returns>The configuration class so more values can be configured</returns>
        IConfigureParametersMap<T, TParent> Map(string name, Expression<Func<T, object>> mapAction, string? format = null);

        /// <summary>
        /// Start configuration to map data from a list in a type safe way, determining which fields to map
        /// </summary>
        /// <typeparam name="T">The type of object contained in the list to map</typeparam>
        /// <typeparam name="TListItemType"></typeparam>
        /// <param name="mapAction">Expression to tell the data map which list to load- example: x => x.Name</param>
        /// <returns>A configuration class that will allow configuration of fields</returns>
        IConfigureParametersMap<TListItemType, IConfigureParametersMap<T, TParent>> MapListDataFrom<TListItemType>(Expression<Func<T, IEnumerable<TListItemType>>> mapAction);

        /// <summary>
        /// Start configuration to map data from a list in a type safe way, determining which fields to map
        /// </summary>
        /// <typeparam name="T">The type of object contained in the list to map</typeparam>
        /// <typeparam name="TListItemType"></typeparam>
        /// <param name="name">Name to use for the list- will be forced to camelCase</param>
        /// <param name="mapAction">Expression to tell the data map which list to load- example: x => x.Name</param>
        /// <returns>A configuration class that will allow configuration of fields</returns>
        IConfigureParametersMap<TListItemType, IConfigureParametersMap<T, TParent>> MapListDataFrom<TListItemType>(string name, Expression<Func<T, IEnumerable<TListItemType>>> mapAction);

        /// <summary>
        /// Map all data from a list of objects- does not allow for exclusion, formatting of individual properties, or lists. If any of these are required, use MapListDataFrom instead
        /// </summary>
        /// <typeparam name="T">The type of object contained in the list to map</typeparam>
        /// <typeparam name="TListItemType"></typeparam>
        /// <param name="mapAction">Expression to tell the data map which list to load- example: x => x.Name</param>
        /// <returns>The configuration class so more values can be configured</returns>
        IConfigureParametersMap<T, TParent> MapAllListDataFrom<TListItemType>(Expression<Func<T, IEnumerable<TListItemType>>> mapAction);


        /// <summary>
        /// Automatically maps all properties in T- individual fields can be overridden or excluded
        /// </summary>
        /// <returns>The configuration class so more values can be configured</returns>
        IConfigureParametersMap<T, TParent> MapAll();

        /// <summary>
        /// Do not include this property when mapping all (no, all does not mean all)
        /// </summary>
        /// <param name="mapAction">Expression to tell the data map which value to exclude- example: x => x.Name</param>
        /// <returns>The configuration class so more values can be configured</returns>
        IConfigureParametersMap<T, TParent> Exclude(Expression<Func<T, object>> mapAction);

        /// <summary>
        /// Finish configuring the map
        /// </summary>
        /// <returns>The parent configuration item so further elements can be configured</returns>
        TParent EndMap();
    }
}
