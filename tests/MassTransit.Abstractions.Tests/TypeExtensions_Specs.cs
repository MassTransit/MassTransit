namespace MassTransit.Abstractions.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Internals;
    using NUnit.Framework;


    [TestFixture]
    public class Using_the_type_extensions
    {
        [Test]
        public void Should_accept_multiple_this_operators()
        {
            var type = typeof(TypeWithMultipleThisOperators);
            Assert.That(() => type.GetAllProperties().ToList(), Throws.Nothing);
        }

        [Test]
        public void Should_accept_multiple_this_operators_subclass()
        {
            var type = typeof(SubclassWithThisOperator);
            Assert.That(() => type.GetAllProperties().ToList(), Throws.Nothing);
        }
    }


    class TypeWithMultipleThisOperators
    {
        public string FirstValue { get; } = "aValue";
        public string SecondValue { get; } = "anotherValue";

        public string this[int index] =>
            index switch
            {
                0 => FirstValue,
                1 => SecondValue,
                _ => throw new ArgumentOutOfRangeException(nameof(index))
            };

        public string this[string name] =>
            name.ToLower() switch
            {
                "first" => FirstValue,
                "second" => SecondValue,
                _ => throw new ArgumentOutOfRangeException(nameof(name))
            };
    }


    class ClassWithThisOperator
    {
        public string FirstValue { get; } = "aValue";
        public string SecondValue { get; } = "anotherValue";

        public string this[int index] =>
            index switch
            {
                0 => FirstValue,
                1 => SecondValue,
                _ => throw new ArgumentOutOfRangeException(nameof(index))
            };
    }


    class SubclassWithThisOperator :
        ClassWithThisOperator
    {
        public string this[string name] =>
            name.ToLower() switch
            {
                "first" => FirstValue,
                "second" => SecondValue,
                _ => throw new ArgumentOutOfRangeException(nameof(name))
            };
    }
}
