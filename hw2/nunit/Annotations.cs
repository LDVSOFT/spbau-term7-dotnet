using System;

namespace net.ldvsoft.spbau.nunit
{
    [AttributeUsage(AttributeTargets.Method)]
    public class Test: Attribute
    {
        public string Ignore { get; set; }
        public Type Expected { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class Before: Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class After: Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class BeforeClass: Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AfterClass: Attribute
    {
    }
}