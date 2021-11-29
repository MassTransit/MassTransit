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
            Assert.IsTrue(typeof(NonGenericSubClass).HasInterface<IGeneric<int>>());
        }

        [Test]
        public void Should_match_a_generic_interface()
        {
            Assert.IsTrue(typeof(GenericClass).HasInterface<IGeneric<int>>());
        }

        [Test]
        public void Should_match_a_regular_interface_by_type_argument_on_an_object()
        {
            Assert.IsTrue(typeof(GenericClass).HasInterface(typeof(INotGeneric)));
        }

        [Test]
        public void Should_match_a_regular_interface_on_an_object()
        {
            Assert.IsTrue(typeof(GenericClass).HasInterface<INotGeneric>());
        }

        [Test]
        public void Should_match_a_regular_interface_using_the_generic_argument()
        {
            Assert.IsTrue(typeof(GenericClass).HasInterface<INotGeneric>());
        }

        [Test]
        public void Should_match_a_regular_interface_using_the_generic_argument_on_a_subclass()
        {
            Assert.IsTrue(typeof(GenericSubClass).HasInterface<INotGeneric>());
        }

        [Test]
        public void Should_match_a_regular_interface_using_the_type_argument()
        {
            Assert.IsTrue(typeof(GenericClass).HasInterface(typeof(INotGeneric)));
        }

        [Test]
        public void Should_match_a_regular_interface_using_the_type_argument_on_a_subclass()
        {
            Assert.IsTrue(typeof(GenericSubClass).HasInterface(typeof(INotGeneric)));
        }

        [Test]
        public void Should_match_an_open_generic_interface()
        {
            Assert.IsTrue(typeof(GenericClass).HasInterface(typeof(IGeneric<>)));
        }

        [Test]
        public void Should_match_an_open_generic_interface_in_a_base_class()
        {
            Assert.IsTrue(typeof(NonGenericSubClass).HasInterface(typeof(IGeneric<>)));
        }

        [Test]
        public void Should_not_match_a_regular_interface_that_is_not_implemented()
        {
            Assert.IsFalse(typeof(GenericClass).HasInterface<IDisposable>());
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
