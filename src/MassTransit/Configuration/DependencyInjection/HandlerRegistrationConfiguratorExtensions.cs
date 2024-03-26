namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;


    public static class HandlerRegistrationConfiguratorExtensions
    {
        /// <summary>
        /// Adds an empty message handler, which consumes the messages and does nothing else. Useful with the test harness to ensure
        /// that produced messages are consumed, which can then be asserted in unit tests.
        /// </summary>
        /// <param name="configurator"></param>
        public static IConsumerRegistrationConfigurator AddHandler<T>(this IRegistrationConfigurator configurator)
            where T : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            configurator.TryAddSingleton(new MessageHandlerMethod<T>((ConsumeContext<T> context) => Task.CompletedTask));

            return configurator.AddConsumer<MessageHandlerConsumer<T>, MessageHandlerConsumerDefinition<MessageHandlerConsumer<T>, T>>();
        }

        /// <summary>
        /// Adds a method handler, using the first parameter to determine the message type
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="handler">An asynchronous method to handle the message</param>
        public static IConsumerRegistrationConfigurator AddHandler<T>(this IRegistrationConfigurator configurator, Func<ConsumeContext<T>, Task> handler)
            where T : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            configurator.TryAddSingleton(new MessageHandlerMethod<T>(handler));

            return configurator.AddConsumer<MessageHandlerConsumer<T>, MessageHandlerConsumerDefinition<MessageHandlerConsumer<T>, T>>();
        }

        /// <summary>
        /// Adds a method handler, using the first parameter to determine the message type
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="handler">An asynchronous method to handle the message</param>
        public static IConsumerRegistrationConfigurator AddHandler<T>(this IRegistrationConfigurator configurator, Func<T, Task> handler)
            where T : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            configurator.TryAddSingleton(new MessageHandlerMethod<T>(handler));

            return configurator.AddConsumer<MessageHandlerConsumer<T>, MessageHandlerConsumerDefinition<MessageHandlerConsumer<T>, T>>();
        }

        /// <summary>
        /// Adds a method handler, using the first parameter to determine the message type
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="handler">An asynchronous method to handle the message</param>
        public static IConsumerRegistrationConfigurator AddHandler<T, TResponse>(this IRegistrationConfigurator configurator,
            Func<ConsumeContext<T>, Task<TResponse>> handler)
            where T : class
            where TResponse : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            configurator.TryAddSingleton(new RequestHandlerMethod<T, TResponse>(handler));

            return configurator.AddConsumer<RequestHandlerConsumer<T, TResponse>, MessageHandlerConsumerDefinition<RequestHandlerConsumer<T, TResponse>, T>>();
        }

        /// <summary>
        /// Adds a method handler, using the first parameter to determine the message type
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="handler">An asynchronous method to handle the message</param>
        public static IConsumerRegistrationConfigurator AddHandler<T, TResponse>(this IRegistrationConfigurator configurator, Func<T, Task<TResponse>> handler)
            where T : class
            where TResponse : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            configurator.TryAddSingleton(new RequestHandlerMethod<T, TResponse>(handler));

            return configurator.AddConsumer<RequestHandlerConsumer<T, TResponse>, MessageHandlerConsumerDefinition<RequestHandlerConsumer<T, TResponse>, T>>();
        }

        /// <summary>
        /// Adds a method handler, using the first parameter to determine the message type
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="handler">An asynchronous method to handle the message</param>
        public static IConsumerRegistrationConfigurator AddHandler<T, T1>(this IRegistrationConfigurator configurator,
            Func<ConsumeContext<T>, T1, Task> handler)
            where T : class
            where T1 : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            configurator.TryAddSingleton(new MessageHandlerMethod<T, T1>(handler));

            return configurator.AddConsumer<MessageHandlerConsumer<T, T1>, MessageHandlerConsumerDefinition<MessageHandlerConsumer<T, T1>, T>>();
        }

        /// <summary>
        /// Adds a method handler, using the first parameter to determine the message type
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="handler">An asynchronous method to handle the message</param>
        public static IConsumerRegistrationConfigurator AddHandler<T, T1, TResponse>(this IRegistrationConfigurator configurator,
            Func<ConsumeContext<T>, T1, Task<TResponse>> handler)
            where T : class
            where T1 : class
            where TResponse : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            configurator.TryAddSingleton(new RequestHandlerMethod<T, T1, TResponse>(handler));

            return configurator.AddConsumer<RequestHandlerConsumer<T, T1, TResponse>,
                MessageHandlerConsumerDefinition<RequestHandlerConsumer<T, T1, TResponse>, T>>();
        }

        /// <summary>
        /// Adds a method handler, using the first parameter to determine the message type
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="handler">An asynchronous method to handle the message</param>
        public static IConsumerRegistrationConfigurator AddHandler<T, T1>(this IRegistrationConfigurator configurator, Func<T, T1, Task> handler)
            where T : class
            where T1 : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            configurator.TryAddSingleton(new MessageHandlerMethod<T, T1>(handler));

            return configurator.AddConsumer<MessageHandlerConsumer<T, T1>, MessageHandlerConsumerDefinition<MessageHandlerConsumer<T, T1>, T>>();
        }

        /// <summary>
        /// Adds a method handler, using the first parameter to determine the message type
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="handler">An asynchronous method to handle the message</param>
        public static IConsumerRegistrationConfigurator AddHandler<T, T1, TResponse>(this IRegistrationConfigurator configurator,
            Func<T, T1, Task<TResponse>> handler)
            where T : class
            where T1 : class
            where TResponse : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            configurator.TryAddSingleton(new RequestHandlerMethod<T, T1, TResponse>(handler));

            return configurator.AddConsumer<RequestHandlerConsumer<T, T1, TResponse>,
                MessageHandlerConsumerDefinition<RequestHandlerConsumer<T, T1, TResponse>, T>>();
        }

        /// <summary>
        /// Adds a method handler, using the first parameter to determine the message type
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="handler">An asynchronous method to handle the message</param>
        public static IConsumerRegistrationConfigurator AddHandler<T, T1, T2>(this IRegistrationConfigurator configurator,
            Func<ConsumeContext<T>, T1, T2, Task> handler)
            where T : class
            where T1 : class
            where T2 : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            configurator.TryAddSingleton(new MessageHandlerMethod<T, T1, T2>(handler));

            return configurator.AddConsumer<MessageHandlerConsumer<T, T1, T2>, MessageHandlerConsumerDefinition<MessageHandlerConsumer<T, T1, T2>, T>>();
        }

        /// <summary>
        /// Adds a method handler, using the first parameter to determine the message type
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="handler">An asynchronous method to handle the message</param>
        public static IConsumerRegistrationConfigurator AddHandler<T, T1, T2, TResponse>(this IRegistrationConfigurator configurator,
            Func<ConsumeContext<T>, T1, T2, Task<TResponse>> handler)
            where T : class
            where T1 : class
            where TResponse : class
            where T2 : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            configurator.TryAddSingleton(new RequestHandlerMethod<T, T1, T2, TResponse>(handler));

            return configurator.AddConsumer<RequestHandlerConsumer<T, T1, T2, TResponse>,
                MessageHandlerConsumerDefinition<RequestHandlerConsumer<T, T1, T2, TResponse>, T>>();
        }

        /// <summary>
        /// Adds a method handler, using the first parameter to determine the message type
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="handler">An asynchronous method to handle the message</param>
        public static IConsumerRegistrationConfigurator AddHandler<T, T1, T2>(this IRegistrationConfigurator configurator, Func<T, T1, T2, Task> handler)
            where T : class
            where T1 : class
            where T2 : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            configurator.TryAddSingleton(new MessageHandlerMethod<T, T1, T2>(handler));

            return configurator.AddConsumer<MessageHandlerConsumer<T, T1, T2>, MessageHandlerConsumerDefinition<MessageHandlerConsumer<T, T1, T2>, T>>();
        }

        /// <summary>
        /// Adds a method handler, using the first parameter to determine the message type
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="handler">An asynchronous method to handle the message</param>
        public static IConsumerRegistrationConfigurator AddHandler<T, T1, T2, TResponse>(this IRegistrationConfigurator configurator,
            Func<T, T1, T2, Task<TResponse>> handler)
            where T : class
            where T1 : class
            where TResponse : class
            where T2 : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            configurator.TryAddSingleton(new RequestHandlerMethod<T, T1, T2, TResponse>(handler));

            return configurator.AddConsumer<RequestHandlerConsumer<T, T1, T2, TResponse>,
                MessageHandlerConsumerDefinition<RequestHandlerConsumer<T, T1, T2, TResponse>, T>>();
        }

        /// <summary>
        /// Adds a method handler, using the first parameter to determine the message type
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="handler">An asynchronous method to handle the message</param>
        public static IConsumerRegistrationConfigurator AddHandler<T, T1, T2, T3>(this IRegistrationConfigurator configurator,
            Func<ConsumeContext<T>, T1, T2, T3, Task> handler)
            where T : class
            where T1 : class
            where T2 : class
            where T3 : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            configurator.TryAddSingleton(new MessageHandlerMethod<T, T1, T2, T3>(handler));

            return configurator.AddConsumer<MessageHandlerConsumer<T, T1, T2, T3>, MessageHandlerConsumerDefinition<MessageHandlerConsumer<T, T1, T2, T3>,
                T>>();
        }

        /// <summary>
        /// Adds a method handler, using the first parameter to determine the message type
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="handler">An asynchronous method to handle the message</param>
        public static IConsumerRegistrationConfigurator AddHandler<T, T1, T2, T3, TResponse>(this IRegistrationConfigurator configurator,
            Func<ConsumeContext<T>, T1, T2, T3, Task<TResponse>> handler)
            where T : class
            where T1 : class
            where T2 : class
            where T3 : class
            where TResponse : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            configurator.TryAddSingleton(new RequestHandlerMethod<T, T1, T2, T3, TResponse>(handler));

            return configurator.AddConsumer<RequestHandlerConsumer<T, T1, T2, T3, TResponse>,
                MessageHandlerConsumerDefinition<RequestHandlerConsumer<T, T1, T2, T3, TResponse>, T>>();
        }

        /// <summary>
        /// Adds a method handler, using the first parameter to determine the message type
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="handler">An asynchronous method to handle the message</param>
        public static IConsumerRegistrationConfigurator AddHandler<T, T1, T2, T3>(this IRegistrationConfigurator configurator, Func<T, T1, T2, T3, Task>
            handler)
            where T : class
            where T1 : class
            where T2 : class
            where T3 : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            configurator.TryAddSingleton(new MessageHandlerMethod<T, T1, T2, T3>(handler));

            return configurator.AddConsumer<MessageHandlerConsumer<T, T1, T2, T3>,
                MessageHandlerConsumerDefinition<MessageHandlerConsumer<T, T1, T2, T3>, T>>();
        }

        /// <summary>
        /// Adds a method handler, using the first parameter to determine the message type
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="handler">An asynchronous method to handle the message</param>
        public static IConsumerRegistrationConfigurator AddHandler<T, T1, T2, T3, TResponse>(this IRegistrationConfigurator configurator,
            Func<T, T1, T2, T3, Task<TResponse>> handler)
            where T : class
            where T1 : class
            where T2 : class
            where T3 : class
            where TResponse : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            configurator.TryAddSingleton(new RequestHandlerMethod<T, T1, T2, T3, TResponse>(handler));

            return configurator.AddConsumer<RequestHandlerConsumer<T, T1, T2, T3, TResponse>,
                MessageHandlerConsumerDefinition<RequestHandlerConsumer<T, T1, T2, T3, TResponse>, T>>();
        }
    }
}
