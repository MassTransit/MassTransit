namespace MassTransit.ServiceBus.Tests.Formatters
{
    using System;
    using System.Collections.Generic;
    using MassTransit.ServiceBus.Formatters;
    using NUnit.Framework;

    [TestFixture]
    public class MessageFinderTests
    {
        [Test]
        public void NAME()
        {
            List<Type> results = MessageFinder.FindAll();
            Assert.AreEqual(17, results.Count);
        }
    }
}