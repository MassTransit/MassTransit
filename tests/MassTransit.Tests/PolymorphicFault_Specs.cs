namespace MassTransit.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using PolymorphicFault_Specs;


    [TestFixture]
    public class Test1
    {
        [Test]
        public async Task TestMessageDoesNotHaveInterfaceAndExcludedFromTopologyBaseClassDoes()
        {
            await PerformTest<MessageDoesNotHaveInterfaceAndExcludedFromTopologyBaseClassDoes>();
        }

        [Test]
        public async Task TestMessageDoesNotHaveInterfaceAndIncludedInTopologyBaseHasInterface()
        {
            await PerformTest<MessageDoesNotHaveInterfaceAndIncludedInTopologyBaseHasInterface>();
        }

        [Test]
        public async Task TestMessageHasInterfaceAndDoesNotHaveBaseClass()
        {
            await PerformTest<MessageHasInterfaceAndDoesNotHaveBaseClass>();
        }

        [Test]
        public async Task TestMessageHasInterfaceAndExcludedFromTopologyBaseClassDoesNot()
        {
            await PerformTest<MessageHasInterfaceAndExcludedFromTopologyBaseClassDoesNot>();
        }

        [Test]
        public async Task TestMessageHasInterfaceAndIncludedInTopologyBaseClassDoesNot()
        {
            await PerformTest<MessageHasInterfaceAndIncludedInTopologyBaseClassDoesNot>();
        }

        async Task PerformTest<T>()
            where T : class, new()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<AlwaysFailConsumer<T>>();
                    x.AddConsumer<FaultMessageConsumer<T>>();
                    x.AddConsumer<FaultMessageConsumer<IMyMessageInterface>>();
                })
                .BuildServiceProvider();

            var harness = provider.GetRequiredService<ITestHarness>();

            await harness.Start();

            await harness.Bus.Publish(new T());

            var messagePublished = await harness.Published.Any<T>();
            Assert.That(messagePublished);

            var messageConsumed = await harness.Consumed.Any<T>();
            Assert.That(messageConsumed);

            IReceivedMessage<T> consumedMessage = harness.Consumed.Select<T>().FirstOrDefault();
            Assert.That(consumedMessage, Is.Not.Null);
            Assert.That(consumedMessage.Exception, Is.Not.Null);

            var faultMessagePublished = await harness.Published.Any<Fault<T>>();
            Assert.That(faultMessagePublished);

            var faultInterfaceMessagePublished = await harness.Published.Any<Fault<IMyMessageInterface>>();
            Assert.That(faultInterfaceMessagePublished);

            IConsumerTestHarness<FaultMessageConsumer<T>> consumerHarness = harness.GetConsumerHarness<FaultMessageConsumer<T>>();
            Assert.That(await consumerHarness.Consumed.Any<Fault<T>>());

            IConsumerTestHarness<FaultMessageConsumer<IMyMessageInterface>> interfaceConsumerHarness =
                harness.GetConsumerHarness<FaultMessageConsumer<IMyMessageInterface>>();
            Assert.That(await interfaceConsumerHarness.Consumed.Any<Fault<IMyMessageInterface>>());
        }
    }


    namespace PolymorphicFault_Specs
    {
        using System;


        public interface IMyMessageInterface
        {
            Guid CorrelationId { get; }
        }


        public interface IMyGeneralMessageInterface : IMyMessageInterface
        {
            string Message { get; }
        }


        [ExcludeFromTopology]
        public abstract class BaseClassExcludedFromTopologyHavingNoInterface
        {
            public virtual Guid CorrelationId { get; set; } = Guid.NewGuid();
        }


        [ExcludeFromTopology]
        public abstract class BaseClassExcludedFromTopologyHavingInterface : IMyMessageInterface
        {
            public virtual Guid CorrelationId { get; set; } = Guid.NewGuid();
        }


        public abstract class BaseClassNotExcludedFromTopologyHavingNoInterface
        {
            public virtual Guid CorrelationId { get; set; } = Guid.NewGuid();
        }


        public abstract class BaseClassNotExcludedFromTopologyHavingInterface : IMyMessageInterface
        {
            public virtual Guid CorrelationId { get; set; } = Guid.NewGuid();
        }


        public class MessageHasInterfaceAndExcludedFromTopologyBaseClassDoesNot : BaseClassExcludedFromTopologyHavingNoInterface,
            IMyMessageInterface
        {
            public virtual string Message { get; set; } = string.Empty;
        }


        public class MessageHasInterfaceAndIncludedInTopologyBaseClassDoesNot : BaseClassNotExcludedFromTopologyHavingNoInterface,
            IMyMessageInterface
        {
            public virtual string Message { get; set; } = string.Empty;
        }


        public class MessageDoesNotHaveInterfaceAndExcludedFromTopologyBaseClassDoes : BaseClassExcludedFromTopologyHavingInterface
        {
            public virtual string Message { get; set; } = string.Empty;
        }


        public class MessageDoesNotHaveInterfaceAndIncludedInTopologyBaseHasInterface : BaseClassNotExcludedFromTopologyHavingInterface
        {
            public virtual string Message { get; set; } = string.Empty;
        }


        public class MessageHasInterfaceAndDoesNotHaveBaseClass : IMyMessageInterface
        {
            public virtual string Message { get; set; } = string.Empty;
            public virtual Guid CorrelationId { get; set; } = Guid.NewGuid();
        }


        public class AlwaysFailConsumer<T> : IConsumer<T>
            where T : class
        {
            public Task Consume(ConsumeContext<T> context)
            {
                throw new Exception("Fail on purpose");
            }
        }


        public class FaultMessageConsumer<T> : IConsumer<Fault<T>>
            where T : class
        {
            public Task Consume(ConsumeContext<Fault<T>> context)
            {
                Console.WriteLine($"Consuming Fault<{typeof(T)}>");
                return Task.CompletedTask;
            }
        }


        public class Test : IConsumer<Fault<MessageHasInterfaceAndExcludedFromTopologyBaseClassDoesNot>>
        {
            public Task Consume(ConsumeContext<Fault<MessageHasInterfaceAndExcludedFromTopologyBaseClassDoesNot>> context)
            {
                Console.WriteLine("Consuming Fault<MessageHasInterfaceAndExcludedFromTopologyBaseClassDoesNot>");
                return Task.CompletedTask;
            }
        }
    }
}
