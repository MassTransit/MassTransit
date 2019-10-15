namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using System;
    using GreenPipes;
    using MassTransit.Configuration;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Filters;


    public class BrokeredMessageReceiverServiceBusEndpointConfiguration :
        ReceiveEndpointConfiguration
    {
        public BrokeredMessageReceiverServiceBusEndpointConfiguration(IBusConfiguration busConfiguration)
            : base(busConfiguration)
        {
            HostAddress = busConfiguration.HostConfiguration.HostAddress;
            InputAddress = new Uri(busConfiguration.HostConfiguration.HostAddress, "no-queue-specified");
        }

        public override Uri HostAddress { get; }

        public override Uri InputAddress { get; }

        public override IReceivePipe CreateReceivePipe()
        {
            return Receive.CreatePipe(ConsumePipe, Serialization.Deserializer, configurator =>
            {
                Receive.ErrorConfigurator.UseFilter(new GenerateFaultFilter());

                configurator.UseRescue(Receive.ErrorConfigurator.Build(), x =>
                {
                    x.Ignore<OperationCanceledException>();
                });
            });
        }
    }
}
