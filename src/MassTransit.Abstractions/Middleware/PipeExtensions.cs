#nullable enable
namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Middleware;


    public static class PipeExtensions
    {
        /// <summary>
        /// Returns true if the pipe is not empty
        /// </summary>
        /// <param name="pipe"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsNotEmpty<T>(this IPipe<T>? pipe)
            where T : class, PipeContext
        {
            return pipe switch
            {
                null => false,
                EmptyPipe<T> _ => false,
                _ => true
            };
        }

        /// <summary>
        /// Returns true if the pipe is empty
        /// </summary>
        /// <param name="pipe"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsEmpty<T>(this IPipe<T>? pipe)
            where T : class, PipeContext
        {
            return pipe switch
            {
                null => true,
                EmptyPipe<T> _ => true,
                _ => false
            };
        }

        /// <summary>
        /// Get a payload from the pipe context
        /// </summary>
        /// <typeparam name="TPayload">The payload type</typeparam>
        /// <param name="context">The pipe context</param>
        /// <returns>The payload, or throws a PayloadNotFoundException if the payload is not present</returns>
        public static TPayload GetPayload<TPayload>(this PipeContext context)
            where TPayload : class
        {
            if (!context.TryGetPayload(out TPayload? payload))
                throw new PayloadNotFoundException($"The payload was not found: {TypeCache<TPayload>.ShortName}");

            return payload!;
        }

        /// <summary>
        /// Get a payload from the pipe context
        /// </summary>
        /// <typeparam name="TPayload">The payload type</typeparam>
        /// <param name="context">The pipe context</param>
        /// <param name="defaultPayload"></param>
        /// <returns>The payload, or the default Value</returns>
        public static TPayload GetPayload<TPayload>(this PipeContext context, TPayload defaultPayload)
            where TPayload : class
        {
            return context.TryGetPayload(out TPayload? payload) ? payload! : defaultPayload;
        }

        /// <summary>
        /// Using a filter-supplied context type, block so that the one time code is only executed once regardless of how many
        /// threads are pushing through the pipe at the same time.
        /// </summary>
        /// <typeparam name="T">The payload type, should be an interface</typeparam>
        /// <param name="context">The pipe context</param>
        /// <param name="setupMethod">The setup method, called once regardless of the thread count</param>
        /// <param name="payloadFactory">The factory method for the payload context, optional if an interface is specified</param>
        /// <returns></returns>
        public static async Task OneTimeSetup<T>(this PipeContext context, Func<T, Task> setupMethod, PayloadFactory<T> payloadFactory)
            where T : class
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (setupMethod == null)
                throw new ArgumentNullException(nameof(setupMethod));
            if (payloadFactory == null)
                throw new ArgumentNullException(nameof(payloadFactory));

            OneTime<T>? newContext = null;
            var existingContext = context.GetOrAddPayload<OneTimeSetupContext<T>>(() =>
            {
                var payload = payloadFactory();

                newContext = new OneTime<T>(payload);

                return newContext;
            });

            if (newContext == existingContext)
            {
                try
                {
                    await setupMethod(newContext.Payload).ConfigureAwait(false);

                    newContext.SetReady();
                }
                catch (Exception exception)
                {
                    newContext.SetFaulted(exception);

                    throw;
                }
            }
            else
                await existingContext.Ready.ConfigureAwait(false);
        }


        interface OneTimeSetupContext<TPayload>
            where TPayload : class
        {
            Task<TPayload> Ready { get; }
        }


        class OneTime<TPayload> :
            OneTimeSetupContext<TPayload>
            where TPayload : class
        {
            readonly TaskCompletionSource<TPayload> _ready;

            public OneTime(TPayload payload)
            {
                Payload = payload;
                _ready = new TaskCompletionSource<TPayload>(TaskCreationOptions.None | TaskCreationOptions.RunContinuationsAsynchronously);
            }

            public TPayload Payload { get; }

            public Task<TPayload> Ready => _ready.Task;

            public void SetReady()
            {
                _ready.TrySetResult(Payload);
            }

            public void SetFaulted(Exception exception)
            {
                _ready.TrySetException(exception);
            }
        }
    }
}
