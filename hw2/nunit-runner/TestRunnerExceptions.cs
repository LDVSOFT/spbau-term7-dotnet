using System;
using System.Reflection;

namespace Net.LDVSoft.SPbAU.NUnitRunner
{
    public class TestDidNotThrowExpectedException
        : Exception
    {
        public MethodInfo TestMethod { get; }
        public Type ExpectedExceptionType { get; }
        public Exception ActualException { get; }

        internal TestDidNotThrowExpectedException(MethodInfo testMethod, Type exceptionType, Exception actualException)
        {
            TestMethod = testMethod;
            ExpectedExceptionType = exceptionType;
            ActualException = actualException;
        }

        public override string Message
        {
            get
            {
                string suffix;
                if (!(ActualException is null))
                {
                    suffix = $"Instead, it threw: {ActualException.Message}.";
                }
                else
                {
                    suffix = "Instead, it threw nothing.";
                }

                return $"Test method {TestMethod.Name} in class {TestMethod.DeclaringType} did not throw "
                       + $"expected exception of type {ExpectedExceptionType.FullName}. {suffix}";
            }
        }
    }

    public class SetupMethodThrewException
        : Exception
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
            $"While trying to run test {TestMethod.Name} in class {TestType.FullName}, "
            + $"before/after method {FailedMethod.Name} threw: {Inner.Message}";
    }

    public class ClassSetupMethodThrewException
        : Exception
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
            $"While trying to run tests in class {TestType.DeclaringType}, "
            + $"before/after method {FailedMethod.Name} threw: {Inner.Message}";
    }
}