namespace MassTransit.Futures.Configurators
{
    using System;
    using System.Threading.Tasks;
    using Endpoints;


    public class FutureResultConfigurator<TCommand, TResult, TInput> :
        IFutureResultConfigurator<TResult, TInput>
        where TCommand : class
        where TInput : class
        where TResult : class
    {
        readonly FutureResult<TCommand, TInput, TResult> _result;

        public FutureResultConfigurator(FutureResult<TCommand, TInput, TResult> result)
        {
            _result = result;
        }

        public void SetCompletedUsingFactory(FutureMessageFactory<TInput, TResult> factoryMethod)
        {
            if (factoryMethod == null)
                throw new ArgumentNullException(nameof(factoryMethod));

            Task<TResult> AsyncFactoryMethod(FutureConsumeContext<TInput> context)
            {
                return Task.FromResult(factoryMethod(context));
            }

            _result.Factory = AsyncFactoryMethod;
        }

        public void SetCompletedUsingFactory(AsyncFutureMessageFactory<TInput, TResult> factoryMethod)
        {
            if (factoryMethod == null)
                throw new ArgumentNullException(nameof(factoryMethod));

            _result.Factory = factoryMethod;
        }

        public void SetCompletedUsingInitializer(InitializerValueProvider<TInput> valueProvider)
        {
            if (valueProvider == null)
                throw new ArgumentNullException(nameof(valueProvider));

            _result.Initializer = valueProvider;
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

        public void SetCompletedUsingFactory(FutureMessageFactory<TResult> factoryMethod)
        {
            if (factoryMethod == null)
                throw new ArgumentNullException(nameof(factoryMethod));

            Task<TResult> AsyncFactoryMethod(FutureConsumeContext context)
            {
                return Task.FromResult(factoryMethod(context));
            }

            _result.Factory = AsyncFactoryMethod;
        }

        public void SetCompletedUsingFactory(AsyncFutureMessageFactory<TResult> factoryMethod)
        {
            if (factoryMethod == null)
                throw new ArgumentNullException(nameof(factoryMethod));

            _result.Factory = factoryMethod;
        }

        public void SetCompletedUsingInitializer(InitializerValueProvider valueProvider)
        {
            if (valueProvider == null)
                throw new ArgumentNullException(nameof(valueProvider));

            _result.Initializer = valueProvider;
        }
    }
}
