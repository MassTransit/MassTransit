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
            Assert.That(typeof(ISingleGeneric<>).ClosesType(typeof(ISingleGeneric<>)), Is.False);
        }

        [Test]
        public void Should_return_an_enumeration_of_a_constraint_based_generic_interface()
        {
            Type[] types = typeof(NestedDoubleGenericInterface).GetClosingArguments(typeof(INestedDoubleGeneric<,>)).ToArray();

            Assert.That(types, Has.Length.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(types[0], Is.EqualTo(typeof(SingleGenericInterface)));
                Assert.That(types[1], Is.EqualTo(typeof(int)));
            });
        }

        [Test]
        public void Should_return_an_enumeration_of_a_deep_double_nested_generic_type()
        {
            Type[] types = typeof(DeepDoubleNestedGeneric).GetClosingArguments(typeof(Dictionary<,>)).ToArray();

            Assert.That(types, Has.Length.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(types[0], Is.EqualTo(typeof(int)));
                Assert.That(types[1], Is.EqualTo(typeof(string)));
            });
        }

        [Test]
        public void Should_return_an_enumeration_of_a_deep_single_nested_generic_type()
        {
            Type[] types = typeof(DeepSingleNestedGeneric).GetClosingArguments(typeof(List<>)).ToArray();

            Assert.That(types, Has.Length.EqualTo(1));
            Assert.That(types[0], Is.EqualTo(typeof(string)));
        }

        [Test]
        public void Should_return_an_enumeration_of_a_double_generic_interface()
        {
            Type[] types = typeof(DoubleGenericInterface).GetClosingArguments(typeof(IDoubleGeneric<,>)).ToArray();

            Assert.That(types, Has.Length.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(types[0], Is.EqualTo(typeof(int)));
                Assert.That(types[1], Is.EqualTo(typeof(string)));
            });
        }

        [Test]
        public void Should_return_an_enumeration_of_a_double_generic_type()
        {
            Type[] types = typeof(Dictionary<int, string>).GetClosingArguments(typeof(Dictionary<,>)).ToArray();

            Assert.That(types, Has.Length.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(types[0], Is.EqualTo(typeof(int)));
                Assert.That(types[1], Is.EqualTo(typeof(string)));
            });
        }

        [Test]
        public void Should_return_an_enumeration_of_a_double_nested_generic_type()
        {
            Type[] types = typeof(DoubleNestedGeneric).GetClosingArguments(typeof(Dictionary<,>)).ToArray();

            Assert.That(types, Has.Length.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(types[0], Is.EqualTo(typeof(int)));
                Assert.That(types[1], Is.EqualTo(typeof(string)));
            });
        }

        [Test]
        public void Should_return_an_enumeration_of_a_single_generic_interface()
        {
            Type[] types = typeof(SingleGenericInterface).GetClosingArguments(typeof(ISingleGeneric<>)).ToArray();

            Assert.That(types, Has.Length.EqualTo(1));
            Assert.That(types[0], Is.EqualTo(typeof(int)));
        }

        [Test]
        public void Should_return_an_enumeration_of_a_single_generic_type()
        {
            Type[] types = typeof(List<string>).GetClosingArguments(typeof(List<>)).ToArray();

            Assert.That(types, Has.Length.EqualTo(1));
            Assert.That(types[0], Is.EqualTo(typeof(string)));
        }

        [Test]
        public void Should_return_an_enumeration_of_a_single_nested_generic_type()
        {
            Type[] types = typeof(SingleNestedGeneric).GetClosingArguments(typeof(List<>)).ToArray();

            Assert.That(types, Has.Length.EqualTo(1));
            Assert.That(types[0], Is.EqualTo(typeof(string)));
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
