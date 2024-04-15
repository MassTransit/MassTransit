namespace MassTransit.Tests.Courier
{
    using System.Threading.Tasks;
    using MassTransit.Courier.Contracts;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Courier;


    [TestFixture]
    public class FaultyRetriedActivityVariables_Specs :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_publish_the_completed_event_and_redeliver()
        {
            Task<ConsumeContext<RoutingSlipActivityFaulted>> completed = SubscribeHandler<RoutingSlipActivityFaulted>();

            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);
            await builder.AddSubscription(InputQueueAddress, RoutingSlipEvents.ActivityFaulted, x => x.Send(new MessageWithVariables()));

            var testActivity = GetActivityContext<SetVariablesFaultyActivity>();
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri);

            await Bus.Execute(builder.Build());

            await completed;

            var message = (await _received.Task).Message;
            Assert.That(message.Variables.Test, Is.EqualTo("Data"));
        }


        class MessageVariables
        {
            public string Test { get; set; }
        }


        class MessageWithVariables
        {
            public MessageVariables Variables { get; set; }
        }


        TaskCompletionSource<ConsumeContext<MessageWithVariables>> _received;

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<SetVariablesFaultyActivity, SetVariablesFaultyArguments>(() => new SetVariablesFaultyActivity());
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseMessageRetry(r => r.Intervals(1000));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = GetTask<ConsumeContext<MessageWithVariables>>();
            configurator.Handler<MessageWithVariables>(async context =>
            {
                _received.TrySetResult(context);
            });
        }
    }
}
