namespace MassTransit.Context
{
    using System;
    using System.Reflection;
    using System.Threading;
    using GreenPipes;
    using GreenPipes.Payloads;


    public class ConsumeContextProxyScope :
        ConsumeContextProxy
    {
        readonly PipeContext _context;
        IPayloadCache _payloadCache;

        public ConsumeContextProxyScope(ConsumeContext context)
            : base(context)
        {
            _context = context;
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
        ConsumeContextProxy<TMessage>
        where TMessage : class
    {
        public ConsumeContextScope(ConsumeContext<TMessage> context)
            : base(context)
        {
        }
    }
}
