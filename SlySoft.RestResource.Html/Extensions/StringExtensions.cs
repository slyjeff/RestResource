namespace SlySoft.RestResource.Html.Extensions; 

internal static class StringExtensions {
    public class BeforeAndAfter {
        public BeforeAndAfter(string before, string after) {
            Before = before;
            After = after;
        }
        public string Before { get;}
        public string After { get; }
    }

    public static BeforeAndAfter GetTextBeforeAndAfterParameter(this string s, string parameter) {
        parameter = "{" + parameter + "}";
        var parameterPosition = s.IndexOf(parameter, StringComparison.CurrentCultureIgnoreCase);
        var before = s.Substring(0, parameterPosition);
        var after = s.Substring(parameterPosition + parameter.Length);
        return new BeforeAndAfter(before, after);
    }
}