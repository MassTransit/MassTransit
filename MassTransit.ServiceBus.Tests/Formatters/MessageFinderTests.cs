namespace MassTransit.ServiceBus.Tests.Formatters
{
    using System;
    using System.Collections.Generic;
    using MassTransit.ServiceBus.Formatters;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
    public class MessageFinderTests
    {
        [Test]
        public void Can_I_Find_Messages()
        {
            List<Type> results = MessageFinder.AllMessageTypes();
			Assert.That(results.Count, Is.GreaterThan(0));
        }
    }
}