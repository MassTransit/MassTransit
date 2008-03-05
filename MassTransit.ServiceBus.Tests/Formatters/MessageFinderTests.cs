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
            List<Type> results = MessageFinder.AllMessageTypes();
            Assert.AreEqual(12, results.Count);
        }
    }
}