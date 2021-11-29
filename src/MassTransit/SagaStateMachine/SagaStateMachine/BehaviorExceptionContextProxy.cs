namespace MassTransit.SagaStateMachine
{
    using System;


    public class BehaviorExceptionContextProxy<TSaga, TException> :
        BehaviorContextProxy<TSaga>,
        BehaviorExceptionContext<TSaga, TException>
        where TSaga : class, ISaga
        where TException : Exception
    {
        readonly BehaviorContext<TSaga> _context;

        public BehaviorExceptionContextProxy(BehaviorContext<TSaga> context, TException exception)
            : base(context.StateMachine, context, context.Event)
        {
            _context = context;
            Exception = exception;
        }

        public TException Exception { get; }

        public new BehaviorExceptionContext<TSaga, T, TException> CreateProxy<T>(Event<T> @event, T data)
            where T : class
        {
            return new BehaviorExceptionContextProxy<TSaga, T, TException>(_context.CreateProxy(@event, data), Exception);
        }
    }


    public class BehaviorExceptionContextProxy<TSaga, TData, TException> :
        BehaviorContextProxy<TSaga, TData>,
        BehaviorExceptionContext<TSaga, TData, TException>
        where TSaga : class, ISaga
        where TData : class
        where TException : Exception
    {
        readonly BehaviorContext<TSaga, TData> _context;

        public BehaviorExceptionContextProxy(BehaviorContext<TSaga, TData> context, TException exception)
            : base(context.StateMachine, context, context, context.Event)
        {
            _context = context;
            Exception = exception;
        }

        public TException Exception { get; }

        public new BehaviorExceptionContext<TSaga, T, TException> CreateProxy<T>(Event<T> @event, T data)
            where T : class
        {
            return new BehaviorExceptionContextProxy<TSaga, T, TException>(_context.CreateProxy(@event, data), Exception);
        }
    }
}
