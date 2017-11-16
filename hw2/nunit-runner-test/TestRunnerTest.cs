using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Net.LDVSoft.SPbAU.NUnit;
using Net.LDVSoft.SPbAU.NUnitRunner;
using NUnit.Framework;

namespace Net.LDVSoft.SPbAU.NUnitRunnerTest
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
            if (ignoredResult is TestIgnored testIgnored)
            {
                Assert.AreEqual("toBeIgnored", testIgnored.Reason);
            }
            else
            {
                    Assert.Fail($"{nameof(ignoredResult)} is not {nameof(TestIgnored)}");
            }
        }

        [global::NUnit.Framework.Test]
        public void TestedTestsTest()
        {
            var results = TestRunner.RunTestsInClass(typeof(Tested));
            TestedResultsTests(results);
        }

        private static void ThrowingTestedResultsTest(ClassTestsResults results)
        {
            Assert.AreEqual(typeof(ThrowingTested).FullName, results.Name);

            var mustNotThrowResult = results.Reports.Find(it => it.Name == "MustNotThrow");
            if (mustNotThrowResult is TestFailed mustNotThrowFailed)
            {
                Assert.That(mustNotThrowFailed.Cause is FormatException);
            }
            else
            {
                Assert.Fail($"{nameof(mustNotThrowResult)} is not {nameof(TestFailed)}");
            }

            var mustThrowResult = results.Reports.Find(it => it.Name == "MustThrow");
            if (mustThrowResult is TestFailed mustThrowFailed)
            {
                if (mustThrowFailed.Cause is TestDidNotThrowExpectedException mustThrowFailedCause)
                {
                    Assert.That(mustThrowFailedCause.ActualException is null);
                }
                else
                {
                    Assert.Fail($"{nameof(mustThrowFailedCause)} is not {nameof(TestDidNotThrowExpectedException)}");
                }
            }
            else
            {
                Assert.Fail($"{nameof(mustThrowFailed)} is not {nameof(TestFailed)}");
            }

            var throwsBadResult = results.Reports.Find(it => it.Name == "ThrowsBad");
            if (throwsBadResult is TestFailed throwsBadFailed)
            {
                if (throwsBadFailed.Cause is TestDidNotThrowExpectedException throwsBadFailedCause)
                {
                    Assert.IsFalse(throwsBadFailedCause.ActualException is null);
                }
                else
                {
                    Assert.Fail($"{nameof(throwsBadFailedCause)} is not {nameof(TestDidNotThrowExpectedException)}");
                }
            }
            else
            {
                Assert.Fail($"{nameof(throwsBadFailed)} is not {nameof(TestFixtureAttribute)}");
            }

            var throwsGoodResult = results.Reports.Find(it => it.Name == "ThrowsGood");
            Assert.That(throwsGoodResult is TestSucceeded);
        }

        [global::NUnit.Framework.Test]
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

        [global::NUnit.Framework.Test]
        public void TestAssemblyTest()
        {
            var results = TestRunner.RunTestsInAssembly(Assembly.GetExecutingAssembly());
            CurrentAssemblyResultsTest(results);
        }

        [global::NUnit.Framework.Test]
        public void TestAssemblyDiscovery()
        {
            var assemplyPath = Assembly.GetExecutingAssembly().Location;
            var results = TestRunner.RunTestsInPath(Path.GetDirectoryName(assemplyPath)).ToList();
            CurrentAssemblyResultsTest(results.Find(it => it.Name == Assembly.GetExecutingAssembly().FullName));
        }
    }

    internal class Tested
    {
        private string _data = "";

        [BeforeClass]
        private void BeforeClass()
        {
            _data += "1";
        }

        [Before]
        private void Before()
        {
            _data += "2";
        }

        [NUnit.Test]
        private void Test1()
        {
            _data += "a";
        }

        [NUnit.Test]
        private void Test2()
        {
            _data += "b";
        }

        [NUnit.Test(Ignore = "toBeIgnored")]
        private void IgnoredTest()
        {
            throw new Exception("Why am I being run?");
        }

        [After]
        private void After()
        {
            _data += "3";
        }

        [AfterClass]
        private void AfterClass()
        {
            _data += "4";
            Assert.That(_data == "12a32b34" || _data == "12b32a34");
        }
    }

    internal class ThrowingTested
    {
        [NUnit.Test]
        private void MustNotThrow()
        {
            throw new FormatException(":/");
        }

        [NUnit.Test(Expected = typeof(Exception))]
        private void MustThrow()
        {
        }

        [NUnit.Test(Expected = typeof(FormatException))]
        private void ThrowsGood()
        {
            throw new FormatException(":)");
        }

        [NUnit.Test(Expected = typeof(FormatException))]
        private void ThrowsBad()
        {
            throw new Exception(":)");
        }
    }
}