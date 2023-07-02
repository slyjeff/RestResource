﻿namespace ClientTest;

internal interface ITest {
    void AssertAreEqual(object expected, object actual);
}

internal static class Test {
    internal static void Start(string testName, Action<ITest> action) {
        try {
            var test = new TestInstance(testName);
            action.Invoke(test);

            if (!test.HadError) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{testName} executed successfully.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        } catch (Exception e) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Message);
            Console.ForegroundColor = ConsoleColor.White;
        }
        Console.WriteLine();
        Console.WriteLine();
    }

    internal static async Task StartAsync(string testName, Func<ITest, Task> action) {
        try {
            var test = new TestInstance(testName);
            await action.Invoke(test);

            if (!test.HadError) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{testName} executed successfully.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        } catch (Exception e) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Message);
            Console.ForegroundColor = ConsoleColor.White;
        }
        Console.WriteLine();
        Console.WriteLine();
    }

    private class TestInstance : ITest {
        public TestInstance(string testName) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"--{testName}--");
        }

        public void Error(string error) {
            HadError = true;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void AssertAreEqual(object expected, object actual) {
            if (!expected.Equals(actual)) {
                Error($"Expected and actual values do not match{Environment.NewLine}E: {expected}{Environment.NewLine}A: {actual}");
            }
        }

        public bool HadError { get; private set; }
    }
}