using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Net.LDVSoft.SPbAU.NUnit;

namespace Net.LDVSoft.SPbAU.NUnitRunner
{
    public static class TestRunner
    {
        public static ClassTestsResults RunTestsInClass(Type type)
        {
            return new ClassTestsRunner(type).RunTests();
        }

        public static AssemblyTestsResults RunTestsInAssembly(Assembly assembly)
        {
            var classTestsReports = assembly.DefinedTypes
                .Where(type => type.GetRuntimeMethods()
                    .Any(ClassTestsRunner.CheckAttributeForExistense<Test>))
                .Select(RunTestsInClass)
                .ToList();
            return new AssemblyTestsResults(assembly.FullName, classTestsReports);
        }

        public static IEnumerable<AssemblyTestsResults> RunTestsInPath(string path) => Directory.GetFiles(path)
            .Select(it =>
            {
                try
                {
                    return Assembly.LoadFrom(it);
                }
                catch (BadImageFormatException)
                {
                    return null;
                }
            })
            .Where(it => it != null)
            .Select(RunTestsInAssembly);
    }
}