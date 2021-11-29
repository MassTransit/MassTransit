namespace MassTransit.Tests.Middleware.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Internals;
    using NUnit.Framework;


    [TestFixture]
    public class Reflecting_over_a_generic_type
    {
        [Test]
        public void Should_not_close_open_generic()
        {
            Assert.IsFalse(typeof(ISingleGeneric<>).ClosesType(typeof(ISingleGeneric<>)));
        }

        [Test]
        public void Should_return_an_enumeration_of_a_constraint_based_generic_interface()
        {
            Type[] types = typeof(NestedDoubleGenericInterface).GetClosingArguments(typeof(INestedDoubleGeneric<,>)).ToArray();

            Assert.AreEqual(2, types.Length);
            Assert.AreEqual(typeof(SingleGenericInterface), types[0]);
            Assert.AreEqual(typeof(int), types[1]);
        }

        [Test]
        public void Should_return_an_enumeration_of_a_deep_double_nested_generic_type()
        {
            Type[] types = typeof(DeepDoubleNestedGeneric).GetClosingArguments(typeof(Dictionary<,>)).ToArray();

            Assert.AreEqual(2, types.Length);
            Assert.AreEqual(typeof(int), types[0]);
            Assert.AreEqual(typeof(string), types[1]);
        }

        [Test]
        public void Should_return_an_enumeration_of_a_deep_single_nested_generic_type()
        {
            Type[] types = typeof(DeepSingleNestedGeneric).GetClosingArguments(typeof(List<>)).ToArray();

            Assert.AreEqual(1, types.Length);
            Assert.AreEqual(typeof(string), types[0]);
        }

        [Test]
        public void Should_return_an_enumeration_of_a_double_generic_interface()
        {
            Type[] types = typeof(DoubleGenericInterface).GetClosingArguments(typeof(IDoubleGeneric<,>)).ToArray();

            Assert.AreEqual(2, types.Length);
            Assert.AreEqual(typeof(int), types[0]);
            Assert.AreEqual(typeof(string), types[1]);
        }

        [Test]
        public void Should_return_an_enumeration_of_a_double_generic_type()
        {
            Type[] types = typeof(Dictionary<int, string>).GetClosingArguments(typeof(Dictionary<,>)).ToArray();

            Assert.AreEqual(2, types.Length);
            Assert.AreEqual(typeof(int), types[0]);
            Assert.AreEqual(typeof(string), types[1]);
        }

        [Test]
        public void Should_return_an_enumeration_of_a_double_nested_generic_type()
        {
            Type[] types = typeof(DoubleNestedGeneric).GetClosingArguments(typeof(Dictionary<,>)).ToArray();

            Assert.AreEqual(2, types.Length);
            Assert.AreEqual(typeof(int), types[0]);
            Assert.AreEqual(typeof(string), types[1]);
        }

        [Test]
        public void Should_return_an_enumeration_of_a_single_generic_interface()
        {
            Type[] types = typeof(SingleGenericInterface).GetClosingArguments(typeof(ISingleGeneric<>)).ToArray();

            Assert.AreEqual(1, types.Length);
            Assert.AreEqual(typeof(int), types[0]);
        }

        [Test]
        public void Should_return_an_enumeration_of_a_single_generic_type()
        {
            Type[] types = typeof(List<string>).GetClosingArguments(typeof(List<>)).ToArray();

            Assert.AreEqual(1, types.Length);
            Assert.AreEqual(typeof(string), types[0]);
        }

        [Test]
        public void Should_return_an_enumeration_of_a_single_nested_generic_type()
        {
            Type[] types = typeof(SingleNestedGeneric).GetClosingArguments(typeof(List<>)).ToArray();

            Assert.AreEqual(1, types.Length);
            Assert.AreEqual(typeof(string), types[0]);
        }


        class SingleNestedGeneric :
            List<string>
        {
        }


        class DeepSingleNestedGeneric :
            SingleNestedGeneric
        {
        }


        class DoubleNestedGeneric :
            Dictionary<int, string>
        {
        }


        class DeepDoubleNestedGeneric :
            DoubleNestedGeneric
        {
        }


        class SingleGenericInterface :
            ISingleGeneric<int>
        {
        }


        interface ISingleGeneric<T>
        {
        }


        class DoubleGenericInterface :
            IDoubleGeneric<int, string>
        {
        }


        interface IDoubleGeneric<T, K>
        {
        }


        class NestedDoubleGenericInterface :
            INestedDoubleGeneric<SingleGenericInterface, int>
        {
        }


        interface INestedDoubleGeneric<T, K>
            where T : ISingleGeneric<K>
        {
        }
    }
}
