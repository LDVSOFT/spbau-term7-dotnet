using System;
using net.ldvsoft.spbau.nunit_runner;
using NUnit.Framework;

namespace net.ldvsoft.spbau.nunit_runner_test
{
    public class TestRunnerTest
    {
        private TestRunner _testRunner = new TestRunner();

        [Test]
        public void TestClassTest()
        {
            var results = TestRunner.RunTestsInClass(typeof(Tested));
            Console.WriteLine(results.Reports.ToString());
            Assert.AreEqual(typeof(Tested).FullName, results.Name);

            var test1Result = results.Reports.Find(it => it.Name == "Test1");
            Console.WriteLine($"test1: $test1Result");
            Assert.That(test1Result is TestSucceeded);
        }
    }

    internal class Tested
    {
        [nunit.Test]
        private void Test1()
        {
            
        }

        [nunit.Test(Ignore = "toBeIgnored")]
        private void IgnoredTest()
        {
            throw new Exception("Why am I being run?");
        }
    }
}