namespace MassTransit.EventHubIntegration
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Transports;


    public interface EventHubSendTransportContext :
        SendTransportContext,
        IProbeSite
    {
        IEnumerable<IAgent> GetAgentHandles();

        Task<EventHubSendContext<T>> CreateContext<T>(T value, IPipe<EventHubSendContext<T>> pipe, CancellationToken cancellationToken,
            IPipe<SendContext<T>> initializerPipe = null)
            where T : class;

        Task Send<T>(ProducerContext producerContext, EventHubSendContext<T> sendContext)
            where T : class;

        Task Send<T>(ProducerContext producerContext, EventHubSendContext<T>[] sendContexts)
            where T : class;

        Task Send(IPipe<ProducerContext> pipe, CancellationToken cancellationToken);
    }
}
