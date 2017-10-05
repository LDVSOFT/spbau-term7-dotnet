using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using net.ldvsoft.spbau.nunit;
using static System.Activator;

namespace net.ldvsoft.spbau.nunit_runner
{        
    public class TestRunner
    {
        public static ClassTestsResults RunTestsInClass(Type type)
        {
            return new ClassTestsRunner(type).RunTests();
        }
        
        public static AssemblyTestsResults RunTestsInAssembly(Assembly assembly)
        {
            var classTestsReports = (
                from type in assembly.ExportedTypes 
                where type.GetRuntimeMethods()
                    .Any(method => method.GetCustomAttribute<Test>() != null) 
                select RunTestsInClass(type)
            ).ToList();
            return new AssemblyTestsResults(assembly.FullName, classTestsReports);
        }

        public static IEnumerable<AssemblyTestsResults> RunTestsInPath(string path)
        {
            return Directory
                .GetFiles(path)
                .Select(Assembly.LoadFrom)
                .Select(RunTestsInAssembly);
        }
    }

    internal class ClassTestsRunner
    {
        private static readonly object[] NoParams = new object[0];

        private readonly Type _type;
        private readonly object _obj;
        private readonly List<MethodInfo> _beforeMethods, _afterMethods;

        public ClassTestsRunner(Type type)
        {
            _type = type;
            _obj = CreateInstance(type);

            _beforeMethods = (
                from method in type.GetMethods()
                where method.GetCustomAttribute<Before>() != null
                select method
            ).ToList();
            _afterMethods = (
                from method in type.GetMethods()
                where method.GetCustomAttribute<After>() != null
                select method
            ).ToList();
                
        }

        private TestResult DoRunTest(MethodInfo testMethod, Test test)
        {
            var stopwatch = new Stopwatch();
            try
            {
                try
                {
                    stopwatch.Start();
                    testMethod.Invoke(_obj, NoParams);
                    if (test.Expected != null)
                        throw new TestDidNotThrowException(testMethod, test.Expected);
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
                return new TestIgnored(testMethod.Name, test.Ignore);
            
            foreach (var method in _beforeMethods)
                try
                {
                    method.Invoke(_obj, NoParams);
                }
                catch (Exception e)
                {
                    throw new SetupMethodThrewException(testMethod, _type, method, e);
                }
            var pendingResult = DoRunTest(testMethod, test);    
            foreach (var method in _afterMethods)
                try
                {
                    method.Invoke(_obj, NoParams);
                }
                catch (Exception e)
                {
                    throw new SetupMethodThrewException(testMethod, _type, method, e);
                }
            return pendingResult;
        }
            
        public ClassTestsResults RunTests()
        {
            foreach (var method in _type
                .GetMethods()
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

            var reports = (
                    from method in _type.GetMethods()
                    where (method.GetCustomAttribute<Test>() != null)
                    select RunTest(method)
            ).ToList();
            
            foreach (var method in _type
                .GetMethods()
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
    }
}