using System;

namespace MassTransit.ServiceBus.Tests
{
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class When_Creating_A_GenericDeliveryAddress
    {
        [Test]
        public void AutoCast_From_String()
        {
            MessageQueueEndpoint addr = @"msmq://localhost/test_endpoint";
			Assert.That(addr.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/test_endpoint"));
        }
    }
}