namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Initializers;


    public partial class MassTransitStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        public class BehaviorContextProxy :
            ConsumeContextProxy,
            BehaviorContext<TInstance>
        {
            readonly SagaConsumeContext<TInstance> _context;
            readonly Event _event;

            public BehaviorContextProxy(StateMachine<TInstance> machine, SagaConsumeContext<TInstance> context, Event @event)
                : base(context)
            {
                StateMachine = machine;
                _context = context;
                _event = @event;
            }

            public StateMachine<TInstance> StateMachine { get; }

            public override Guid? CorrelationId => Saga.CorrelationId;

            public TInstance Saga => _context.Saga;

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

            Task<SendTuple<T>> BehaviorContext<TInstance>.Init<T>(object values)
            {
                return MessageInitializerCache<T>.InitializeMessage(this, values);
            }

            Event BehaviorContext<TInstance>.Event => _event;

            public TInstance Instance => _context.Saga;

            public BehaviorContext<TInstance> CreateProxy(Event @event)
            {
                return new BehaviorContextProxy(StateMachine, _context, @event);
            }

            public BehaviorContext<TInstance, T> CreateProxy<T>(Event<T> @event, T data)
                where T : class
            {
                return new BehaviorContextProxy<T>(StateMachine, _context, new MessageConsumeContext<T>(_context, data), @event);
            }
        }


        public class BehaviorContextProxy<TMessage> :
            ConsumeContextProxy<TMessage>,
            BehaviorContext<TInstance, TMessage>
            where TMessage : class
        {
            readonly SagaConsumeContext<TInstance> _context;
            readonly Event<TMessage> _event;

            public BehaviorContextProxy(StateMachine<TInstance> machine, SagaConsumeContext<TInstance> context, ConsumeContext<TMessage> consumeContext,
                Event<TMessage> @event)
                : base(consumeContext)
            {
                StateMachine = machine;
                _context = context;
                _event = @event;
            }

            public StateMachine<TInstance> StateMachine { get; }

            public override Guid? CorrelationId => Saga.CorrelationId;

            public TInstance Saga => _context.Saga;

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

            Task<SendTuple<T>> BehaviorContext<TInstance, TMessage>.Init<T>(object values)
            {
                return MessageInitializerCache<T>.InitializeMessage(this, values);
            }

            Task<SendTuple<T>> BehaviorContext<TInstance>.Init<T>(object values)
            {
                return MessageInitializerCache<T>.InitializeMessage(this, values);
            }

            public TMessage Data => Message;
            Event BehaviorContext<TInstance>.Event => _event;
            Event<TMessage> BehaviorContext<TInstance, TMessage>.Event => _event;

            public TInstance Instance => _context.Saga;

            public BehaviorContext<TInstance> CreateProxy(Event @event)
            {
                return new BehaviorContextProxy(StateMachine, _context, @event);
            }

            public BehaviorContext<TInstance, T> CreateProxy<T>(Event<T> @event, T data)
                where T : class
            {
                return new BehaviorContextProxy<T>(StateMachine, _context, new MessageConsumeContext<T>(_context, data), @event);
            }
        }
    }
}
