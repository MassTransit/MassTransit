namespace Automatonymous
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;


    public static partial class BehaviorContextExtensions
    {
        public static Task Send<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, T message)
            where T : class
        {
            return GetConsumeContext(context).Send(message);
        }

        public static Task Send<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, T message, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).Send(message, sendPipe);
        }

        public static Task Send<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, T message, IPipe<SendContext> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).Send(message, sendPipe);
        }

        public static Task Send<TInstance, TData>(this BehaviorContext<TInstance, TData> context, object message)
        {
            return GetConsumeContext(context).Send(message);
        }

        public static Task Send<TInstance, TData>(this BehaviorContext<TInstance, TData> context, object message, Type messageType)
        {
            return GetConsumeContext(context).Send(message, messageType);
        }

        public static Task Send<TInstance, TData>(this BehaviorContext<TInstance, TData> context, object message, IPipe<SendContext> sendPipe)
        {
            return GetConsumeContext(context).Send(message, sendPipe);
        }

        public static Task Send<TInstance, TData>(this BehaviorContext<TInstance, TData> context, object message, Type messageType,
            IPipe<SendContext> sendPipe)
        {
            return GetConsumeContext(context).Send(message, messageType, sendPipe);
        }

        public static Task Send<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, object values)
            where T : class
        {
            return GetConsumeContext(context).Send<T>(values);
        }

        public static Task Send<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, object values, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).Publish(values, sendPipe);
        }

        public static Task Send<TInstance, TData, T>(this BehaviorContext<TInstance, TData> context, object values, IPipe<SendContext> sendPipe)
            where T : class
        {
            return GetConsumeContext(context).Send<T>(values, sendPipe);
        }
    }
}
