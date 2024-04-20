namespace MassTransit.Tests.Middleware.Internals
{
    using System;
    using MassTransit.Internals;
    using NUnit.Framework;


    [TestFixture]
    public class An_object_that_implements_a_generic_interface
    {
        [Test]
        public void Should_match_a_generic_base_class_implementation_of_the_interface()
        {
            Assert.That(typeof(NonGenericSubClass).HasInterface<IGeneric<int>>(), Is.True);
        }

        [Test]
        public void Should_match_a_generic_interface()
        {
            Assert.That(typeof(GenericClass).HasInterface<IGeneric<int>>(), Is.True);
        }

        [Test]
        public void Should_match_a_regular_interface_by_type_argument_on_an_object()
        {
            Assert.That(typeof(GenericClass).HasInterface(typeof(INotGeneric)), Is.True);
        }

        [Test]
        public void Should_match_a_regular_interface_on_an_object()
        {
            Assert.That(typeof(GenericClass).HasInterface<INotGeneric>(), Is.True);
        }

        [Test]
        public void Should_match_a_regular_interface_using_the_generic_argument()
        {
            Assert.That(typeof(GenericClass).HasInterface<INotGeneric>(), Is.True);
        }

        [Test]
        public void Should_match_a_regular_interface_using_the_generic_argument_on_a_subclass()
        {
            Assert.That(typeof(GenericSubClass).HasInterface<INotGeneric>(), Is.True);
        }

        [Test]
        public void Should_match_a_regular_interface_using_the_type_argument()
        {
            Assert.That(typeof(GenericClass).HasInterface(typeof(INotGeneric)), Is.True);
        }

        [Test]
        public void Should_match_a_regular_interface_using_the_type_argument_on_a_subclass()
        {
            Assert.That(typeof(GenericSubClass).HasInterface(typeof(INotGeneric)), Is.True);
        }

        [Test]
        public void Should_match_an_open_generic_interface()
        {
            Assert.That(typeof(GenericClass).HasInterface(typeof(IGeneric<>)), Is.True);
        }

        [Test]
        public void Should_match_an_open_generic_interface_in_a_base_class()
        {
            Assert.That(typeof(NonGenericSubClass).HasInterface(typeof(IGeneric<>)), Is.True);
        }

        [Test]
        public void Should_not_match_a_regular_interface_that_is_not_implemented()
        {
            Assert.That(typeof(GenericClass).HasInterface<IDisposable>(), Is.False);
        }


        interface INotGeneric
        {
        }


        interface IGeneric<T>
        {
        }


        class GenericClass :
            IGeneric<int>,
            INotGeneric
        {
        }


        class GenericSubClass :
            GenericClass
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
