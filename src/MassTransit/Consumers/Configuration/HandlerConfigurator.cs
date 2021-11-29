namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using Observables;


    /// <summary>
    /// Connects a handler to the inbound pipe of the receive endpoint
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class HandlerConfigurator<TMessage> :
        IHandlerConfigurator<TMessage>,
        IReceiveEndpointSpecification
        where TMessage : class
    {
        readonly IPipeSpecification<ConsumeContext<TMessage>> _handlerConfigurator;
        readonly HandlerConfigurationObservable _observers;
        readonly IBuildPipeConfigurator<ConsumeContext<TMessage>> _pipeConfigurator;

        public HandlerConfigurator(MessageHandler<TMessage> handler, IHandlerConfigurationObserver observer)
        {
            _pipeConfigurator = new PipeConfigurator<ConsumeContext<TMessage>>();
            _handlerConfigurator = new HandlerPipeSpecification<TMessage>(handler);
            _observers = new HandlerConfigurationObservable();

            _observers.Connect(observer);
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TMessage>> specification)
        {
            _pipeConfigurator.AddPipeSpecification(specification);
        }

        public ConnectHandle ConnectHandlerConfigurationObserver(IHandlerConfigurationObserver observer)
        {
            return _observers.Connect(observer);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _handlerConfigurator.Validate().Concat(_pipeConfigurator.Validate());
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            _observers.ForEach(observer => observer.HandlerConfigured(this));

            _pipeConfigurator.AddPipeSpecification(_handlerConfigurator);

            IPipe<ConsumeContext<TMessage>> pipe = _pipeConfigurator.Build();

            builder.ConnectConsumePipe(pipe);
        }
    }
}
