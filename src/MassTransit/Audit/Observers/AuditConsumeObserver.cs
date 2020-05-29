namespace MassTransit.Audit.Observers
{
    using System;
    using System.Threading.Tasks;
    using Util;
    using Util.Scanning;


    public class AuditConsumeObserver :
        IConsumeObserver
    {
        readonly CompositeFilter<ConsumeContext> _filter;
        readonly IConsumeMetadataFactory _metadataFactory;

        readonly IMessageAuditStore _store;

        public AuditConsumeObserver(IMessageAuditStore store, IConsumeMetadataFactory metadataFactory, CompositeFilter<ConsumeContext> filter)
        {
            _store = store;
            _metadataFactory = metadataFactory;
            _filter = filter;
        }

        Task IConsumeObserver.PreConsume<T>(ConsumeContext<T> context)
        {
            if (!_filter.Matches(context))
                return TaskUtil.Completed;

            var metadata = _metadataFactory.CreateAuditMetadata(context);

            return _store.StoreMessage(context.Message, metadata);
        }

        Task IConsumeObserver.PostConsume<T>(ConsumeContext<T> context)
        {
            return TaskUtil.Completed;
        }

        Task IConsumeObserver.ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
        {
            return TaskUtil.Completed;
        }
    }
}
