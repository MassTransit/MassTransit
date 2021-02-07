namespace MassTransit.Futures.Endpoints
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Internals;


    public class FutureResult<TCommand, TResult, TInput> :
        ISpecification
        where TCommand : class
        where TResult : class
        where TInput : class
    {
        IResultEndpoint<TResult> _endpoint;

        public AsyncFutureMessageFactory<TResult, TInput> Factory
        {
            set => _endpoint = new FactoryResultEndpoint<TResult, TInput>(value);
        }

        public InitializerValueProvider<TResult> Initializer
        {
            set => _endpoint = new InitializerResultEndpoint<TCommand, TResult, TInput>(value);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_endpoint == null)
                yield return this.Failure("Response", "Factory", "Init or Create must be configured");
        }

        public Task SetResult(BehaviorContext<FutureState, TResult> context)
        {
            FutureConsumeContext<TResult> consumeContext = context.CreateFutureConsumeContext();

            return consumeContext.Instance.HasSubscriptions()
                ? _endpoint.SendResponse(consumeContext, consumeContext.Instance.Subscriptions.ToArray())
                : _endpoint.SendResponse(consumeContext);
        }
    }


    public class FutureResult<TCommand, TResult> :
        ISpecification
        where TCommand : class
        where TResult : class
    {
        IResultEndpoint _endpoint;

        public AsyncFutureMessageFactory<TResult> Factory
        {
            set => _endpoint = new FactoryResultEndpoint<TResult>(value);
        }

        public InitializerValueProvider Initializer
        {
            set => _endpoint = new InitializerResultEndpoint<TCommand, TResult>(value);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_endpoint == null)
                yield return this.Failure("Response", "Factory", "Init or Create must be configured");
        }

        public Task SetResult(BehaviorContext<FutureState> context)
        {
            var consumeContext = context.CreateFutureConsumeContext();

            return consumeContext.Instance.HasSubscriptions()
                ? _endpoint.SendResponse(consumeContext, consumeContext.Instance.Subscriptions.ToArray())
                : _endpoint.SendResponse(consumeContext);
        }
    }
}
