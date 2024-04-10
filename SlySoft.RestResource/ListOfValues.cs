namespace SlySoft.RestResource;

/// <summary>
/// Use an enum to create a list of strings
/// </summary>
/// <typeparam name="T">Enum type that will be converted to a string list</typeparam>
public class ListOfValues<T> : List<string> where T : Enum {
    public ListOfValues() {
        AddRange(Enum.GetNames(typeof(T)));
    }
}