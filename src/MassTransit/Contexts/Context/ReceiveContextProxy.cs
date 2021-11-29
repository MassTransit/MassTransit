namespace MassTransit.Context
{
    using System;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;


    public abstract class ReceiveContextProxy :
        ReceiveContext
    {
        readonly ReceiveContext _context;

        protected ReceiveContextProxy(ReceiveContext context)
        {
            _context = context;
        }

        public CancellationToken CancellationToken => _context.CancellationToken;

        public virtual bool HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        public virtual bool TryGetPayload<TPayload>(out TPayload payload)
            where TPayload : class
        {
            return _context.TryGetPayload(out payload);
        }

        public virtual TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            where TPayload : class
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        T PipeContext.AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
        {
            return _context.AddOrUpdatePayload(addFactory, updateFactory);
        }

        public bool PublishFaults => _context.PublishFaults;
        public MessageBody Body => _context.Body;

        public TimeSpan ElapsedTime => _context.ElapsedTime;
        public Uri InputAddress => _context.InputAddress;
        public ContentType ContentType => _context.ContentType;
        public bool Redelivered => _context.Redelivered;
        public Headers TransportHeaders => _context.TransportHeaders;
        public Task ReceiveCompleted => _context.ReceiveCompleted;
        public bool IsDelivered => _context.IsDelivered;
        public bool IsFaulted => _context.IsFaulted;

        public virtual Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            return _context.NotifyConsumed(context, duration, consumerType);
        }

        public virtual Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            return _context.NotifyFaulted(context, duration, consumerType, exception);
        }

        public Task NotifyFaulted(Exception exception)
        {
            return _context.NotifyFaulted(exception);
        }

        public virtual void AddReceiveTask(Task task)
        {
            _context.AddReceiveTask(task);
        }

        public virtual ISendEndpointProvider SendEndpointProvider => _context.SendEndpointProvider;
        public virtual IPublishEndpointProvider PublishEndpointProvider => _context.PublishEndpointProvider;
    }
}
