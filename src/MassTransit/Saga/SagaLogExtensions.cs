namespace MassTransit.Saga
{
    using System;
    using Context;
    using Metadata;


    public static class SagaLogExtensions
    {
        public static void LogUsed<TSaga, TMessage>(this SagaConsumeContext<TSaga, TMessage> context, Guid? correlationId = default)
            where TSaga : class, ISaga
            where TMessage : class
        {
            LogContext.Debug?.Log("SAGA:{SagaType}:{CorrelationId} Used {MessageType}", TypeMetadataCache<TSaga>.ShortName,
                context.CorrelationId ?? correlationId, TypeMetadataCache<TMessage>.ShortName);
        }

        public static void LogAdded<TSaga, TMessage>(this SagaConsumeContext<TSaga, TMessage> context, Guid? correlationId = default)
            where TSaga : class, ISaga
            where TMessage : class
        {
            LogContext.Debug?.Log("SAGA:{SagaType}:{CorrelationId} Added {MessageType}", TypeMetadataCache<TSaga>.ShortName,
                context.CorrelationId ?? correlationId, TypeMetadataCache<TMessage>.ShortName);
        }

        public static void LogCreated<TSaga, TMessage>(this SagaConsumeContext<TSaga, TMessage> context, Guid? correlationId = default)
            where TSaga : class, ISaga
            where TMessage : class
        {
            LogContext.Debug?.Log("SAGA:{SagaType}:{CorrelationId} Created {MessageType}", TypeMetadataCache<TSaga>.ShortName,
                context.CorrelationId ?? correlationId, TypeMetadataCache<TMessage>.ShortName);
        }

        public static void LogInsert<TSaga, TMessage>(this ConsumeContext<TMessage> context, ISagaRepository<TSaga> repository, Guid? correlationId = default)
            where TSaga : class, ISaga
            where TMessage : class
        {
            LogContext.Debug?.Log("SAGA:{SagaType}:{CorrelationId} Added {MessageType}", TypeMetadataCache<TSaga>.ShortName,
                context.CorrelationId ?? correlationId, TypeMetadataCache<TMessage>.ShortName);
        }

        public static void LogInsertFault<TSaga, TMessage>(this ConsumeContext<TMessage> context, ISagaRepository<TSaga> repository, Exception exception,
            Guid? correlationId = default)
            where TSaga : class, ISaga
            where TMessage : class
        {
            LogContext.Debug?.Log(exception, "SAGA:{SagaType}:{CorrelationId} Dupe {MessageType}", TypeMetadataCache<TSaga>.ShortName, correlationId,
                TypeMetadataCache<TMessage>.ShortName);
        }

        public static void LogRemoved<TSaga, TMessage>(this SagaConsumeContext<TSaga, TMessage> context, Guid? correlationId = default)
            where TSaga : class, ISaga
            where TMessage : class
        {
            LogContext.Debug?.Log("SAGA:{SagaType}:{CorrelationId} Removed {MessageType}", TypeMetadataCache<TSaga>.ShortName,
                context.CorrelationId ?? correlationId, TypeMetadataCache<TMessage>.ShortName);
        }

        public static void LogRemoved<TSaga, TMessage>(this SagaConsumeContext<TSaga, TMessage> context, Exception exception, Guid? correlationId = default)
            where TSaga : class, ISaga
            where TMessage : class
        {
            LogContext.Error?.Log(exception, "SAGA:{SagaType}:{CorrelationId} Removed(Fault) {MessageType}", TypeMetadataCache<TSaga>.ShortName,
                context.CorrelationId ?? correlationId, TypeMetadataCache<TMessage>.ShortName);
        }

        public static void LogFault<TSaga, TMessage>(this ConsumeContext<TMessage> context, ISagaRepository<TSaga> repository, Exception exception,
            Guid? correlationId = default)
            where TSaga : class, ISaga
            where TMessage : class
        {
            LogContext.Error?.Log(exception, "SAGA:{SagaType}:{CorrelationId} Fault {MessageType}", TypeMetadataCache<TSaga>.ShortName, correlationId,
                TypeMetadataCache<TMessage>.ShortName);
        }

        public static void LogFault<TSaga, TMessage>(this SagaQueryConsumeContext<TSaga, TMessage> context, Exception exception, Guid? correlationId = default)
            where TSaga : class, ISaga
            where TMessage : class
        {
            LogContext.Error?.Log(exception, "SAGA:{SagaType}:{CorrelationId} Fault {MessageType}", TypeMetadataCache<TSaga>.ShortName, correlationId,
                TypeMetadataCache<TMessage>.ShortName);
        }

        public static void LogFault<TSaga, TMessage>(this SagaConsumeContext<TSaga, TMessage> context, Exception exception, Guid? correlationId = default)
            where TSaga : class, ISaga
            where TMessage : class
        {
            LogContext.Error?.Log(exception, "SAGA:{SagaType}:{CorrelationId} Fault {MessageType}", TypeMetadataCache<TSaga>.ShortName, correlationId,
                TypeMetadataCache<TMessage>.ShortName);
        }
    }
}
