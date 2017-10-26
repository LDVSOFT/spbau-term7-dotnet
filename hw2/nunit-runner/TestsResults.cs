using System.Collections.Generic;

namespace Net.LDVSoft.SPbAU.NUnitRunner
{
    public class ClassTestsResults
    {
        public string Name { get; }
        public List<TestResult> Reports { get; }

        internal ClassTestsResults(string name, List<TestResult> reports)
        {
            Name = name;
            Reports = reports;
        }
    }

    public class AssemblyTestsResults
    {
        public string Name { get; }
        public List<ClassTestsResults> Reports { get; }

        internal AssemblyTestsResults(string name, List<ClassTestsResults> reports)
        {
            Name = name;
            Reports = reports;
        }
    }
}