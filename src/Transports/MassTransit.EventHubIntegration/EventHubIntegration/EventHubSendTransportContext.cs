namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Transports;


    public interface EventHubSendTransportContext :
        SendTransportContext
    {
        Uri HostAddress { get; }
        EventHubEndpointAddress EndpointAddress { get; }
        ISendPipe SendPipe { get; }

        Task Send(IPipe<ProducerContext> pipe, CancellationToken cancellationToken);
    }
}
