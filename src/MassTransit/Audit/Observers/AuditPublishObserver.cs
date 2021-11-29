namespace MassTransit.Audit.Observers
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Util;
    using Util.Scanning;


    public class AuditPublishObserver :
        IPublishObserver
    {
        readonly CompositeFilter<SendContext> _filter;
        readonly ISendMetadataFactory _metadataFactory;
        readonly IMessageAuditStore _store;

        public AuditPublishObserver(IMessageAuditStore store, ISendMetadataFactory metadataFactory, CompositeFilter<SendContext> filter)
        {
            _store = store;
            _metadataFactory = metadataFactory;
            _filter = filter;
        }

        Task IPublishObserver.PrePublish<T>(PublishContext<T> context)
        {
            return Task.CompletedTask;
        }

        Task IPublishObserver.PostPublish<T>(PublishContext<T> context)
        {
            if (!_filter.Matches(context))
                return Task.CompletedTask;

            var metadata = _metadataFactory.CreateAuditMetadata(context);

            return _store.StoreMessage(context.Message, metadata);
        }

        Task IPublishObserver.PublishFault<T>(PublishContext<T> context, Exception exception)
        {
            return Task.CompletedTask;
        }
    }
}
