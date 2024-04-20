namespace MassTransit.Tests.Middleware.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Internals;
    using NUnit.Framework;


    [TestFixture]
    public class When_getting_the_generic_types_from_an_interface
    {
        [Test]
        public void Should_close_generic_type()
        {
            Assert.That(typeof(GenericClass).ClosesType(typeof(IGeneric<>)), Is.True);
        }

        [Test]
        public void Should_not_close_nested_open_generic_base_class()
        {
            Assert.That(typeof(SuperGenericBaseClass<>).ClosesType(typeof(GenericBaseClass<>)), Is.False);
        }

        [Test]
        public void Should_not_close_nested_open_generic_interface_in_base_class()
        {
            Assert.That(typeof(SuperGenericBaseClass<>).ClosesType(typeof(IGeneric<>)), Is.False);
        }

        [Test]
        public void Should_not_close_open_generic_type()
        {
            Assert.That(typeof(GenericBaseClass<>).ClosesType(typeof(IGeneric<>)), Is.False);
        }

        [Test]
        public void Should_not_have_closing_arguments_for_a_class_that_isnt_closed()
        {
            IEnumerable<Type> types = typeof(SuperGenericBaseClass<>).GetClosingArguments(typeof(IGeneric<>));

            Assert.That(types.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Should_return_the_appropriate_generic_type()
        {
            IEnumerable<Type> types = typeof(GenericClass).GetClosingArguments(typeof(IGeneric<>)).ToArray();

            Assert.Multiple(() =>
            {
                Assert.That(types.Count(), Is.EqualTo(1));
                Assert.That(types.First(), Is.EqualTo(typeof(int)));
            });
        }

        [Test]
        public void Should_return_the_appropriate_generic_type_for_a_subclass_non_generic()
        {
            IEnumerable<Type> types = typeof(SubClass).GetClosingArguments(typeof(IGeneric<>)).ToArray();

            Assert.Multiple(() =>
            {
                Assert.That(types.Count(), Is.EqualTo(1));
                Assert.That(types.First(), Is.EqualTo(typeof(int)));
            });
        }

        [Test]
        public void Should_return_the_appropriate_generic_type_with_a_generic_base_class()
        {
            IEnumerable<Type> types = typeof(NonGenericSubClass).GetClosingArguments(typeof(IGeneric<>)).ToArray();

            Assert.Multiple(() =>
            {
                Assert.That(types.Count(), Is.EqualTo(1));
                Assert.That(types.First(), Is.EqualTo(typeof(int)));
            });
        }

        [Test]
        public void Should_return_the_generic_type_from_a_class()
        {
            IEnumerable<Type> types = typeof(NonGenericSubClass).GetClosingArguments(typeof(GenericBaseClass<>)).ToArray();

            Assert.Multiple(() =>
            {
                Assert.That(types.Count(), Is.EqualTo(1));
                Assert.That(types.First(), Is.EqualTo(typeof(int)));
            });
        }


        interface IGeneric<T>
        {
        }


        class GenericClass :
            IGeneric<int>
        {
        }


        class SubClass :
            GenericClass
        {
        }


        class SuperGenericBaseClass<T> :
            GenericBaseClass<T>
        {
        }


        class GenericBaseClass<T> :
            IGeneric<T>
        {
        }


        class NonGenericSubClass :
            GenericBaseClass<int>
        {
        }
    }
}
