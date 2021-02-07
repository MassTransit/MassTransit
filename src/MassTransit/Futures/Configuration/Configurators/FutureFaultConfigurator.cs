namespace MassTransit.Futures.Configurators
{
    using System;
    using System.Threading.Tasks;
    using Endpoints;


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

        public void SetFaultedUsingFactory(FutureMessageFactory<TInput, TFault> factoryMethod)
        {
            if (factoryMethod == null)
                throw new ArgumentNullException(nameof(factoryMethod));

            Task<TFault> AsyncFactoryMethod(FutureConsumeContext<TInput> context)
            {
                return Task.FromResult(factoryMethod(context));
            }

            _fault.SetFaultedUsingFactory(AsyncFactoryMethod);
        }

        public void SetFaultedUsingFactory(AsyncFutureMessageFactory<TInput, TFault> factoryMethod)
        {
            if (factoryMethod == null)
                throw new ArgumentNullException(nameof(factoryMethod));

            _fault.SetFaultedUsingFactory(factoryMethod);
        }

        public void SetFaultedUsingInitializer(InitializerValueProvider<TInput> valueProvider)
        {
            if (valueProvider == null)
                throw new ArgumentNullException(nameof(valueProvider));

            _fault.SetFaultedUsingInitializer(valueProvider);
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

        public void SetFaultedUsingFactory(FutureMessageFactory<TFault> factoryMethod)
        {
            if (factoryMethod == null)
                throw new ArgumentNullException(nameof(factoryMethod));

            Task<TFault> AsyncFactoryMethod(FutureConsumeContext context)
            {
                return Task.FromResult(factoryMethod(context));
            }

            _fault.SetFaultedUsingFactory(AsyncFactoryMethod);
        }

        public void SetFaultedUsingFactory(AsyncFutureMessageFactory<TFault> factoryMethod)
        {
            if (factoryMethod == null)
                throw new ArgumentNullException(nameof(factoryMethod));

            _fault.SetFaultedUsingFactory(factoryMethod);
        }

        public void SetFaultedUsingInitializer(InitializerValueProvider valueProvider)
        {
            if (valueProvider == null)
                throw new ArgumentNullException(nameof(valueProvider));

            _fault.SetFaultedUsingInitializer(valueProvider);
        }
    }
}
