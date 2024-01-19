namespace MassTransit.Mediator
{
    using Internals;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// A Mediator request handler base class that provides a simplified overridable method with
    /// a Task (void) return type
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    public abstract class MediatorRequestHandler<TRequest> :
        IConsumer<TRequest>
        where TRequest : class
    {
        public Task Consume(ConsumeContext<TRequest> context)
        {
            return Handle(context.Message, context.CancellationToken);
        }

        protected abstract Task Handle(TRequest request, CancellationToken cancellationToken);
    }


    /// <summary>
    /// A Mediator request handler base class that provides a simplified overridable method with
    /// a Task&lt;<typeparamref name="TResponse"/>&gt; return type
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public abstract class MediatorRequestHandler<TRequest, TResponse> :
        IConsumer<TRequest>
        where TRequest : class, Request<TResponse>
        where TResponse : class
    {
        public async Task Consume(ConsumeContext<TRequest> context)
        {
            var response = await Handle(context.Message, context.CancellationToken).ConfigureAwait(false);

            if (response is not null)
            {
                await context.RespondAsync(response).ConfigureAwait(false);
            }
            else
            {
                await context.RespondAsync(Null<TResponse>.Value).ConfigureAwait(false);
            }
        }

        protected abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}
