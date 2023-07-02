﻿namespace Slysoft.RestResource.Client.Extensions; 

internal static class StringExtensions {
    public static string RemoveOuterQuotes(this string s) {
        if (string.IsNullOrEmpty(s)) {
            return s;
        }

        if (s.StartsWith("\"")) {
            s = s.Substring(1);
        }

        if (s.EndsWith("\"")) {
            s = s.Substring(0, s.Length - 1);
        }

        return s;
    }
}