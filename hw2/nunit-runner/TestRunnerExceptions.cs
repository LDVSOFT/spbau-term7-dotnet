using System;
using System.Reflection;

namespace net.ldvsoft.spbau.nunit_runner
{
    public class TestDidNotThrowException : Exception
    {
        public MethodInfo TestMethod { get; }
        public Type ExpectedExceptionType { get; }

        internal TestDidNotThrowException(MethodInfo testMethod, Type exceptionType)
        {
            TestMethod = testMethod;
            ExpectedExceptionType = exceptionType;
        }

        public override string Message => 
            $"Test method ${TestMethod.Name} in class ${TestMethod.DeclaringType} did not throw "
            + $"expected exception of type ${ExpectedExceptionType.FullName}.";
    }

    public class SetupMethodThrewException : Exception
    {
        public MethodInfo TestMethod { get; }
        public Type TestType { get; }
        public MethodInfo FailedMethod { get; }
        public Exception Inner { get; }
        
        internal SetupMethodThrewException(MethodInfo testMethod, Type testType, MethodInfo failedMethod, Exception inner)
        {
            TestMethod = testMethod;
            TestType = testType;
            FailedMethod = failedMethod;
            Inner = inner;
        }

        public override string Message =>
            $"While trying to run test ${TestMethod.Name} in class ${TestType.FullName}, " 
            + $"before/after method ${FailedMethod.Name} threw: ${Inner.Message}";
    }
    
    public class ClassSetupMethodThrewException : Exception
    {
        public Type TestType { get; }
        public MethodInfo FailedMethod { get; }
        public Exception Inner { get; }
        
        internal ClassSetupMethodThrewException(Type testType, MethodInfo failedMethod, Exception inner)
        {
            TestType = testType;
            FailedMethod = failedMethod;
            Inner = inner;
        }

        public override string Message =>
            $"While trying to run tests in class ${TestType.DeclaringType}, " 
            + $"before/after method ${FailedMethod.Name} threw: ${Inner.Message}";
    }
}