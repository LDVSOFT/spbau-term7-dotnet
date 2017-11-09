using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyOption;

namespace MyOptionTest
{
    [TestClass]
    public class OptionTest
    {
        [TestMethod]
        public void SimpleTest()
        {
            var int1 = Option<int>.Some(1);
            var anotherInt1 = Option<int>.Some(1);
            var int2 = Option<int>.Some(2);
            var empty = Option<int>.None();
            var anotherEmpty = Option<int>.None();

            Assert.IsTrue(int1.IsSome());
            Assert.IsTrue(empty.IsNone());
            Assert.IsFalse(int1.IsNone());
            Assert.IsFalse(empty.IsSome());
            
            Assert.AreEqual(1, int1.Value());
            Assert.ThrowsException<InvalidOperationException>(() => empty.Value());
            
            Assert.AreEqual(int1, anotherInt1);
            Assert.AreEqual(int1.GetHashCode(), anotherInt1.GetHashCode());
            Assert.AreEqual(empty, anotherEmpty);
            Assert.AreEqual(empty.GetHashCode(), anotherEmpty.GetHashCode());

            Assert.AreNotEqual(int1, int2);
            Assert.AreNotEqual(int1.GetHashCode(), int2.GetHashCode());
            Assert.AreNotEqual(int1, empty);
            Assert.AreNotEqual(int1.GetHashCode(), empty.GetHashCode());
        }

        [TestMethod]
        public void TestMapping()
        {
            Assert.AreEqual(Option<int>.None(), Option<int>.None().Map(x => x * 2));
            Assert.AreEqual(Option<int>.Some(2), Option<int>.Some(1).Map(x => x * 2));
            
            Assert.AreEqual(Option<string>.None(), Option<int>.None().Map(x => x.ToString()));
            Assert.AreEqual(Option<string>.Some("2"), Option<int>.Some(2).Map(x => x.ToString()));
        }

        [TestMethod]
        public void TestFlatten()
        {
            Assert.AreEqual
            (
                Option<int>.None(), 
                Option<int>.Flatten(Option<Option<int>>.None())
            );
            Assert.AreEqual
            (
                Option<int>.None(), 
                Option<int>.Flatten(Option<Option<int>>.Some(Option<int>.None()))
            );
            Assert.AreEqual
            (
                Option<int>.Some(55),
                Option<int>.Flatten(Option<Option<int>>.Some(Option<int>.Some(55)))
            );
        }
    }
}