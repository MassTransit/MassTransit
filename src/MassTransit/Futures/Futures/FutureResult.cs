namespace MassTransit.Futures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using SagaStateMachine;


    public class FutureResult<TCommand, TResult, TInput> :
        ISpecification
        where TCommand : class
        where TResult : class
        where TInput : class
    {
        ContextMessageFactory<BehaviorContext<FutureState, TInput>, TResult> _factory;

        public ContextMessageFactory<BehaviorContext<FutureState, TInput>, TResult> Factory
        {
            set => _factory = value;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_factory == null)
                yield return this.Failure("Response", "Factory", "Init or Create must be configured");
        }

        public async Task SetResult(BehaviorContext<FutureState, TInput> context)
        {
            context.SetCompleted(context.Saga.CorrelationId);

            var result = await context.SendMessageToSubscriptions(_factory,
                context.Saga.HasSubscriptions() ? context.Saga.Subscriptions.ToArray() : Array.Empty<FutureSubscription>());

            context.SetResult(context.Saga.CorrelationId, result);
        }
    }


    public class FutureResult<TCommand, TResult> :
        ISpecification
        where TCommand : class
        where TResult : class
    {
        ContextMessageFactory<BehaviorContext<FutureState>, TResult> _factory;

        public ContextMessageFactory<BehaviorContext<FutureState>, TResult> Factory
        {
            set => _factory = value;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_factory == null)
                yield return this.Failure("Response", "Factory", "Init or Create must be configured");
        }

        public async Task SetResult(BehaviorContext<FutureState> context)
        {
            context.SetCompleted(context.Saga.CorrelationId);

            var result = await context.SendMessageToSubscriptions(_factory,
                context.Saga.HasSubscriptions() ? context.Saga.Subscriptions.ToArray() : Array.Empty<FutureSubscription>());

            context.SetResult(context.Saga.CorrelationId, result);
        }
    }
}
