using System;

namespace Net.LDVSoft.SPbAU.NUnitRunner
{
    internal static class TestRunnerMain
    {
        private static string ResultToString(TestResult result)
        {
            switch (result)
            {
                case TestSucceeded res:
                    return $"OK (in {res.RunnedTimeSpan})";

                case TestFailed res:
                    return $"FAILED (in ${res.RunnedTimeSpan}): {res.Cause}";

                case TestIgnored res:
                    return $"ignored: {res.Reason}";

                default:
                    throw new ArgumentOutOfRangeException(nameof(result));

            }
        }


        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Only one argument with path is accepted;");
                Environment.ExitCode = 1;
                return;
            }

            var results = TestRunner.RunTestsInPath(args[0]);
            foreach (var assemblyResults in results)
            {
                Console.WriteLine($"Assembly {assemblyResults.Name}:");
                foreach (var classResults in assemblyResults.Reports)
                {
                    Console.WriteLine($"  Class {classResults.Name}:");
                    foreach (var testResult in classResults.Reports)
                    {
                        Console.WriteLine($"    Test {testResult.Name}: {ResultToString(testResult)}");

                    }
                }
            }
        }
    }
}