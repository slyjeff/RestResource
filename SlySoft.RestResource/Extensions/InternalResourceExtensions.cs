using SlySoft.RestResource.Utils;

// ReSharper disable once CheckNamespace
namespace SlySoft.RestResource;

internal static class InternalResourceExtensions {
    public static void AddParameter(this Link link, string name, string? type, string? defaultValue, IList<string>? listOfValues) {
        var parameter = new LinkParameter(name.ToCamelCase());

        var existingParameter = link.GetParameter(name);
        if (existingParameter != null) {
            link.Parameters.Remove(existingParameter);
        }

        link.Parameters.Add(parameter);

        parameter.Type = type;
        parameter.DefaultValue = defaultValue;

        if (listOfValues == null) {
            return;
        }

        foreach (var value in listOfValues) {
            parameter.ListOfValues.Add(value);
        }
    }
}