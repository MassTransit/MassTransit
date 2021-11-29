namespace MassTransit.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Futures;
    using Initializers;
    using SagaStateMachine;


    public class FutureResultConfigurator<TCommand, TResult, TInput> :
        IFutureResultConfigurator<TResult, TInput>
        where TCommand : class
        where TInput : class
        where TResult : class
    {
        readonly FutureResult<TCommand, TResult, TInput> _result;

        public FutureResultConfigurator(FutureResult<TCommand, TResult, TInput> result)
        {
            _result = result;
        }

        public void SetCompletedUsingFactory(EventMessageFactory<FutureState, TInput, TResult> factoryMethod)
        {
            if (factoryMethod == null)
                throw new ArgumentNullException(nameof(factoryMethod));

            _result.Factory = MessageFactory<TResult>.Create(factoryMethod);
        }

        public void SetCompletedUsingFactory(AsyncEventMessageFactory<FutureState, TInput, TResult> factoryMethod)
        {
            if (factoryMethod == null)
                throw new ArgumentNullException(nameof(factoryMethod));

            _result.Factory = MessageFactory<TResult>.Create(factoryMethod);
        }

        public void SetCompletedUsingInitializer(InitializerValueProvider<TInput> valueProvider)
        {
            if (valueProvider == null)
                throw new ArgumentNullException(nameof(valueProvider));

            Task<SendTuple<TResult>> Factory(BehaviorContext<FutureState, TInput> context)
            {
                return MessageInitializerCache<TResult>.InitializeMessage(context, valueProvider(context), new object[]
                {
                    new
                    {
                        context.Saga.Completed,
                        context.Saga.Created,
                        context.Saga.Faulted,
                        context.Saga.Location,
                    },
                    context.GetCommand<TCommand>(),
                    context.Message
                });
            }

            _result.Factory = MessageFactory<TResult>.Create((Func<BehaviorContext<FutureState, TInput>, Task<SendTuple<TResult>>>)Factory);
        }
    }


    public class FutureResultConfigurator<TCommand, TResult> :
        IFutureResultConfigurator<TResult>
        where TCommand : class
        where TResult : class
    {
        readonly FutureResult<TCommand, TResult> _result;

        public FutureResultConfigurator(FutureResult<TCommand, TResult> result)
        {
            _result = result;
        }

        public void SetCompletedUsingFactory(EventMessageFactory<FutureState, TResult> factoryMethod)
        {
            if (factoryMethod == null)
                throw new ArgumentNullException(nameof(factoryMethod));

            _result.Factory = MessageFactory<TResult>.Create(factoryMethod);
        }

        public void SetCompletedUsingFactory(AsyncEventMessageFactory<FutureState, TResult> factoryMethod)
        {
            if (factoryMethod == null)
                throw new ArgumentNullException(nameof(factoryMethod));

            _result.Factory = MessageFactory<TResult>.Create(factoryMethod);
        }

        public void SetCompletedUsingInitializer(InitializerValueProvider valueProvider)
        {
            if (valueProvider == null)
                throw new ArgumentNullException(nameof(valueProvider));

            Task<SendTuple<TResult>> Factory(BehaviorContext<FutureState> context)
            {
                return MessageInitializerCache<TResult>.InitializeMessage(context, valueProvider(context), new object[]
                {
                    new
                    {
                        context.Saga.Completed,
                        context.Saga.Created,
                        context.Saga.Faulted,
                        context.Saga.Location,
                    },
                    context.GetCommand<TCommand>()
                });
            }

            _result.Factory = MessageFactory<TResult>.Create((Func<BehaviorContext<FutureState>, Task<SendTuple<TResult>>>)Factory);
        }
    }
}
