using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyOption;

namespace MyOptionTest
{
    [TestClass]
    public class OptionTest
    {
        private readonly Option<int> _int1 = Option<int>.Some(1);
        private readonly Option<int> _anotherInt1 = Option<int>.Some(1);
        private readonly Option<int> _int2 = Option<int>.Some(2);
        private readonly Option<int> _empty = Option<int>.None();
        private readonly Option<int> _anotherEmpty = Option<int>.None();

        [TestMethod]
        public void NoneOrSomeTest()
        {
            Assert.IsTrue(_int1.IsSome());
            Assert.IsTrue(_empty.IsNone());
            Assert.IsFalse(_int1.IsNone());
            Assert.IsFalse(_empty.IsSome());
        }

        [TestMethod]
        public void ValueTest()
        {
            Assert.AreEqual(1, _int1.Value());
            Assert.ThrowsException<InvalidOperationException>(() => _empty.Value());
        }

        [TestMethod]
        public void EqualityTest()
        {            
            Assert.AreEqual(_int1, _anotherInt1);
            Assert.AreEqual(_int1.GetHashCode(), _anotherInt1.GetHashCode());
            Assert.AreEqual(_empty, _anotherEmpty);
            Assert.AreEqual(_empty.GetHashCode(), _anotherEmpty.GetHashCode());

            Assert.AreNotEqual(_int1, _int2);
            Assert.AreNotEqual(_int1.GetHashCode(), _int2.GetHashCode());
            Assert.AreNotEqual(_int1, _empty);
            Assert.AreNotEqual(_int1.GetHashCode(), _empty.GetHashCode());
        }

        [TestMethod]
        public void MappingTest()
        {
            Assert.AreEqual(Option<int>.None(), Option<int>.None().Map(x => x * 2));
            Assert.AreEqual(Option<int>.Some(2), Option<int>.Some(1).Map(x => x * 2));
            
            Assert.AreEqual(Option<string>.None(), Option<int>.None().Map(x => x.ToString()));
            Assert.AreEqual(Option<string>.Some("2"), Option<int>.Some(2).Map(x => x.ToString()));
        }

        [TestMethod]
        public void NoneToNoneFlattenTest()
        {
            Assert.AreEqual
            (
                Option<int>.None(),
                Option<int>.Flatten(Option<Option<int>>.None())
            );
        }

        [TestMethod]
        public void SomeToNoneFlattenTest()
        {
            Assert.AreEqual
            (
                Option<int>.None(),
                Option<int>.Flatten(Option<Option<int>>.Some(Option<int>.None()))
            );
        }

        [TestMethod]
        public void SomeToSomeFlattenTest()
        {
            Assert.AreEqual
            (
                Option<int>.Some(55),
                Option<int>.Flatten(Option<Option<int>>.Some(Option<int>.Some(55)))
            );
        }
    }
}