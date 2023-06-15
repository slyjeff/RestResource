// ReSharper disable StringLiteralTypo

namespace TestUtils;

public static class GenerateRandom {
    private static readonly Random Random = new();

    public static string String(int length = 0) {
        if (length == 0) {
            length = Random.Next(10, 20);
        }
        

        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    public static int Int(int min = 0, int max = 1000) {
        return Random.Next(min, max);
    }

    public static DateTime DateTime() {
        return System.DateTime.Now.AddDays(Int() * -1 + Int());
    }

    public static float Float() {
        var mantissa = (Random.NextDouble() * 2.0) - 1.0;
        var exponent = Math.Pow(2.0, Random.Next(-126, 128));
        return (float)(mantissa * exponent);
    }
}