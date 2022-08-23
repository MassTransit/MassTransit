using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MassTransit.Abstractions.Tests
{
    [TestFixture]
    public class Using_the_TypeExtensions
    {
        [Test]
        public void Should_accept_multiple_this_operators()
        {
            var typeInfo = typeof(TypeWithMultipleThisOperators).GetTypeInfo();
            Assert.That(() => Internals.TypeExtensions.GetAllProperties(typeInfo).ToList(), Throws.Nothing);
        }
    }

    internal class TypeWithMultipleThisOperators
    {
        public string FirstValue { get; } = "aValue";
        public string SecondValue { get; } = "anotherValue";

        public string this[int index] => index switch
        {
            0 => FirstValue,
            1 => SecondValue,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        public string this[string name] => name.ToLower() switch
        {
            "first" => FirstValue,
            "second" => SecondValue,
            _ => throw new ArgumentOutOfRangeException(nameof(name))
        };


    }
}
