namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Initializers;


    public static class MessageInitializerExtensions
    {
        public struct ToMessage<T>
            where T : class
        {
            readonly ConsumeContext<T> _context;

            public ToMessage(ConsumeContext<T> context)
            {
                _context = context;
            }

            public async Task<TMessage> Init<TMessage>(object values, CancellationToken cancellationToken = default)
                where TMessage : class
            {
                if (values == null)
                    throw new ArgumentNullException(nameof(values));

                var messageContext = await MessageInitializerCache<TMessage>.Initialize(_context.Message, cancellationToken).ConfigureAwait(false);

                var resultContext = await MessageInitializerCache<TMessage>.Initialize(messageContext, values).ConfigureAwait(false);

                return resultContext.Message;
            }
        }


        public static ToMessage<T> To<T>(this ConsumeContext<T> context)
            where T : class
        {
            return new ToMessage<T>(context);
        }

        /// <summary>
        /// Initialize a message using the specified input values (via an anonymous object, or an actual object)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="selector"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> Init<T>(this ConsumeContext context, Func<IInitSelector, Task<InitializeContext<T>>> selector)
            where T : class
        {
            var initSelector = new InitSelector();

            var result = await selector(initSelector).ConfigureAwait(false);

            return result.Message;
        }

        /// <summary>
        /// Initialize a message using the specified input values (via an anonymous object, or an actual object)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="selector"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static async Task<TResult> Init<T, TResult>(this ConsumeContext<T> context,
            Func<IInitSelector<T>, Task<InitializeContext<TResult>>> selector)
            where T : class
            where TResult : class
        {
            var initSelector = new InitSelector<T>(context);

            var result = await selector(initSelector).ConfigureAwait(false);

            return result.Message;
        }


        public interface IInitSelector
        {
            Task<InitializeContext<TResult>> Init<TResult>(object values, CancellationToken cancellationToken = default)
                where TResult : class;
        }


        public class InitSelector :
            IInitSelector
        {
            public Task<InitializeContext<TResult>> Init<TResult>(object values, CancellationToken cancellationToken = default)
                where TResult : class
            {
                return MessageInitializerCache<TResult>.Initialize(values, cancellationToken);
            }
        }


        public interface IInitSelector<T>
        {
            Task<InitializeContext<TResult>> Init<TResult>(object values, CancellationToken cancellationToken = default)
                where TResult : class;
        }


        public class InitSelector<T> :
            IInitSelector<T>
            where T : class
        {
            readonly ConsumeContext<T> _context;

            public InitSelector(ConsumeContext<T> context)
            {
                _context = context;
            }

            public async Task<InitializeContext<TResult>> Init<TResult>(object values, CancellationToken cancellationToken = default)
                where TResult : class
            {
                var contextMessage = await MessageInitializerCache<TResult>.Initialize(_context.Message, cancellationToken).ConfigureAwait(false);

                return await MessageInitializerCache<TResult>.Initialize(contextMessage, values).ConfigureAwait(false);
            }
        }
    }
}
