namespace MassTransit.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Futures;
    using SagaStateMachine;


    public class FutureFaultConfigurator<TCommand, TFault, TInput> :
        IFutureFaultConfigurator<TFault, TInput>
        where TInput : class
        where TFault : class
        where TCommand : class
    {
        readonly FutureFault<TCommand, TFault, TInput> _fault;

        public FutureFaultConfigurator(FutureFault<TCommand, TFault, TInput> fault)
        {
            _fault = fault;
        }

        public void SetFaultedUsingFactory(EventMessageFactory<FutureState, TInput, TFault> factoryMethod)
        {
            if (factoryMethod == null)
                throw new ArgumentNullException(nameof(factoryMethod));

            _fault.Factory = MessageFactory<TFault>.Create(factoryMethod);
        }

        public void SetFaultedUsingFactory(AsyncEventMessageFactory<FutureState, TInput, TFault> factoryMethod)
        {
            if (factoryMethod == null)
                throw new ArgumentNullException(nameof(factoryMethod));

            _fault.Factory = MessageFactory<TFault>.Create(factoryMethod);
        }

        public void SetFaultedUsingInitializer(InitializerValueProvider<TInput> valueProvider)
        {
            if (valueProvider == null)
                throw new ArgumentNullException(nameof(valueProvider));

            Task<SendTuple<TFault>> Factory(BehaviorContext<FutureState, TInput> context)
            {
                return context.Init<TFault>(valueProvider(context));
            }

            _fault.Factory = MessageFactory<TFault>.Create((Func<BehaviorContext<FutureState, TInput>, Task<SendTuple<TFault>>>)Factory);
        }
    }


    public class FutureFaultConfigurator<TFault> :
        IFutureFaultConfigurator<TFault>
        where TFault : class
    {
        readonly FutureFault<TFault> _fault;

        public FutureFaultConfigurator(FutureFault<TFault> fault)
        {
            _fault = fault;
        }

        public void SetFaultedUsingFactory(EventMessageFactory<FutureState, TFault> factoryMethod)
        {
            if (factoryMethod == null)
                throw new ArgumentNullException(nameof(factoryMethod));

            _fault.Factory = MessageFactory<TFault>.Create(factoryMethod);
        }

        public void SetFaultedUsingFactory(AsyncEventMessageFactory<FutureState, TFault> factoryMethod)
        {
            if (factoryMethod == null)
                throw new ArgumentNullException(nameof(factoryMethod));

            _fault.Factory = MessageFactory<TFault>.Create(factoryMethod);
        }

        public void SetFaultedUsingInitializer(InitializerValueProvider valueProvider)
        {
            if (valueProvider == null)
                throw new ArgumentNullException(nameof(valueProvider));

            Task<SendTuple<TFault>> Factory(BehaviorContext<FutureState> context)
            {
                return context.Init<TFault>(valueProvider(context));
            }

            _fault.Factory = MessageFactory<TFault>.Create((Func<BehaviorContext<FutureState>, Task<SendTuple<TFault>>>)Factory);
        }
    }
}
