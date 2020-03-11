namespace MassTransit.Saga
{
    using System;
    using System.Runtime.CompilerServices;
    using Context;
    using Metadata;
    using Microsoft.Extensions.Logging;


    public static class SagaLogExtensions
    {
        static readonly LogMessage<string, Guid?, string> _logUsed = LogContext.Define<string, Guid?, string>(LogLevel.Debug,
            "SAGA:{SagaType}:{CorrelationId} Used {MessageType}");

        static readonly LogMessage<string, Guid?, string> _logAdded = LogContext.Define<string, Guid?, string>(LogLevel.Debug,
            "SAGA:{SagaType}:{CorrelationId} Added {MessageType}");

        static readonly LogMessage<string, Guid?, string> _logCreated = LogContext.Define<string, Guid?, string>(LogLevel.Debug,
            "SAGA:{SagaType}:{CorrelationId} Created {MessageType}");

        static readonly LogMessage<string, Guid?, string> _logInserted = LogContext.Define<string, Guid?, string>(LogLevel.Debug,
            "SAGA:{SagaType}:{CorrelationId} Used {MessageType}");

        static readonly LogMessage<string, Guid?, string> _logInsertFaulted = LogContext.Define<string, Guid?, string>(LogLevel.Debug,
            "SAGA:{SagaType}:{CorrelationId} Dupe {MessageType}");

        static readonly LogMessage<string, Guid?, string> _logRemoved = LogContext.Define<string, Guid?, string>(LogLevel.Debug,
            "SAGA:{SagaType}:{CorrelationId} Removed {MessageType}");

        static readonly LogMessage<string, Guid?, string> _logFaulted = LogContext.Define<string, Guid?, string>(LogLevel.Error,
            "SAGA:{SagaType}:{CorrelationId} Fault {MessageType}");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogUsed<TSaga, TMessage>(this SagaConsumeContext<TSaga, TMessage> context, Guid? correlationId = default)
            where TSaga : class, ISaga
            where TMessage : class
        {
            _logUsed(TypeMetadataCache<TSaga>.ShortName, context.CorrelationId ?? correlationId, TypeMetadataCache<TMessage>.ShortName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogAdded<TSaga, TMessage>(this SagaConsumeContext<TSaga, TMessage> context, Guid? correlationId = default)
            where TSaga : class, ISaga
            where TMessage : class
        {
            _logAdded(TypeMetadataCache<TSaga>.ShortName, context.CorrelationId ?? correlationId, TypeMetadataCache<TMessage>.ShortName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogCreated<TSaga, TMessage>(this SagaConsumeContext<TSaga, TMessage> context, Guid? correlationId = default)
            where TSaga : class, ISaga
            where TMessage : class
        {
            _logCreated(TypeMetadataCache<TSaga>.ShortName, context.CorrelationId ?? correlationId, TypeMetadataCache<TMessage>.ShortName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogInsert<TSaga, TMessage>(this ConsumeContext<TMessage> context, Guid? correlationId = default)
            where TSaga : class, ISaga
            where TMessage : class
        {
            _logInserted(TypeMetadataCache<TSaga>.ShortName, context.CorrelationId ?? correlationId, TypeMetadataCache<TMessage>.ShortName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogInsertFault<TSaga, TMessage>(this ConsumeContext<TMessage> context, Exception exception,
            Guid? correlationId = default)
            where TSaga : class, ISaga
            where TMessage : class
        {
            _logInsertFaulted(TypeMetadataCache<TSaga>.ShortName, context.CorrelationId ?? correlationId, TypeMetadataCache<TMessage>.ShortName, exception);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogRemoved<TSaga, TMessage>(this SagaConsumeContext<TSaga, TMessage> context, Guid? correlationId = default)
            where TSaga : class, ISaga
            where TMessage : class
        {
            _logRemoved(TypeMetadataCache<TSaga>.ShortName, context.CorrelationId ?? correlationId, TypeMetadataCache<TMessage>.ShortName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogFault<TSaga, TMessage>(this ConsumeContext<TMessage> context, Exception exception, Guid? correlationId = default)
            where TSaga : class, ISaga
            where TMessage : class
        {
            _logFaulted(TypeMetadataCache<TSaga>.ShortName, context.CorrelationId ?? correlationId, TypeMetadataCache<TMessage>.ShortName, exception);
        }
    }
}
