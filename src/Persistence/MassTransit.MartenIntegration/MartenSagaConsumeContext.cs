namespace MassTransit.MartenIntegration
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Logging;
    using Marten;
    using Saga;
    using Util;


    public class MartenSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextProxyScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga
    {
        static readonly ILog Log = Logger.Get<MartenSagaRepository<TSaga>>();
        readonly IDocumentSession _session;

        public MartenSagaConsumeContext(IDocumentSession session, 
            ConsumeContext<TMessage> context, TSaga instance)
            : base(context)
        {
            _session = session;
            Saga = instance;
        }

        Guid? MessageContext.CorrelationId => Saga.CorrelationId;

        SagaConsumeContext<TSaga, T> SagaConsumeContext<TSaga>.PopContext<T>()
        {
            var context = this as SagaConsumeContext<TSaga, T>;
            if (context == null)
                throw new ContextException($"The ConsumeContext<{TypeMetadataCache<TMessage>.ShortName}> could not be cast to {TypeMetadataCache<T>.ShortName}");

            return context;
        }

        Task SagaConsumeContext<TSaga>.SetCompleted()
        {
            _session.Delete(Saga);
            _session.SaveChanges();
            IsCompleted = true;
            if (Log.IsDebugEnabled)
                Log.DebugFormat("SAGA:{0}:{1} Removed {2}", TypeMetadataCache<TSaga>.ShortName, TypeMetadataCache<TMessage>.ShortName,
                    Saga.CorrelationId);

            return TaskUtil.Completed;
        }

        public TSaga Saga { get; }
        public bool IsCompleted { get; private set; }
    }
}