namespace MassTransit.Tests.Courier
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Courier.Contracts;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Sending_a_routing_slip :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_properly_serialized_as_a_message()
        {
            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddActivity("test", new Uri("loopback://localhost/execute_testactivity"), new { });

            await InputQueueSendEndpoint.Send(builder.Build());

            await _received;
        }

        Task<ConsumeContext<RoutingSlip>> _received;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<RoutingSlip>(configurator);
        }
    }
}
