namespace MassTransit.Futures.Endpoints
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Automatonymous;
    using Configurators;
    using GreenPipes;
    using Internals;


    public class FutureFault<TCommand, TFault, TInput> :
        ISpecification
        where TCommand : class
        where TFault : class
        where TInput : class
    {
        IFaultEndpoint<TInput> _endpoint;

        public FutureFault()
        {
            _endpoint = new InitializerFaultEndpoint<TCommand, TFault, TInput>(FutureConfiguratorHelpers.DefaultProvider);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_endpoint == null)
                yield return this.Failure("Fault", "Factory", "Init or Create must be configured");
        }

        public void SetFaultedUsingInitializer(InitializerValueProvider<TInput> value)
        {
            _endpoint = new InitializerFaultEndpoint<TCommand, TFault, TInput>(value);
        }

        public void SetFaultedUsingFactory(AsyncFutureMessageFactory<TInput, TFault> value)
        {
            _endpoint = new FactoryFaultEndpoint<TInput, TFault>(value);
        }

        public Task SetFaulted(BehaviorContext<FutureState, TInput> context)
        {
            FutureConsumeContext<TInput> consumeContext = context.CreateFutureConsumeContext();

            return consumeContext.Instance.HasSubscriptions()
                ? _endpoint.SendFault(consumeContext, consumeContext.Instance.Subscriptions.ToArray())
                : _endpoint.SendFault(consumeContext);
        }
    }


    public class FutureFault<TFault> :
        ISpecification
        where TFault : class
    {
        IFaultEndpoint _endpoint;

        public FutureFault()
        {
            _endpoint = new InitializerFaultEndpoint<TFault>(FutureConfiguratorHelpers.DefaultProvider);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_endpoint == null)
                yield return this.Failure("Fault", "Factory", "Init or Create must be configured");
        }

        public void SetFaultedUsingFactory(AsyncFutureMessageFactory<TFault> value)
        {
            _endpoint = new FactoryFaultEndpoint<TFault>(value);
        }

        public void SetFaultedUsingInitializer(InitializerValueProvider value)
        {
            _endpoint = new InitializerFaultEndpoint<TFault>(value);
        }

        public Task SetFaulted(BehaviorContext<FutureState> context)
        {
            var consumeContext = context.CreateFutureConsumeContext();

            return consumeContext.Instance.HasSubscriptions()
                ? _endpoint.SendFault(consumeContext, consumeContext.Instance.Subscriptions.ToArray())
                : _endpoint.SendFault(consumeContext);
        }
    }
}
