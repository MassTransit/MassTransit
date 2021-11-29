namespace MassTransit.Audit.Observers
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Util;
    using Util.Scanning;


    public class AuditSendObserver :
        ISendObserver
    {
        readonly CompositeFilter<SendContext> _filter;
        readonly ISendMetadataFactory _metadataFactory;
        readonly IMessageAuditStore _store;

        public AuditSendObserver(IMessageAuditStore store, ISendMetadataFactory metadataFactory, CompositeFilter<SendContext> filter)
        {
            _store = store;
            _metadataFactory = metadataFactory;
            _filter = filter;
        }

        Task ISendObserver.PreSend<T>(SendContext<T> context)
        {
            return Task.CompletedTask;
        }

        Task ISendObserver.PostSend<T>(SendContext<T> context)
        {
            if (!_filter.Matches(context))
                return Task.CompletedTask;

            var metadata = _metadataFactory.CreateAuditMetadata(context);

            return _store.StoreMessage(context.Message, metadata);
        }

        Task ISendObserver.SendFault<T>(SendContext<T> context, Exception exception)
        {
            return Task.CompletedTask;
        }
    }
}
