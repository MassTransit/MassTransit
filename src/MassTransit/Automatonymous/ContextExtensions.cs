namespace Automatonymous
{
    using System;
    using Contexts;
    using MassTransit;


    public static class ContextExtensions
    {
        public static ConsumeEventContext<TInstance, TData> CreateConsumeContext<TInstance, TData>(this BehaviorContext<TInstance, TData> context)
            where TData : class
        {
            if (!context.TryGetPayload(out ConsumeContext<TData> consumeContext))
                throw new ArgumentException("The ConsumeContext was not available");

            return new AutomatonymousConsumeEventContext<TInstance, TData>(context, consumeContext);
        }

        public static ConsumeEventContext<TInstance> CreateConsumeContext<TInstance>(this BehaviorContext<TInstance> context)
        {
            if (!context.TryGetPayload(out ConsumeContext consumeContext))
                throw new ArgumentException("The ConsumeContext was not available");

            return new AutomatonymousConsumeEventContext<TInstance>(context, consumeContext);
        }

        public static bool TryGetExceptionContext<TInstance, TException>(this BehaviorContext<TInstance> context,
            out ConsumeExceptionEventContext<TInstance, TException> exceptionContext)
            where TException : Exception
        {
            if (context is BehaviorExceptionContext<TInstance, TException> behaviorExceptionContext)
            {
                if (!context.TryGetPayload(out ConsumeContext consumeContext))
                    throw new ContextException("The consume context could not be retrieved.");

                exceptionContext = new AutomatonymousConsumeExceptionEventContext<TInstance, TException>(behaviorExceptionContext, consumeContext);
                return true;
            }

            exceptionContext = null;
            return false;
        }

        public static bool TryGetExceptionContext<TInstance, TData, TException>(this BehaviorContext<TInstance, TData> context,
            out ConsumeExceptionEventContext<TInstance, TData, TException> exceptionContext)
            where TData : class
            where TException : Exception
        {
            if (context is BehaviorExceptionContext<TInstance, TData, TException> behaviorExceptionContext)
            {
                if (!context.TryGetPayload(out ConsumeContext<TData> consumeContext))
                    throw new ContextException("The consume context could not be retrieved.");

                exceptionContext = new AutomatonymousConsumeExceptionEventContext<TInstance, TData, TException>(behaviorExceptionContext, consumeContext);
                return true;
            }

            exceptionContext = null;
            return false;
        }
    }
}
