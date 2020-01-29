namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using System;
    using GreenPipes;
    using MassTransit.Configuration;
    using MassTransit.Pipeline;


    public class BrokeredMessageReceiverServiceBusEndpointConfiguration :
        ReceiveEndpointConfiguration
    {
        public BrokeredMessageReceiverServiceBusEndpointConfiguration(IBusConfiguration busConfiguration)
            : base(busConfiguration)
        {
            HostAddress = busConfiguration.HostConfiguration.HostAddress;
            InputAddress = new Uri(busConfiguration.HostConfiguration.HostAddress, "no-queue-specified");

            var deadLetterPipe = Pipe.Execute<ReceiveContext>(context => throw new TransportException(context.InputAddress, "The message was not consumed"));

            Receive.DeadLetterConfigurator.UseDeadLetter(deadLetterPipe);
        }

        public override Uri HostAddress { get; }

        public override Uri InputAddress { get; }

        public override IReceivePipe CreateReceivePipe()
        {
            return Receive.CreatePipe(ConsumePipe, Serialization.Deserializer);
        }
    }
}
