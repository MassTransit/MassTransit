namespace MassTransit.Initializers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Factories;


    public class MessageInitializerCache<TMessage> :
        IMessageInitializerCache<TMessage>
        where TMessage : class
    {
        readonly IDictionary<Type, Lazy<IMessageInitializer<TMessage>>> _initializers;

        MessageInitializerCache()
        {
            _initializers = new Dictionary<Type, Lazy<IMessageInitializer<TMessage>>>();
        }

        IMessageInitializer<TMessage> IMessageInitializerCache<TMessage>.GetInitializer(Type inputType)
        {
            Lazy<IMessageInitializer<TMessage>> result;
            lock (_initializers)
            {
                if (_initializers.TryGetValue(inputType, out Lazy<IMessageInitializer<TMessage>> initializer))
                    return initializer.Value;

                result = new Lazy<IMessageInitializer<TMessage>>(() => CreateMessageInitializer(inputType));

                _initializers[inputType] = result;
            }

            return result.Value;
        }

        static IMessageInitializer<TMessage> CreateMessageInitializer(Type inputType)
        {
            var factoryType = typeof(MessageInitializerFactory<,>).MakeGenericType(typeof(TMessage), inputType);

            var factory = (IMessageInitializerFactory<TMessage>)Activator.CreateInstance(factoryType, new object[] { MessageInitializer.Conventions });

            return factory.CreateMessageInitializer();
        }

        /// <summary>
        /// Returns the initializer for the message/input type combination
        /// </summary>
        /// <returns></returns>
        public static IMessageInitializer<TMessage> GetInitializer(Type inputType)
        {
            return Cached.InitializerCache.GetInitializer(inputType);
        }

        public static Task<InitializeContext<TMessage>> Initialize(object values, CancellationToken cancellationToken = default)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return Cached.InitializerCache.GetInitializer(values.GetType()).Initialize(values, cancellationToken);
        }

        public static Task<SendTuple<TMessage>> InitializeMessage(PipeContext context, object values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            IMessageInitializer<TMessage> initializer = Cached.InitializerCache.GetInitializer(values.GetType());

            return initializer.InitializeMessage(context, values);
        }

        public static Task<SendTuple<TMessage>> InitializeMessage(PipeContext context, object values, object[] moreValues,
            IPipe<SendContext<TMessage>> pipe = null)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            IMessageInitializer<TMessage> initializer = Cached.InitializerCache.GetInitializer(values.GetType());

            return initializer.InitializeMessage(context, values, moreValues, pipe);
        }

        public static Task<SendTuple<TMessage>> InitializeMessage(PipeContext context, object values, IPipe<SendContext<TMessage>> pipe)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            IMessageInitializer<TMessage> initializer = Cached.InitializerCache.GetInitializer(values.GetType());

            return initializer.InitializeMessage(context, values, pipe.IsNotEmpty() ? pipe : Pipe.Empty<SendContext<TMessage>>());
        }

        public static Task<SendTuple<TMessage>> InitializeMessage(object values, CancellationToken cancellationToken = default)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            IMessageInitializer<TMessage> initializer = Cached.InitializerCache.GetInitializer(values.GetType());

            return initializer.InitializeMessage(values, Pipe.Empty<SendContext<TMessage>>(), cancellationToken);
        }

        public static Task<SendTuple<TMessage>> InitializeMessage(object values, IPipe<SendContext<TMessage>> pipe,
            CancellationToken cancellationToken = default)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            IMessageInitializer<TMessage> initializer = Cached.InitializerCache.GetInitializer(values.GetType());

            return initializer.InitializeMessage(values, pipe.IsNotEmpty() ? pipe : Pipe.Empty<SendContext<TMessage>>(), cancellationToken);
        }

        public static Task<InitializeContext<TMessage>> Initialize(InitializeContext<TMessage> context, object values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return Cached.InitializerCache.GetInitializer(values.GetType()).Initialize(context, values);
        }


        static class Cached
        {
            internal static readonly IMessageInitializerCache<TMessage> InitializerCache = new MessageInitializerCache<TMessage>();
        }
    }
}
