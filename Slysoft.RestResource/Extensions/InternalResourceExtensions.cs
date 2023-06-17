using Slysoft.RestResource.Utils;

namespace Slysoft.RestResource.Extensions; 

internal static class InternalResourceExtensions {
    public static void AddInputItem(this Link link, string name, string? type, string? defaultValue, IList<string>? listOfValues) {
        var inputItem = new InputItem(name.ToCamelCase());
        link.InputItems.Add(inputItem);

        inputItem.Type = type;
        inputItem.DefaultValue = defaultValue;

        if (listOfValues == null) {
            return;
        }

        foreach (var value in listOfValues) {
            inputItem.ListOfValues.Add(value);
        }
    }
}