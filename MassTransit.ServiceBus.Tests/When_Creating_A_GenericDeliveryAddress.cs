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
            MessageQueueEndpoint addr = @".\private$\test_endpoint";
            Assert.That(addr.Transport.Address, Is.EqualTo(Environment.MachineName + @"\private$\test_endpoint"));
        }
    }
}