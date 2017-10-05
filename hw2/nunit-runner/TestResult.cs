using System;

namespace net.ldvsoft.spbau.nunit_runner
{
    public class TestResult
    {
        public string Name { get; }

        protected TestResult(string name)
        {
            Name = name;
        }
    }

    public class TestRunned : TestResult
    {
        public TimeSpan RunnedTimeSpan { get; }

        protected TestRunned(string name, TimeSpan runnedTimeSpan): base(name)
        {
            RunnedTimeSpan = runnedTimeSpan;
        }
    }
    
    public class TestSucceeded : TestRunned
    {
        internal TestSucceeded(String name, TimeSpan runnedTimeSpan) : base(name, runnedTimeSpan)
        {
        }
    }

    public class TestFailed : TestRunned
    {
        public Exception Cause { get; }
        
        internal TestFailed(String name, TimeSpan runnedTimeSpan, Exception cause) : base(name, runnedTimeSpan)
        {
            Cause = cause;
        }
    }

    public class TestIgnored : TestResult
    {
        public String Reason { get; }

        internal TestIgnored(string name, string reason): base(name)
        {
            Reason = reason;
        }        
    }
}