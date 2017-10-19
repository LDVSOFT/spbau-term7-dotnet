using System;
using System.IO;
using System.Linq;
using System.Reflection;
using net.ldvsoft.spbau.nunit_runner;
using NUnit.Framework;

namespace net.ldvsoft.spbau.nunit_runner_test
{
    internal class TestRunnerTest
    {
        private static void TestedResultsTests(ClassTestsResults results)
        {
            Assert.AreEqual(typeof(Tested).FullName, results.Name);

            var test1Result = results.Reports.Find(it => it.Name == "Test1");
            Assert.That(test1Result is TestSucceeded);
            var test2Result = results.Reports.Find(it => it.Name == "Test2");
            Assert.That(test2Result is TestSucceeded);
            var ignoredResult = results.Reports.Find(it => it.Name == "IgnoredTest");
            Assert.That(ignoredResult is TestIgnored);
            Assert.AreEqual("toBeIgnored", (ignoredResult as TestIgnored).Reason);
        }

        [Test]
        public void TestedTestsTest()
        {
            var results = TestRunner.RunTestsInClass(typeof(Tested));
            TestedResultsTests(results);
        }

        private static void ThrowingTestedResultsTest(ClassTestsResults results)
        {
            Assert.AreEqual(typeof(ThrowingTested).FullName, results.Name);

            var mustNotThrowResult = results.Reports.Find(it => it.Name == "MustNotThrow");
            Assert.That(mustNotThrowResult is TestFailed);
            Assert.That((mustNotThrowResult as TestFailed).Cause is FormatException);

            var mustThrowResult = results.Reports.Find(it => it.Name == "MustThrow");
            Assert.That(mustThrowResult is TestFailed);
            var mustThrowResultCause = (mustThrowResult as TestFailed).Cause;
            Assert.That(mustThrowResultCause is TestDidNotThrowExpectedException);
            Assert.That((mustThrowResultCause as TestDidNotThrowExpectedException).ActualException is null);

            var throwsBadResult = results.Reports.Find(it => it.Name == "ThrowsBad");
            Assert.That(throwsBadResult is TestFailed);
            var throwsBadResultCause = (throwsBadResult as TestFailed).Cause;
            Assert.That(throwsBadResultCause is TestDidNotThrowExpectedException);
            Assert.False((throwsBadResultCause as TestDidNotThrowExpectedException).ActualException is null);

            var throwsGoodResult = results.Reports.Find(it => it.Name == "ThrowsGood");
            Assert.That(throwsGoodResult is TestSucceeded);
        }

        [Test]
        public void ThrowingTestsedTestsTest()
        {
            var results = TestRunner.RunTestsInClass(typeof(ThrowingTested));
            ThrowingTestedResultsTest(results);
        }

        private static void CurrentAssemblyResultsTest(AssemblyTestsResults results)
        {
            var testedResults = results.Reports.Find(it => it.Name == typeof(Tested).FullName);
            var throwingTestedResults = results.Reports.Find(it => it.Name == typeof(ThrowingTested).FullName);

            TestedResultsTests(testedResults);
            ThrowingTestedResultsTest(throwingTestedResults);
        }

        [Test]
        public void TestAssemblyTest()
        {
            var results = TestRunner.RunTestsInAssembly(Assembly.GetExecutingAssembly());
            CurrentAssemblyResultsTest(results);
        }

        [Test]
        public void TestAssemblyDiscovery()
        {
            var assemplyPath = Assembly.GetExecutingAssembly().Location;
            var results = TestRunner.RunTestsInPath(Path.GetDirectoryName(assemplyPath)).ToList();
        }
    }

    internal class Tested
    {
        private string _data = "";

        [nunit.BeforeClass]
        private void BeforeClass()
        {
            _data += "1";
        }

        [nunit.Before]
        private void Before()
        {
            _data += "2";
        }

        [nunit.Test]
        private void Test1()
        {
            _data += "a";
        }

        [nunit.Test]
        private void Test2()
        {
            _data += "b";
        }

        [nunit.Test(Ignore = "toBeIgnored")]
        private void IgnoredTest()
        {
            throw new Exception("Why am I being run?");
        }

        [nunit.After]
        private void After()
        {
            _data += "3";
        }

        [nunit.AfterClass]
        private void AfterClass()
        {
            _data += "4";
            Assert.That(_data == "12a32b34" || _data == "12b32a34");
        }
    }

    internal class ThrowingTested
    {
        [nunit.Test]
        private void MustNotThrow()
        {
            throw new FormatException(":/");
        }

        [nunit.Test(Expected = typeof(Exception))]
        private void MustThrow()
        {
        }

        [nunit.Test(Expected = typeof(FormatException))]
        private void ThrowsGood()
        {
            throw new FormatException(":)");
        }

        [nunit.Test(Expected = typeof(FormatException))]
        private void ThrowsBad()
        {
            throw new Exception(":)");
        }
    }
}