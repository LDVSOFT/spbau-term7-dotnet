using System;

namespace net.ldvsoft.spbau.nunit_runner
{
    internal static class TestRunnerMain
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine("Only one argument with path is accepted;");
                Environment.ExitCode = 1;
                return;
            }                
            var runner = new TestRunner();
            runner.RunTestsInPath(args[1]);            
        }
    }
}