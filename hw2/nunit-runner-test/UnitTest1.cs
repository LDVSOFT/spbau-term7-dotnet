using System;
using System.Reflection;
using net.ldvsoft.spbau.nunit_runner;
using NUnit.Framework;

namespace net.ldvsoft.spbau.nunit_runner_test
{
    public class TestRunnerTest
    {
        private TestRunner _testRunner = new TestRunner();

        [NUnit.Framework.Test]
        public void TestClassTest()
        {
            var results = TestRunner.RunTestsInClass(typeof(Tested));
            Console.WriteLine(results.Reports.ToString());
            Assert.NotNull(results.Reports.Find(testsResults => testsResults.Name == nameof(Tested)));
        }
    }

    internal class Tested
    {
        [net.ldvsoft.spbau.nunit.Test]
        private void test1()
        {
            
        }
    }
}