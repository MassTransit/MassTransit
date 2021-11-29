namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class ObserverConfigurator<TMessage> :
        IObserverConfigurator<TMessage>,
        IReceiveEndpointSpecification
        where TMessage : class
    {
        readonly IPipeSpecification<ConsumeContext<TMessage>> _handlerConfigurator;
        readonly IBuildPipeConfigurator<ConsumeContext<TMessage>> _pipeConfigurator;

        public ObserverConfigurator(IObserver<ConsumeContext<TMessage>> observer)
        {
            _pipeConfigurator = new PipeConfigurator<ConsumeContext<TMessage>>();
            _handlerConfigurator = new ObserverPipeSpecification<TMessage>(observer);
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TMessage>> specification)
        {
            _pipeConfigurator.AddPipeSpecification(specification);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _handlerConfigurator.Validate().Concat(_pipeConfigurator.Validate());
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            _pipeConfigurator.AddPipeSpecification(_handlerConfigurator);

            IPipe<ConsumeContext<TMessage>> pipe = _pipeConfigurator.Build();

            builder.ConnectConsumePipe(pipe);
        }
    }
}
