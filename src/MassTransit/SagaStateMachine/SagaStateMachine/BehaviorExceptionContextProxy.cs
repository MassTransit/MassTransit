namespace MassTransit
{
    using System;


    public partial class MassTransitStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        public class BehaviorExceptionContextProxy<TException> :
            BehaviorContextProxy,
            BehaviorExceptionContext<TInstance, TException>
            where TException : Exception
        {
            readonly BehaviorContext<TInstance> _context;

            public BehaviorExceptionContextProxy(BehaviorContext<TInstance> context, TException exception)
                : base(context.StateMachine, context, context.Event)
            {
                _context = context;
                Exception = exception;
            }

            public TException Exception { get; }

            public new BehaviorExceptionContext<TInstance, T, TException> CreateProxy<T>(Event<T> @event, T data)
                where T : class
            {
                return new BehaviorExceptionContextProxy<T, TException>(_context.CreateProxy(@event, data), Exception);
            }
        }


        public class BehaviorExceptionContextProxy<TData, TException> :
            BehaviorContextProxy<TData>,
            BehaviorExceptionContext<TInstance, TData, TException>
            where TData : class
            where TException : Exception
        {
            readonly BehaviorContext<TInstance, TData> _context;

            public BehaviorExceptionContextProxy(BehaviorContext<TInstance, TData> context, TException exception)
                : base(context.StateMachine, context, context, context.Event)
            {
                _context = context;
                Exception = exception;
            }

            public TException Exception { get; }

            public new BehaviorExceptionContext<TInstance, T, TException> CreateProxy<T>(Event<T> @event, T data)
                where T : class
            {
                return new BehaviorExceptionContextProxy<T, TException>(_context.CreateProxy(@event, data), Exception);
            }
        }
    }
}
