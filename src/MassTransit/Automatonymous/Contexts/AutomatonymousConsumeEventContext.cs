namespace Automatonymous.Contexts
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Context;


    public class AutomatonymousConsumeEventContext<TInstance> :
        ConsumeContextProxy,
        ConsumeEventContext<TInstance>
    {
        readonly BehaviorContext<TInstance> _context;

        public AutomatonymousConsumeEventContext(BehaviorContext<TInstance> context, ConsumeContext consumeContext)
            : base(consumeContext)
        {
            _context = context;
        }

        public override bool HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType) || base.HasPayloadType(contextType);
        }

        public override bool TryGetPayload<TPayload>(out TPayload context)
        {
            if (_context.TryGetPayload(out context))
                return true;

            return base.TryGetPayload(out context);
        }

        public override TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
        {
            if (_context.TryGetPayload(out TPayload payload))
                return payload;

            if (base.TryGetPayload(out payload))
                return payload;

            return _context.GetOrAddPayload(payloadFactory);
        }

        public override T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
        {
            if (this is T context)
                return context;

            if (_context.TryGetPayload<T>(out var payload))
                return _context.AddOrUpdatePayload(addFactory, updateFactory);

            if (base.TryGetPayload(out payload))
            {
                T Add()
                {
                    return updateFactory(payload);
                }

                return _context.AddOrUpdatePayload(Add, updateFactory);
            }

            return _context.AddOrUpdatePayload(addFactory, updateFactory);
        }

        TInstance InstanceContext<TInstance>.Instance => _context.Instance;

        public Task Raise(Event @event)
        {
            return _context.Raise(@event);
        }

        public Task Raise<TData>(Event<TData> @event, TData data)
        {
            return _context.Raise(@event, data);
        }

        Event EventContext<TInstance>.Event => _context.Event;
    }


    public class AutomatonymousConsumeEventContext<TInstance, TData> :
        AutomatonymousConsumeEventContext<TInstance>,
        ConsumeEventContext<TInstance, TData>
        where TData : class
    {
        readonly BehaviorContext<TInstance, TData> _context;

        public AutomatonymousConsumeEventContext(BehaviorContext<TInstance, TData> context, ConsumeContext<TData> consumeContext)
            : base(context, consumeContext)
        {
            _context = context;
        }

        Event<TData> EventContext<TInstance, TData>.Event => _context.Event;
        TData EventContext<TInstance, TData>.Data => _context.Data;
    }
}
