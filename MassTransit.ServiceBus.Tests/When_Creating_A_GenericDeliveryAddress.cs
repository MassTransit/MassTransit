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

        [Test]
        public void Simplyfy_the_constructors()
        {
            MessageQueueEndpoint q = new MessageQueueEndpoint("msmq://localhost/test_endpoint");
            Assert.That(q.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/test_endpoint"));
            Assert.That(q.QueuePath, Is.EqualTo(@"FormatName:DIRECT=OS:" + Environment.MachineName.ToLowerInvariant() + "\\private$\\test_endpoint"));
        }

        [Test]
        public void FromFullQueuePath()
        {
            IMessageQueueEndpoint q = MessageQueueEndpoint.FromQueuePath("FormatName:DIRECT=OS:" + Environment.MachineName + @"\private$\test_endpoint");
            Assert.That(q.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/test_endpoint"));
            Assert.That(q.QueuePath, Is.EqualTo("FormatName:DIRECT=OS:" + Environment.MachineName.ToLowerInvariant() + "\\private$\\test_endpoint"));
        }

        [Test]
        public void FromPartialQueuePath()
        {
            IMessageQueueEndpoint q = MessageQueueEndpoint.FromQueuePath(".\\private$\\test_endpoint");
            Assert.That(q.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/test_endpoint"));
            Assert.That(q.QueuePath, Is.EqualTo("FormatName:DIRECT=OS:" + Environment.MachineName.ToLowerInvariant() + "\\private$\\test_endpoint"));
        }
    }
}