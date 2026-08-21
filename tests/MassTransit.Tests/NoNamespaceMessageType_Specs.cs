using System;
using System.Threading.Tasks;
using MassTransit;


// These types are deliberately declared without a namespace, to cover message types
// declared in files that do not use a namespace declaration.
class NoNamespaceMessage
{
    public int Value { get; set; }
}


class NoNamespaceMessageConsumer :
    IConsumer<NoNamespaceMessage>
{
    public Task Consume(ConsumeContext<NoNamespaceMessage> context)
    {
        return Task.CompletedTask;
    }
}


class NoNamespaceScope
{
    public class NestedMessage
    {
        public int Value { get; set; }
    }


    public class NestedMessageConsumer :
        IConsumer<NestedMessage>
    {
        public Task Consume(ConsumeContext<NestedMessage> context)
        {
            return Task.CompletedTask;
        }
    }
}


class NoNamespaceSagaScope
{
    public class Start :
        CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
    }


    public class Instance :
        SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
    }


    public class Machine :
        MassTransitStateMachine<Instance>
    {
        public Machine()
        {
            InstanceState(x => x.CurrentState);

            Initially(
                When(StartEvent)
                    .Finalize());

            SetCompletedWhenFinalized();
        }

        public Event<Start> StartEvent { get; init; }
    }
}


namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    [TestFixture]
    public class Using_a_message_type_without_a_namespace
    {
        [Test]
        public async Task Should_be_consumed()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x => x.AddConsumer<NoNamespaceMessageConsumer>())
                .BuildServiceProvider(true);

            var harness = await provider.StartTestHarness();

            await harness.Bus.Publish(new NoNamespaceMessage { Value = 27 });

            Assert.That(await harness.Consumed.Any<NoNamespaceMessage>(), Is.True);
        }

        [Test]
        public async Task Should_be_consumed_when_nested()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x => x.AddConsumer<NoNamespaceScope.NestedMessageConsumer>())
                .BuildServiceProvider(true);

            var harness = await provider.StartTestHarness();

            await harness.Bus.Publish(new NoNamespaceScope.NestedMessage { Value = 27 });

            Assert.That(await harness.Consumed.Any<NoNamespaceScope.NestedMessage>(), Is.True);
        }

        [Test]
        public async Task Should_be_consumed_by_a_saga_state_machine()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                    x.AddSagaStateMachine<NoNamespaceSagaScope.Machine, NoNamespaceSagaScope.Instance>())
                .BuildServiceProvider(true);

            var harness = await provider.StartTestHarness();

            await harness.Bus.Publish(new NoNamespaceSagaScope.Start { CorrelationId = NewId.NextGuid() });

            Assert.That(await harness.Consumed.Any<NoNamespaceSagaScope.Start>(), Is.True);
        }

        [Test]
        public void Should_be_a_valid_message_type()
        {
            Assert.Multiple(() =>
            {
                Assert.That(MessageTypeCache<NoNamespaceMessage>.IsValidMessageType, Is.True);
                Assert.That(MessageTypeCache<NoNamespaceScope.NestedMessage>.IsValidMessageType, Is.True);
            });
        }

        [Test]
        public void Should_omit_the_namespace_from_the_message_urn()
        {
            Assert.Multiple(() =>
            {
                Assert.That(MessageUrn.ForTypeString<NoNamespaceMessage>(), Is.EqualTo("urn:message:NoNamespaceMessage"));
                Assert.That(MessageUrn.ForTypeString<NoNamespaceScope.NestedMessage>(),
                    Is.EqualTo("urn:message:NoNamespaceScope+NestedMessage"));
            });
        }

        [Test]
        public void Should_still_reject_anonymous_types()
        {
            Assert.That(IsValidMessageType(new { Value = 27 }), Is.False);
        }

        static bool IsValidMessageType<T>(T message)
            where T : class
        {
            return MessageTypeCache<T>.IsValidMessageType;
        }
    }
}
