namespace MassTransit.Courier.Hosts
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    public class HostCompensateActivityContext<TActivity, TLog> :
        CompensateActivityContext<TActivity, TLog>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly TActivity _activity;
        readonly CompensateContext<TLog> _context;

        public HostCompensateActivityContext(TActivity activity, CompensateContext<TLog> context)
        {
            _context = context;
            _activity = activity;
        }

        CancellationToken PipeContext.CancellationToken => _context.CancellationToken;

        bool PipeContext.HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
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

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            return _context.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            return _context.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            return _context.Publish(message, messageType, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            return _context.Publish(message, messageType, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            return _context.Publish<T>(values, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish(values, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish<T>(values, publishPipe, cancellationToken);
        }

        Task<ISendEndpoint> ISendEndpointProvider.GetSendEndpoint(Uri address)
        {
            return _context.GetSendEndpoint(address);
        }

        Guid CompensateContext.TrackingNumber => _context.TrackingNumber;
        HostInfo CompensateContext.Host => _context.Host;
        DateTime CompensateContext.StartTimestamp => _context.Timestamp;
        TimeSpan CompensateContext.ElapsedTime => _context.Elapsed;
        DateTime CompensateContext.Timestamp => _context.Timestamp;
        TimeSpan CompensateContext.Elapsed => _context.Elapsed;
        ConsumeContext CompensateContext.ConsumeContext => _context.ConsumeContext;
        string CompensateContext.ActivityName => _context.ActivityName;
        Guid CompensateContext.ExecutionId => _context.ExecutionId;

        CompensationResult CompensateContext.Compensated()
        {
            return _context.Compensated();
        }

        CompensationResult CompensateContext.Compensated(object values)
        {
            return _context.Compensated(values);
        }

        CompensationResult CompensateContext.Compensated(IDictionary<string, object> variables)
        {
            return _context.Compensated(variables);
        }

        CompensationResult CompensateContext.Failed()
        {
            return _context.Failed();
        }

        CompensationResult CompensateContext.Failed(Exception exception)
        {
            return _context.Failed(exception);
        }

        TLog CompensateContext<TLog>.Log => _context.Log;
        TActivity CompensateActivityContext<TActivity, TLog>.Activity => _activity;

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _context.ConnectPublishObserver(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }
    }
}
