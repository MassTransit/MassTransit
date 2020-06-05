namespace Automatonymous
{
    using System;
    using Contexts;
    using GreenPipes;
    using MassTransit;


    public static class ContextExtensions
    {
        public static ConsumeEventContext<TInstance, TData> CreateConsumeContext<TInstance, TData>(this BehaviorContext<TInstance, TData> context)
            where TData : class
        {
            var consumeContext = context.GetPayload<ConsumeContext<TData>>();

            return new AutomatonymousConsumeEventContext<TInstance, TData>(context, consumeContext);
        }

        public static ConsumeEventContext<TInstance> CreateConsumeContext<TInstance>(this BehaviorContext<TInstance> context)
        {
            var consumeContext = context.GetPayload<ConsumeContext>();

            return new AutomatonymousConsumeEventContext<TInstance>(context, consumeContext);
        }

        public static bool TryGetExceptionContext<TInstance, TException>(this BehaviorContext<TInstance> context,
            out ConsumeExceptionEventContext<TInstance, TException> exceptionContext)
            where TException : Exception
        {
            if (context is BehaviorExceptionContext<TInstance, TException> behaviorExceptionContext)
            {
                var consumeContext = context.GetPayload<ConsumeContext>();

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
                var consumeContext = context.GetPayload<ConsumeContext<TData>>();

                exceptionContext = new AutomatonymousConsumeExceptionEventContext<TInstance, TData, TException>(behaviorExceptionContext, consumeContext);
                return true;
            }

            exceptionContext = null;
            return false;
        }
    }
}
