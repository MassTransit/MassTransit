namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Context;
    using Initializers;
    using Transformation;


    /// <summary>
    /// Applies a transform to the message
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    public class TransformFilter<T> :
        IFilter<ConsumeContext<T>>,
        IFilter<ExecuteContext<T>>,
        IFilter<CompensateContext<T>>,
        IFilter<SendContext<T>>
        where T : class
    {
        readonly IMessageInitializer<T> _initializer;

        public TransformFilter(IMessageInitializer<T> initializer)
        {
            _initializer = initializer;
        }

        Task IFilter<CompensateContext<T>>.Send(CompensateContext<T> context, IPipe<CompensateContext<T>> next)
        {
            var transformContext = new ConsumeTransformContext<T>(context, context.Log);

            Task<InitializeContext<T>> initializeTask = _initializer.Initialize(_initializer.Create(transformContext), context.Log);
            if (initializeTask.Status == TaskStatus.RanToCompletion)
            {
                var log = initializeTask.Result.Message;

                return next.Send(ReferenceEquals(log, context.Log)
                    ? context
                    : new CompensateContextProxy<T>(context, log));
            }

            async Task SendAsync()
            {
                InitializeContext<T> initializeContext = await initializeTask.ConfigureAwait(false);

                await next.Send(ReferenceEquals(initializeContext.Message, context.Log)
                    ? context
                    : new CompensateContextProxy<T>(context, initializeContext.Message)).ConfigureAwait(false);
            }

            return SendAsync();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("transform");
        }

        Task IFilter<ConsumeContext<T>>.Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            var transformContext = new ConsumeTransformContext<T>(context, context.Message);

            Task<InitializeContext<T>> initializeTask = _initializer.Initialize(_initializer.Create(transformContext), context.Message);
            if (initializeTask.Status == TaskStatus.RanToCompletion)
            {
                var message = initializeTask.Result.Message;

                return next.Send(ReferenceEquals(message, context.Message)
                    ? context
                    : new MessageConsumeContext<T>(context, message));
            }

            async Task SendAsync()
            {
                InitializeContext<T> initializeContext = await initializeTask.ConfigureAwait(false);

                await next.Send(ReferenceEquals(initializeContext.Message, context.Message)
                    ? context
                    : new MessageConsumeContext<T>(context, initializeContext.Message)).ConfigureAwait(false);
            }

            return SendAsync();
        }

        Task IFilter<ExecuteContext<T>>.Send(ExecuteContext<T> context, IPipe<ExecuteContext<T>> next)
        {
            var transformContext = new ConsumeTransformContext<T>(context, context.Arguments);

            Task<InitializeContext<T>> initializeTask = _initializer.Initialize(_initializer.Create(transformContext), context.Arguments);
            if (initializeTask.Status == TaskStatus.RanToCompletion)
            {
                var arguments = initializeTask.Result.Message;

                return next.Send(ReferenceEquals(arguments, context.Arguments)
                    ? context
                    : new ExecuteContextProxy<T>(context, arguments));
            }

            async Task SendAsync()
            {
                InitializeContext<T> initializeContext = await initializeTask.ConfigureAwait(false);

                await next.Send(ReferenceEquals(initializeContext.Message, context.Arguments)
                    ? context
                    : new ExecuteContextProxy<T>(context, initializeContext.Message)).ConfigureAwait(false);
            }

            return SendAsync();
        }

        Task IFilter<SendContext<T>>.Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            var transformContext = new SendTransformContext<T>(context);

            Task<InitializeContext<T>> initializeTask = _initializer.Initialize(_initializer.Create(transformContext), context.Message);
            if (initializeTask.Status == TaskStatus.RanToCompletion)
            {
                var message = initializeTask.Result.Message;

                return next.Send(ReferenceEquals(message, context.Message)
                    ? context
                    : context.CreateProxy(message));
            }

            async Task SendAsync()
            {
                InitializeContext<T> initializeContext = await initializeTask.ConfigureAwait(false);

                await next.Send(ReferenceEquals(initializeContext.Message, context.Message)
                    ? context
                    : context.CreateProxy(initializeContext.Message)).ConfigureAwait(false);
            }

            return SendAsync();
        }
    }
}
