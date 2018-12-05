namespace MassTransit.WebJobs.EventHubsIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.Azure.EventHubs;


    public class SharedEventDataSendEndpointContext :
        EventDataSendEndpointContext
    {
        readonly EventDataSendEndpointContext _context;

        public SharedEventDataSendEndpointContext(EventDataSendEndpointContext context, CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
            _context = context;
        }

        bool PipeContext.HasPayloadType(Type payloadType)
        {
            return _context.HasPayloadType(payloadType);
        }

        bool PipeContext.TryGetPayload<TPayload>(out TPayload payload)
        {
            return _context.TryGetPayload(out payload);
        }

        TPayload PipeContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        T PipeContext.AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
        {
            return _context.AddOrUpdatePayload(addFactory, updateFactory);
        }

        public CancellationToken CancellationToken { get; }

        string EventDataSendEndpointContext.EntityPath => _context.EntityPath;

        Task EventDataSendEndpointContext.Send(EventData message)
        {
            return _context.Send(message);
        }
    }
}