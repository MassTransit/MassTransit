namespace MassTransit.Context
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Payloads;


    public class ConsumeContextScope :
        ConsumeContextProxy
    {
        readonly ConsumeContext _context;
        IPayloadCache _payloadCache;

        public ConsumeContextScope(ConsumeContext context)
            : base(context)
        {
            _context = context;
        }

        public ConsumeContextScope(ConsumeContext context, params object[] payloads)
            : base(context)
        {
            _context = context;

            _payloadCache = new ListPayloadCache(payloads);
        }

        public override CancellationToken CancellationToken => _context.CancellationToken;

        IPayloadCache PayloadCache
        {
            get
            {
                if (_payloadCache != null)
                    return _payloadCache;

                while (Volatile.Read(ref _payloadCache) == null)
                    Interlocked.CompareExchange(ref _payloadCache, new ListPayloadCache(), null);

                return _payloadCache;
            }
        }

        public override bool HasPayloadType(Type payloadType)
        {
            return payloadType.GetTypeInfo().IsInstanceOfType(this) || PayloadCache.HasPayloadType(payloadType) || _context.HasPayloadType(payloadType);
        }

        public override bool TryGetPayload<T>(out T payload)
        {
            if (this is T context)
            {
                payload = context;
                return true;
            }

            return PayloadCache.TryGetPayload(out payload) || _context.TryGetPayload(out payload);
        }

        public override T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)
        {
            if (this is T context)
                return context;

            if (PayloadCache.TryGetPayload<T>(out var payload))
                return payload;

            if (_context.TryGetPayload(out payload))
                return payload;

            return PayloadCache.GetOrAddPayload(payloadFactory);
        }

        public override T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
        {
            if (this is T context)
                return context;

            if (PayloadCache.TryGetPayload<T>(out var payload))
                return PayloadCache.AddOrUpdatePayload(addFactory, updateFactory);

            if (_context.TryGetPayload(out payload))
            {
                T Add()
                {
                    return updateFactory(payload);
                }

                return PayloadCache.AddOrUpdatePayload(Add, updateFactory);
            }

            return PayloadCache.AddOrUpdatePayload(addFactory, updateFactory);
        }
    }


    public class ConsumeContextScope<TMessage> :
        ConsumeContextScope,
        ConsumeContext<TMessage>
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;

        public ConsumeContextScope(ConsumeContext<TMessage> context)
            : base(context)
        {
            _context = context;
        }

        public ConsumeContextScope(ConsumeContext<TMessage> context, params object[] payloads)
            : base(context, payloads)
        {
            _context = context;
        }

        public TMessage Message => _context.Message;

        public virtual Task NotifyConsumed(TimeSpan duration, string consumerType)
        {
            return NotifyConsumed(this, duration, consumerType);
        }

        public virtual Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return NotifyFaulted(this, duration, consumerType, exception);
        }
    }
}
