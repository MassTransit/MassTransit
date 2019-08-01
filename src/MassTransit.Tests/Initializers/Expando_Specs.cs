namespace MassTransit.Tests.Initializers
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Threading.Tasks;
    using MassTransit.Initializers;
    using MassTransit.Initializers.PropertyInitializers;
    using NUnit.Framework;


    [TestFixture]
    public class Initializing_using_a_dictionary
    {
        [Test]
        public void Should_have_an_interface_from_dictionary_converter()
        {
            Assert.IsTrue(PropertyInitializerCache.TryGetFactory<MessageContract>(typeof(IDictionary<string, object>), out var propertyConverter));
        }

        [Test]
        public async Task Should_work_with_a_dictionary()
        {
            IDictionary<string, object> dto = new Dictionary<string, object>();
            dto.Add(nameof(MessageContract.Id), 27);
            dto.Add(nameof(MessageContract.CustomerId), "SuperMart");

            var message = await MessageInitializerCache<MessageContract>.Initialize(dto);

            Assert.That(message.Message.Id, Is.EqualTo(27));
            Assert.That(message.Message.CustomerId, Is.EqualTo("SuperMart"));
        }

        [Test]
        public async Task Should_work_with_a_dictionary_sourced_object_property()
        {
            IDictionary<string, object> dto = new Dictionary<string, object>();
            dto.Add(nameof(MessageContract.Id), 27);
            dto.Add(nameof(MessageContract.CustomerId), "SuperMart");

            var message = await MessageInitializerCache<MessageEnvelope>.Initialize(new
            {
                Contract = dto
            });

            Assert.That(message.Message.Contract, Is.Not.Null);
            Assert.That(message.Message.Contract.Id, Is.EqualTo(27));
            Assert.That(message.Message.Contract.CustomerId, Is.EqualTo("SuperMart"));
        }

        [Test]
        public async Task Should_do_the_right_thing()
        {
            var uniqueId = Guid.NewGuid();
            IDictionary<string, object> dto = new ExpandoObject();
            dto.Add(nameof(MessageContract.Id), 27);
            dto.Add(nameof(MessageContract.CustomerId), "SuperMart");
            dto.Add(nameof(MessageContract.UniqueId), uniqueId);

            var message = await MessageInitializerCache<MessageContract>.Initialize(dto);

            Assert.That(message.Message.Id, Is.EqualTo(27));
            Assert.That(message.Message.CustomerId, Is.EqualTo("SuperMart"));
            Assert.That(message.Message.UniqueId, Is.EqualTo(uniqueId));
        }


        public interface MessageContract
        {
            int Id { get; }
            string CustomerId { get; }
            Guid UniqueId { get; }
        }


        public interface MessageEnvelope
        {
            MessageContract Contract { get; }
        }
    }
}
