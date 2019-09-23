namespace MassTransit.Pipeline.Filters
{
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Initializers;
    using Transformation.Contexts;


    /// <summary>
    /// Applies a transform to the message
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    public class TransformFilter<T> :
        IFilter<ConsumeContext<T>>,
        IFilter<SendContext<T>>
        where T : class
    {
        readonly IMessageInitializer<T> _initializer;

        public TransformFilter(IMessageInitializer<T> initializer)
        {
            _initializer = initializer;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("transform");
        }

        Task IFilter<ConsumeContext<T>>.Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            var transformContext = new ConsumeTransformContext<T>(context);

            var initializeTask = _initializer.Initialize(_initializer.Create(transformContext), context.Message);
            if (initializeTask.IsCompleted)
            {
                var message = initializeTask.Result.Message;

                return next.Send(ReferenceEquals(message, context.Message)
                    ? context
                    : new MessageConsumeContext<T>(context, message));
            }

            async Task SendAsync()
            {
                var initializeContext = await initializeTask.ConfigureAwait(false);

                await next.Send(ReferenceEquals(initializeContext.Message, context.Message)
                    ? context
                    : new MessageConsumeContext<T>(context, initializeContext.Message)).ConfigureAwait(false);
            }

            return SendAsync();
        }

        Task IFilter<SendContext<T>>.Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            var transformContext = new SendTransformContext<T>(context);

            var initializeTask = _initializer.Initialize(_initializer.Create(transformContext), context.Message);
            if (initializeTask.IsCompleted)
            {
                var message = initializeTask.Result.Message;

                return next.Send(ReferenceEquals(message, context.Message)
                    ? context
                    : context.CreateProxy(message));
            }

            async Task SendAsync()
            {
                var initializeContext = await initializeTask.ConfigureAwait(false);

                await next.Send(ReferenceEquals(initializeContext.Message, context.Message)
                    ? context
                    : context.CreateProxy(initializeContext.Message)).ConfigureAwait(false);
            }

            return SendAsync();
        }
    }
}
