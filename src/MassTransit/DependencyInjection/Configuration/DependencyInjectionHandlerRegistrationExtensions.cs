namespace MassTransit.Configuration
{
    using System;
    using System.Threading.Tasks;
    using DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;


    public static class DependencyInjectionHandlerRegistrationExtensions
    {
        public static IConsumerRegistration RegisterHandler<T>(this IServiceCollection collection)
            where T : class
        {
            return RegisterHandler<T>(collection, new DependencyInjectionContainerRegistrar(collection));
        }

        public static IConsumerRegistration RegisterHandler<T>(this IServiceCollection collection, IContainerRegistrar registrar)
            where T : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            collection.TryAddSingleton(new MessageHandlerMethod<T>((ConsumeContext<T> _) => Task.CompletedTask));

            return collection.RegisterConsumer<MessageHandlerConsumer<T>, MessageHandlerConsumerDefinition<MessageHandlerConsumer<T>, T>>(registrar);
        }

        public static IConsumerRegistration RegisterHandler<T>(this IServiceCollection collection, Func<ConsumeContext<T>, Task> handler)
            where T : class
        {
            return RegisterHandler(collection, new DependencyInjectionContainerRegistrar(collection), handler);
        }

        public static IConsumerRegistration RegisterHandler<T>(this IServiceCollection collection, IContainerRegistrar registrar,
            Func<ConsumeContext<T>, Task> handler)
            where T : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            collection.TryAddSingleton(new MessageHandlerMethod<T>(handler));

            return collection.RegisterConsumer<MessageHandlerConsumer<T>, MessageHandlerConsumerDefinition<MessageHandlerConsumer<T>, T>>(registrar);
        }

        public static IConsumerRegistration RegisterHandler<T>(this IServiceCollection collection, Func<T, Task> handler)
            where T : class
        {
            return RegisterHandler(collection, new DependencyInjectionContainerRegistrar(collection), handler);
        }

        public static IConsumerRegistration RegisterHandler<T>(this IServiceCollection collection, IContainerRegistrar registrar, Func<T, Task> handler)
            where T : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            collection.TryAddSingleton(new MessageHandlerMethod<T>(handler));

            return collection.RegisterConsumer<MessageHandlerConsumer<T>, MessageHandlerConsumerDefinition<MessageHandlerConsumer<T>, T>>(registrar);
        }

        public static IConsumerRegistration RegisterHandler<T, TResponse>(this IServiceCollection collection, Func<ConsumeContext<T>, Task<TResponse>> handler)
            where T : class
            where TResponse : class
        {
            return RegisterHandler(collection, new DependencyInjectionContainerRegistrar(collection), handler);
        }

        public static IConsumerRegistration RegisterHandler<T, TResponse>(this IServiceCollection collection, IContainerRegistrar registrar,
            Func<ConsumeContext<T>, Task<TResponse>> handler)
            where T : class
            where TResponse : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            if (!MessageTypeCache<TResponse>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<TResponse>.InvalidMessageTypeReason, nameof(TResponse));

            collection.TryAddSingleton(new RequestHandlerMethod<T, TResponse>(handler));

            return collection
                .RegisterConsumer<RequestHandlerConsumer<T, TResponse>, MessageHandlerConsumerDefinition<RequestHandlerConsumer<T, TResponse>, T>>(registrar);
        }

        public static IConsumerRegistration RegisterHandler<T, TResponse>(this IServiceCollection collection, Func<T, Task<TResponse>> handler)
            where T : class
            where TResponse : class
        {
            return RegisterHandler(collection, new DependencyInjectionContainerRegistrar(collection), handler);
        }

        public static IConsumerRegistration RegisterHandler<T, TResponse>(this IServiceCollection collection, IContainerRegistrar registrar,
            Func<T, Task<TResponse>> handler)
            where T : class
            where TResponse : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            if (!MessageTypeCache<TResponse>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<TResponse>.InvalidMessageTypeReason, nameof(TResponse));

            collection.TryAddSingleton(new RequestHandlerMethod<T, TResponse>(handler));

            return collection
                .RegisterConsumer<RequestHandlerConsumer<T, TResponse>, MessageHandlerConsumerDefinition<RequestHandlerConsumer<T, TResponse>, T>>(registrar);
        }

        public static IConsumerRegistration RegisterHandler<T, T1>(this IServiceCollection collection, Func<ConsumeContext<T>, T1, Task> handler)
            where T : class
            where T1 : class
        {
            return RegisterHandler(collection, new DependencyInjectionContainerRegistrar(collection), handler);
        }

        public static IConsumerRegistration RegisterHandler<T, T1>(this IServiceCollection collection, IContainerRegistrar registrar,
            Func<ConsumeContext<T>, T1, Task> handler)
            where T : class
            where T1 : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            collection.TryAddSingleton(new MessageHandlerMethod<T, T1>(handler));

            return collection.RegisterConsumer<MessageHandlerConsumer<T, T1>, MessageHandlerConsumerDefinition<MessageHandlerConsumer<T, T1>, T>>(registrar);
        }

        public static IConsumerRegistration RegisterHandler<T, T1>(this IServiceCollection collection, Func<T, T1, Task> handler)
            where T : class
            where T1 : class
        {
            return RegisterHandler(collection, new DependencyInjectionContainerRegistrar(collection), handler);
        }

        public static IConsumerRegistration RegisterHandler<T, T1>(this IServiceCollection collection, IContainerRegistrar registrar, Func<T, T1, Task> handler)
            where T : class
            where T1 : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            collection.TryAddSingleton(new MessageHandlerMethod<T, T1>(handler));

            return collection.RegisterConsumer<MessageHandlerConsumer<T, T1>, MessageHandlerConsumerDefinition<MessageHandlerConsumer<T, T1>, T>>(registrar);
        }

        public static IConsumerRegistration RegisterHandler<T, T1, TResponse>(this IServiceCollection collection, Func<ConsumeContext<T>, T1, Task<TResponse>>
            handler)
            where T : class
            where T1 : class
            where TResponse : class
        {
            return RegisterHandler(collection, new DependencyInjectionContainerRegistrar(collection), handler);
        }

        public static IConsumerRegistration RegisterHandler<T, T1, TResponse>(this IServiceCollection collection, IContainerRegistrar registrar,
            Func<ConsumeContext<T>, T1, Task<TResponse>> handler)
            where T : class
            where T1 : class
            where TResponse : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            if (!MessageTypeCache<TResponse>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<TResponse>.InvalidMessageTypeReason, nameof(TResponse));

            collection.TryAddSingleton(new RequestHandlerMethod<T, T1, TResponse>(handler));

            return collection
                .RegisterConsumer<RequestHandlerConsumer<T, T1, TResponse>,
                    MessageHandlerConsumerDefinition<RequestHandlerConsumer<T, T1, TResponse>, T>>(registrar);
        }

        public static IConsumerRegistration RegisterHandler<T, T1, TResponse>(this IServiceCollection collection, Func<T, T1, Task<TResponse>> handler)
            where T : class
            where T1 : class
            where TResponse : class
        {
            return RegisterHandler(collection, new DependencyInjectionContainerRegistrar(collection), handler);
        }

        public static IConsumerRegistration RegisterHandler<T, T1, TResponse>(this IServiceCollection collection, IContainerRegistrar registrar,
            Func<T, T1, Task<TResponse>> handler)
            where T : class
            where T1 : class
            where TResponse : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            if (!MessageTypeCache<TResponse>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<TResponse>.InvalidMessageTypeReason, nameof(TResponse));

            collection.TryAddSingleton(new RequestHandlerMethod<T, T1, TResponse>(handler));

            return collection
                .RegisterConsumer<RequestHandlerConsumer<T, T1, TResponse>,
                    MessageHandlerConsumerDefinition<RequestHandlerConsumer<T, T1, TResponse>, T>>(registrar);
        }

        public static IConsumerRegistration RegisterHandler<T, T1, T2>(this IServiceCollection collection, Func<ConsumeContext<T>, T1, T2, Task> handler)
            where T : class
            where T1 : class
            where T2 : class
        {
            return RegisterHandler(collection, new DependencyInjectionContainerRegistrar(collection), handler);
        }

        public static IConsumerRegistration RegisterHandler<T, T1, T2>(this IServiceCollection collection, IContainerRegistrar registrar,
            Func<ConsumeContext<T>, T1, T2, Task> handler)
            where T : class
            where T1 : class
            where T2 : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            collection.TryAddSingleton(new MessageHandlerMethod<T, T1, T2>(handler));

            return collection.RegisterConsumer<MessageHandlerConsumer<T, T1, T2>,
                MessageHandlerConsumerDefinition<MessageHandlerConsumer<T, T1, T2>, T>>(registrar);
        }

        public static IConsumerRegistration RegisterHandler<T, T1, T2>(this IServiceCollection collection, Func<T, T1, T2, Task> handler)
            where T : class
            where T1 : class
            where T2 : class
        {
            return RegisterHandler(collection, new DependencyInjectionContainerRegistrar(collection), handler);
        }

        public static IConsumerRegistration RegisterHandler<T, T1, T2>(this IServiceCollection collection, IContainerRegistrar registrar, Func<T, T1, T2, Task>
            handler)
            where T : class
            where T1 : class
            where T2 : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            collection.TryAddSingleton(new MessageHandlerMethod<T, T1, T2>(handler));

            return collection.RegisterConsumer<MessageHandlerConsumer<T, T1, T2>,
                MessageHandlerConsumerDefinition<MessageHandlerConsumer<T, T1, T2>, T>>(registrar);
        }

        public static IConsumerRegistration RegisterHandler<T, T1, T2, TResponse>(this IServiceCollection collection, Func<ConsumeContext<T>, T1, T2,
                Task<TResponse>>
            handler)
            where T : class
            where T1 : class
            where T2 : class
            where TResponse : class
        {
            return RegisterHandler(collection, new DependencyInjectionContainerRegistrar(collection), handler);
        }

        public static IConsumerRegistration RegisterHandler<T, T1, T2, TResponse>(this IServiceCollection collection, IContainerRegistrar registrar,
            Func<ConsumeContext<T>, T1, T2, Task<TResponse>> handler)
            where T : class
            where T1 : class
            where T2 : class
            where TResponse : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            if (!MessageTypeCache<TResponse>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<TResponse>.InvalidMessageTypeReason, nameof(TResponse));

            collection.TryAddSingleton(new RequestHandlerMethod<T, T1, T2, TResponse>(handler));

            return collection
                .RegisterConsumer<RequestHandlerConsumer<T, T1, T2, TResponse>,
                    MessageHandlerConsumerDefinition<RequestHandlerConsumer<T, T1, T2, TResponse>, T>>(registrar);
        }

        public static IConsumerRegistration RegisterHandler<T, T1, T2, TResponse>(this IServiceCollection collection, Func<T, T1, T2, Task<TResponse>> handler)
            where T : class
            where T1 : class
            where T2 : class
            where TResponse : class
        {
            return RegisterHandler(collection, new DependencyInjectionContainerRegistrar(collection), handler);
        }

        public static IConsumerRegistration RegisterHandler<T, T1, T2, TResponse>(this IServiceCollection collection, IContainerRegistrar registrar,
            Func<T, T1, T2, Task<TResponse>> handler)
            where T : class
            where T1 : class
            where T2 : class
            where TResponse : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            if (!MessageTypeCache<TResponse>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<TResponse>.InvalidMessageTypeReason, nameof(TResponse));

            collection.TryAddSingleton(new RequestHandlerMethod<T, T1, T2, TResponse>(handler));

            return collection
                .RegisterConsumer<RequestHandlerConsumer<T, T1, T2, TResponse>,
                    MessageHandlerConsumerDefinition<RequestHandlerConsumer<T, T1, T2, TResponse>, T>>(registrar);
        }

        public static IConsumerRegistration RegisterHandler<T, T1, T2, T3>(this IServiceCollection collection, Func<ConsumeContext<T>, T1, T2, T3, Task>
            handler)
            where T : class
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return RegisterHandler(collection, new DependencyInjectionContainerRegistrar(collection), handler);
        }

        public static IConsumerRegistration RegisterHandler<T, T1, T2, T3>(this IServiceCollection collection, IContainerRegistrar registrar,
            Func<ConsumeContext<T>, T1, T2, T3, Task> handler)
            where T : class
            where T1 : class
            where T2 : class
            where T3 : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            collection.TryAddSingleton(new MessageHandlerMethod<T, T1, T2, T3>(handler));

            return collection.RegisterConsumer<MessageHandlerConsumer<T, T1, T2, T3>,
                MessageHandlerConsumerDefinition<MessageHandlerConsumer<T, T1, T2, T3>, T>>(registrar);
        }

        public static IConsumerRegistration RegisterHandler<T, T1, T2, T3>(this IServiceCollection collection, Func<T, T1, T2, T3, Task> handler)
            where T : class
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return RegisterHandler(collection, new DependencyInjectionContainerRegistrar(collection), handler);
        }

        public static IConsumerRegistration RegisterHandler<T, T1, T2, T3>(this IServiceCollection collection, IContainerRegistrar registrar, Func<T, T1, T2,
                T3,
                Task>
            handler)
            where T : class
            where T1 : class
            where T2 : class
            where T3 : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            collection.TryAddSingleton(new MessageHandlerMethod<T, T1, T2, T3>(handler));

            return collection.RegisterConsumer<MessageHandlerConsumer<T, T1, T2, T3>,
                MessageHandlerConsumerDefinition<MessageHandlerConsumer<T, T1, T2, T3>, T>>(registrar);
        }

        public static IConsumerRegistration RegisterHandler<T, T1, T2, T3, TResponse>(this IServiceCollection collection, Func<ConsumeContext<T>, T1, T2, T3,
                Task<TResponse>>
            handler)
            where T : class
            where T1 : class
            where T2 : class
            where T3 : class
            where TResponse : class
        {
            return RegisterHandler(collection, new DependencyInjectionContainerRegistrar(collection), handler);
        }

        public static IConsumerRegistration RegisterHandler<T, T1, T2, T3, TResponse>(this IServiceCollection collection, IContainerRegistrar registrar,
            Func<ConsumeContext<T>, T1, T2, T3, Task<TResponse>> handler)
            where T : class
            where T1 : class
            where T2 : class
            where T3 : class
            where TResponse : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            if (!MessageTypeCache<TResponse>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<TResponse>.InvalidMessageTypeReason, nameof(TResponse));

            collection.TryAddSingleton(new RequestHandlerMethod<T, T1, T2, T3, TResponse>(handler));

            return collection
                .RegisterConsumer<RequestHandlerConsumer<T, T1, T2, T3, TResponse>,
                    MessageHandlerConsumerDefinition<RequestHandlerConsumer<T, T1, T2, T3, TResponse>, T>>(registrar);
        }

        public static IConsumerRegistration RegisterHandler<T, T1, T2, T3, TResponse>(this IServiceCollection collection, Func<T, T1, T2, T3, Task<TResponse>>
            handler)
            where T : class
            where T1 : class
            where T2 : class
            where T3 : class
            where TResponse : class
        {
            return RegisterHandler(collection, new DependencyInjectionContainerRegistrar(collection), handler);
        }

        public static IConsumerRegistration RegisterHandler<T, T1, T2, T3, TResponse>(this IServiceCollection collection, IContainerRegistrar registrar,
            Func<T, T1, T2, T3, Task<TResponse>> handler)
            where T : class
            where T1 : class
            where T2 : class
            where T3 : class
            where TResponse : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            if (!MessageTypeCache<TResponse>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<TResponse>.InvalidMessageTypeReason, nameof(TResponse));

            collection.TryAddSingleton(new RequestHandlerMethod<T, T1, T2, T3, TResponse>(handler));

            return collection
                .RegisterConsumer<RequestHandlerConsumer<T, T1, T2, T3, TResponse>,
                    MessageHandlerConsumerDefinition<RequestHandlerConsumer<T, T1, T2, T3, TResponse>, T>>(registrar);
        }
    }
}
