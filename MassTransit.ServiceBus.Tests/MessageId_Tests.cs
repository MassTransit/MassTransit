using System;
using MassTransit.ServiceBus.Util;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class MessageId_Tests
    {
        [Test]
        public void When_comparing_two_MessageId_objects_they_should_be_equal()
        {
            MessageId firstId = Guid.NewGuid() + @"\12345";
            MessageId secondId = firstId;

            Assert.That(firstId, Is.EqualTo(secondId));
        }

        [Test]
        public void A_newly_created_MessageId_should_be_empty()
        {
            MessageId id = new MessageId();

            Assert.That(id, Is.EqualTo(MessageId.Empty));
        }

        [Test]
        public void A_zero_string_should_be_an_empty_MessageId()
        {
            string value = @"{00000000-0000-0000-0000-000000000000}\0";

            MessageId id = value;

            Assert.That(id, Is.EqualTo(MessageId.Empty));
        }
    }
}