namespace MassTransit.Tests.SagaStateMachineTests;

using System;
using System.Threading.Tasks;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;


[TestFixture]
public class Scheduling_a_correlated_message
{
    [Test]
    public async Task Should_automatically_configure_correlation()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(3));
                x.AddSagaStateMachine<InstanceStateMachine, Instance>();
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var id = NewId.NextGuid();

        await harness.Bus.Publish(new StartInstance { CorrelationId = id });

        Assert.That(await harness.Consumed.Any<NotifyInstance>());
    }


    public class Instance :
        SagaStateMachineInstance
    {
        public string CurrentState { get; set; }
        public Guid? NotifyTokenId { get; set; }
        public Guid CorrelationId { get; set; }
    }


    public class InstanceStateMachine :
        MassTransitStateMachine<Instance>
    {
        public InstanceStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Schedule(() => Notify, x => x.NotifyTokenId, x => x.Delay = TimeSpan.FromSeconds(2));

            Initially(
                When(Start)
                    .Schedule(Notify, context => new NotifyInstance { CorrelationId = context.Message.CorrelationId })
                    .TransitionTo(Starting)
            );

            During(Starting,
                When(Notify.Received)
                    .Finalize());
        }

        public Event<StartInstance> Start { get; set; }
        public State Starting { get; set; }

        public Schedule<Instance, NotifyInstance> Notify { get; set; }
    }


    public record StartInstance
    {
        public Guid CorrelationId { get; set; }
    }


    public record NotifyInstance :
        CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
    }
}
