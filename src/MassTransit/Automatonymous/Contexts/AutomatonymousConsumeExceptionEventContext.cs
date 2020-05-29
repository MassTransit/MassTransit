namespace Automatonymous.Contexts
{
    using System;
    using MassTransit;


    public class AutomatonymousConsumeExceptionEventContext<TInstance, TException> :
        AutomatonymousConsumeEventContext<TInstance>,
        ConsumeExceptionEventContext<TInstance, TException>
        where TException : Exception
    {
        public AutomatonymousConsumeExceptionEventContext(BehaviorExceptionContext<TInstance, TException> context, ConsumeContext consumeContext)
            : base(context, consumeContext)
        {
            Exception = context.Exception;
        }

        public TException Exception { get; }
    }


    public class AutomatonymousConsumeExceptionEventContext<TInstance, TData, TException> :
        AutomatonymousConsumeEventContext<TInstance, TData>,
        ConsumeExceptionEventContext<TInstance, TData, TException>
        where TData : class
        where TException : Exception
    {
        public AutomatonymousConsumeExceptionEventContext(BehaviorExceptionContext<TInstance, TData, TException> context, ConsumeContext<TData> consumeContext)
            : base(context, consumeContext)
        {
            Exception = context.Exception;
        }

        public TException Exception { get; }
    }
}
