using Slysoft.RestResource.Utils;

namespace Slysoft.RestResource.Extensions; 

internal static class InternalResourceExtensions {
    public static void AddParameter(this Link link, string name, string? type, string? defaultValue, IList<string>? listOfValues) {
        var parameter = new LinkParameter(name.ToCamelCase());
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