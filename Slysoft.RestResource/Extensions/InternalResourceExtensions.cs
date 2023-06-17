using Slysoft.RestResource.Utils;

namespace Slysoft.RestResource.Extensions; 

internal static class InternalResourceExtensions {
    public static void AddInputSpec(this Link link, string name, string? type, string? defaultValue, IList<string>? listOfValues) {
        var inputSpec = new InputSpec(name.ToCamelCase());
        link.InputSpecs.Add(inputSpec);

        inputSpec.Type = type;
        inputSpec.DefaultValue = defaultValue;

        if (listOfValues == null) {
            return;
        }

        foreach (var value in listOfValues) {
            inputSpec.ListOfValues.Add(value);
        }
    }
}