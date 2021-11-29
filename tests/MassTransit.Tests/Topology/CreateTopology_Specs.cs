namespace MassTransit.Tests.Topology
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;


    [TestFixture]
    public class Creating_a_topology
    {
        [Test]
        public async Task Should_have_a_familiar_syntax()
        {
            MessageCorrelation.UseCorrelationId<LegacyMessage>(x => x.TransactionId);

            var harness = new InMemoryTestHarness();
            harness.OnConfigureInMemoryBus += configurator =>
            {
                configurator.Send<IEvent>(x =>
                {
                    x.UseCorrelationId(p => p.TransactionId);
                });
            };

            Task<ConsumeContext<INewUserEvent>> handled = null;
            Task<ConsumeContext<OtherMessage>> otherHandled = null;
            Task<ConsumeContext<LegacyMessage>> legacyHandled = null;
            harness.OnConfigureInMemoryReceiveEndpoint += configurator =>
            {
                handled = harness.Handled<INewUserEvent>(configurator);
                otherHandled = harness.Handled<OtherMessage>(configurator);
                legacyHandled = harness.Handled<LegacyMessage>(configurator);
            };


            await harness.Start();
            try
            {
                var transactionId = NewId.NextGuid();

                await harness.InputQueueSendEndpoint.Send<INewUserEvent>(new { TransactionId = transactionId });

                ConsumeContext<INewUserEvent> context = await handled;

                Assert.IsTrue(context.CorrelationId.HasValue);
                Assert.That(context.CorrelationId.Value, Is.EqualTo(transactionId));

                await harness.InputQueueSendEndpoint.Send<OtherMessage>(new { CorrelationId = transactionId });

                ConsumeContext<OtherMessage> otherContext = await otherHandled;

                Assert.IsTrue(otherContext.CorrelationId.HasValue);
                Assert.That(otherContext.CorrelationId.Value, Is.EqualTo(transactionId));

                await harness.InputQueueSendEndpoint.Send<LegacyMessage>(new { TransactionId = transactionId });

                ConsumeContext<LegacyMessage> legacyContext = await legacyHandled;

                Assert.IsTrue(legacyContext.CorrelationId.HasValue);
                Assert.That(legacyContext.CorrelationId.Value, Is.EqualTo(transactionId));
            }
            finally
            {
                await harness.Stop();
            }
        }


        public interface IEvent
        {
            Guid TransactionId { get; }
        }


        public interface INewUserEvent :
            IEvent
        {
        }


        public class OtherMessage
        {
            public Guid CorrelationId { get; set; }
        }


        public class LegacyMessage
        {
            public Guid TransactionId { get; set; }
        }
    }
}
