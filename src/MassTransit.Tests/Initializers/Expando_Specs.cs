namespace MassTransit.Tests.Initializers
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Threading.Tasks;
    using MassTransit.Initializers;
    using MassTransit.Initializers.PropertyProviders;
    using Newtonsoft.Json;
    using NUnit.Framework;


    [TestFixture]
    public class Initializing_using_a_dictionary
    {
        [Test]
        public void Should_have_an_interface_from_dictionary_converter()
        {
            var factory = new PropertyProviderFactory<IDictionary<string, object>>();
            Assert.IsTrue(factory.TryGetPropertyConverter(out IPropertyConverter<MessageContract, object> converter));
        }

        [Test]
        public async Task Should_work_with_a_dictionary()
        {
            var uniqueId = Guid.NewGuid();
            IDictionary<string, object> dto = new Dictionary<string, object>();
            dto.Add(nameof(MessageContract.Id), 27);
            dto.Add(nameof(MessageContract.CustomerId), "SuperMart");
            dto.Add(nameof(MessageContract.UniqueId), uniqueId);

            var message = await MessageInitializerCache<MessageContract>.Initialize(dto);

            Assert.That(message.Message.Id, Is.EqualTo(27));
            Assert.That(message.Message.CustomerId, Is.EqualTo("SuperMart"));
            Assert.That(message.Message.UniqueId, Is.EqualTo(uniqueId));
        }

        [Test]
        public async Task Should_work_with_a_list()
        {
            var dto = new
            {
                Id = 32,
                CustomerId = "CustomerXp",
                Product = new
                {
                    Name = "Foo",
                    Category = "Bar"
                },
                Orders = new[]
                {
                    new
                    {
                        Id = Guid.NewGuid(),
                        Product = new
                        {
                            Name = "Product",
                            Category = "Category"
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };

            var expando = JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(dto));

            var message = await MessageInitializerCache<MessageContract>.Initialize(expando); // doesn't work (orders not included)

            Assert.That(message.Message.Id, Is.EqualTo(32));
            Assert.That(message.Message.Orders, Is.Not.Null);
        }

        [Test]
        public async Task Should_work_with_a_dictionary_sourced_object_property()
        {
            var uniqueId = Guid.NewGuid();
            IDictionary<string, object> dto = new Dictionary<string, object>();
            dto.Add(nameof(MessageContract.Id), 27);
            dto.Add(nameof(MessageContract.CustomerId), "SuperMart");
            dto.Add(nameof(MessageContract.UniqueId), uniqueId);

            var message = await MessageInitializerCache<MessageEnvelope>.Initialize(new {Contract = dto});

            Assert.That(message.Message.Contract, Is.Not.Null);
            Assert.That(message.Message.Contract.Id, Is.EqualTo(27));
            Assert.That(message.Message.Contract.CustomerId, Is.EqualTo("SuperMart"));
            Assert.That(message.Message.Contract.UniqueId, Is.EqualTo(uniqueId));
        }

        [Test]
        public async Task Should_do_the_right_thing()
        {
            var uniqueId = Guid.NewGuid();
            IDictionary<string, object> dto = new ExpandoObject();
            dto.Add(nameof(MessageContract.Id), 27);
            dto.Add(nameof(MessageContract.CustomerId), "SuperMart");
            dto.Add(nameof(MessageContract.UniqueId), uniqueId);
            dto.Add(nameof(MessageContract.CustomerType), (long)1);
            dto.Add(nameof(MessageContract.TypeByName), "Internal");

            var message = await MessageInitializerCache<MessageContract>.Initialize(dto);

            Assert.That(message.Message.Id, Is.EqualTo(27));
            Assert.That(message.Message.CustomerId, Is.EqualTo("SuperMart"));
            Assert.That(message.Message.UniqueId, Is.EqualTo(uniqueId));
            Assert.That(message.Message.CustomerType, Is.EqualTo(CustomerType.Public));
            Assert.That(message.Message.TypeByName, Is.EqualTo(CustomerType.Internal));
        }


        public interface MessageContract
        {
            int Id { get; }
            string CustomerId { get; }
            Guid UniqueId { get; }
            CustomerType CustomerType { get; }
            CustomerType TypeByName { get; }
            List<IOrder> Orders { get; }
        }


        public enum CustomerType
        {
            Public = 1,
            Internal = 2
        }


        public interface IOrder
        {
            Guid Id { get; }
            IProduct Product { get; }
            int Quantity { get; }
        }


        public interface IProduct
        {
            string Name { get; }
            string Category { get; }
        }


        public interface MessageEnvelope
        {
            MessageContract Contract { get; }
        }
    }
}
