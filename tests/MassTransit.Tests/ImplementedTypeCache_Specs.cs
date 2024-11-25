namespace MassTransit.Tests;

using System;
using System.Collections.Generic;
using Metadata;
using NUnit.Framework;


[TestFixture]
public class ImplementedTypeCache_Specs
{
    [Test]
    public void It_should_be_able_to_get_an_interface()
    {
        var collector = new ImplementedTypeCollector();

        ImplementedMessageTypeCache<ThirdInterface>.EnumerateImplementedTypes(collector);

        Assert.That(collector.ImplementedTypes.Count, Is.EqualTo(1));
    }


    class ImplementedTypeCollector :
        IImplementedMessageType
    {
        public List<Type> ImplementedTypes = new();

        public void ImplementsMessageType<T>(bool direct)
            where T : class
        {
            ImplementedTypes.Add(typeof(T));
        }
    }


    public interface SingleInterface
    {
    }


    public interface FirstInterface
    {
    }


    public interface SecondInterface :
        FirstInterface
    {
    }


    public interface ThirdInterface :
        SecondInterface
    {
    }


    public interface AnotherThirdInterface :
        SecondInterface
    {
    }
}
