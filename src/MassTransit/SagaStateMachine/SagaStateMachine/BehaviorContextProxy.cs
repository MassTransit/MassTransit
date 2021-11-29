namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Initializers;


    public class BehaviorContextProxy<TSaga> :
        ConsumeContextProxy,
        BehaviorContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly SagaConsumeContext<TSaga> _context;
        readonly Event _event;

        public BehaviorContextProxy(StateMachine<TSaga> machine, SagaConsumeContext<TSaga> context, Event @event)
            : base(context)
        {
            StateMachine = machine;
            _context = context;
            _event = @event;
        }

        public StateMachine<TSaga> StateMachine { get; }

        public override Guid? CorrelationId => Saga.CorrelationId;

        public TSaga Saga => _context.Saga;

        public Task SetCompleted()
        {
            return _context.SetCompleted();
        }

        public bool IsCompleted => _context.IsCompleted;

        public Task Raise(Event @event)
        {
            return StateMachine.RaiseEvent(CreateProxy(@event));
        }

        public Task Raise<T>(Event<T> @event, T data)
            where T : class
        {
            return StateMachine.RaiseEvent(CreateProxy(@event, data));
        }

        Task<SendTuple<T>> BehaviorContext<TSaga>.Init<T>(object values)
        {
            return MessageInitializerCache<T>.InitializeMessage(this, values);
        }

        Event BehaviorContext<TSaga>.Event => _event;

        public TSaga Instance => _context.Saga;

        public BehaviorContext<TSaga> CreateProxy(Event @event)
        {
            return new BehaviorContextProxy<TSaga>(StateMachine, _context, @event);
        }

        public BehaviorContext<TSaga, T> CreateProxy<T>(Event<T> @event, T data)
            where T : class
        {
            return new BehaviorContextProxy<TSaga, T>(StateMachine, _context, new MessageConsumeContext<T>(_context, data), @event);
        }
    }


    public class BehaviorContextProxy<TSaga, TMessage> :
        ConsumeContextProxy<TMessage>,
        BehaviorContext<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly SagaConsumeContext<TSaga> _context;
        readonly Event<TMessage> _event;

        public BehaviorContextProxy(StateMachine<TSaga> machine, SagaConsumeContext<TSaga> context, ConsumeContext<TMessage> consumeContext,
            Event<TMessage> @event)
            : base(consumeContext)
        {
            StateMachine = machine;
            _context = context;
            _event = @event;
        }

        public StateMachine<TSaga> StateMachine { get; }

        public override Guid? CorrelationId => Saga.CorrelationId;

        public TSaga Saga => _context.Saga;

        public Task SetCompleted()
        {
            return _context.SetCompleted();
        }

        public bool IsCompleted => _context.IsCompleted;

        public Task Raise(Event @event)
        {
            return StateMachine.RaiseEvent(CreateProxy(@event));
        }

        public Task Raise<T>(Event<T> @event, T data)
            where T : class
        {
            return StateMachine.RaiseEvent(CreateProxy(@event, data));
        }

        Task<SendTuple<T>> BehaviorContext<TSaga, TMessage>.Init<T>(object values)
        {
            return MessageInitializerCache<T>.InitializeMessage(this, values);
        }

        Task<SendTuple<T>> BehaviorContext<TSaga>.Init<T>(object values)
        {
            return MessageInitializerCache<T>.InitializeMessage(this, values);
        }

        public TMessage Data => Message;
        Event BehaviorContext<TSaga>.Event => _event;
        Event<TMessage> BehaviorContext<TSaga, TMessage>.Event => _event;

        public TSaga Instance => _context.Saga;

        public BehaviorContext<TSaga> CreateProxy(Event @event)
        {
            return new BehaviorContextProxy<TSaga>(StateMachine, _context, @event);
        }

        public BehaviorContext<TSaga, T> CreateProxy<T>(Event<T> @event, T data)
            where T : class
        {
            return new BehaviorContextProxy<TSaga, T>(StateMachine, _context, new MessageConsumeContext<T>(_context, data), @event);
        }
    }
}
