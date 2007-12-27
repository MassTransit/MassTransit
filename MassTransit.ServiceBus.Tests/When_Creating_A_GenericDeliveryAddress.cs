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
            MessageQueueEndpoint addr = "bob";
            Assert.That(addr.Transport.Address, Is.EqualTo("bob"));
        }
    }
}