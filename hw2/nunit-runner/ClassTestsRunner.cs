using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Net.LDVSoft.SPbAU.NUnit;

namespace Net.LDVSoft.SPbAU.NUnitRunner
{
    internal class ClassTestsRunner
    {
        private static readonly object[] NoParams = new object[0];

        private readonly Type _type;
        private readonly object _obj;
        private readonly List<MethodInfo> _beforeMethods, _afterMethods;

        public ClassTestsRunner(Type type)
        {
            _type = type;
            _obj = Activator.CreateInstance(type);

            _beforeMethods = type.GetRuntimeMethods()
                .Where(CheckAttributeForExistense<Before>)
                .ToList();
            _afterMethods = type.GetRuntimeMethods()
                .Where(CheckAttributeForExistense<After>)
                .ToList();
        }

        private TestResult DoRunTest(MethodInfo testMethod, Test test)
        {
            var stopwatch = new Stopwatch();
            try
            {
                try
                {
                    var haveThrown = false;
                    stopwatch.Start();
                    try
                    {
                        testMethod.Invoke(_obj, NoParams);
                    }
                    catch (TargetInvocationException e)
                    {
                        if (test.Expected != null)
                        {
                            haveThrown = true;
                            if (!test.Expected.IsInstanceOfType(e.InnerException))
                            {
                                throw new TestDidNotThrowExpectedException(testMethod, test.Expected, e.InnerException);
                            }
                        }
                        else
                        {
                            return new TestFailed(testMethod.Name, stopwatch.Elapsed, e.InnerException);
                        }
                    }
                    if (test.Expected != null && !haveThrown)
                    {
                        return new TestFailed(testMethod.Name, stopwatch.Elapsed, new TestDidNotThrowExpectedException(testMethod, test.Expected, null));
                    }
                }
                finally
                {
                    stopwatch.Stop();
                }
            }
            catch (Exception e)
            {
                return new TestFailed(testMethod.Name, stopwatch.Elapsed, e);
            }
            return new TestSucceeded(testMethod.Name, stopwatch.Elapsed);
        }

        private TestResult RunTest(MethodInfo testMethod)
        {
            var test = testMethod.GetCustomAttribute<Test>();
            if (test.Ignore != null)
            {
                return new TestIgnored(testMethod.Name, test.Ignore);
            }

            foreach (var method in _beforeMethods)
            {
                try
                {
                    method.Invoke(_obj, NoParams);
                }
                catch (Exception e)
                {
                    throw new SetupMethodThrewException(testMethod, _type, method, e);
                }
            }

            var pendingResult = DoRunTest(testMethod, test);
            foreach (var method in _afterMethods)
            {
                try
                {
                    method.Invoke(_obj, NoParams);
                }
                catch (Exception e)
                {
                    throw new SetupMethodThrewException(testMethod, _type, method, e);
                }
            }

            return pendingResult;
        }

        public ClassTestsResults RunTests()
        {
            foreach (var method in _type
                .GetRuntimeMethods()
                .Where(method => method.GetCustomAttribute<BeforeClass>() != null))
            {
                try
                {
                    method.Invoke(_obj, NoParams);
                }
                catch (Exception e)
                {
                    throw new ClassSetupMethodThrewException(_type, method, e);
                }
            }

            var reports = _type.GetRuntimeMethods()
                .Where(CheckAttributeForExistense<Test>)
                .Select(RunTest)
                .ToList();

            foreach (var method in _type
                .GetRuntimeMethods()
                .Where(method => method.GetCustomAttribute<AfterClass>() != null))
            {
                try
                {
                    method.Invoke(_obj, NoParams);
                }
                catch (Exception e)
                {
                    throw new ClassSetupMethodThrewException(_type, method, e);
                }
            }

            return new ClassTestsResults(_type.FullName, reports);
        }

        internal static bool CheckAttributeForExistense<T>(MemberInfo methodInfo)
            where T : System.Attribute
        {
            return methodInfo.GetCustomAttribute<T>() != null;
        }
    }
}