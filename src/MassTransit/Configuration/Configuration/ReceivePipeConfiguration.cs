namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using Pipeline;
    using Pipeline.Filters;
    using Pipeline.Pipes;


    public class ReceivePipeConfiguration :
        IReceivePipeConfiguration,
        IReceivePipeConfigurator,
        ISpecification
    {
        readonly IBuildPipeConfigurator<ReceiveContext> _configurator;
        bool _created;

        public ReceivePipeConfiguration()
        {
            _configurator = new PipeConfigurator<ReceiveContext>();
            DeadLetterConfigurator = new PipeConfigurator<ReceiveContext>();
            ErrorConfigurator = new PipeConfigurator<ExceptionReceiveContext>();
        }

        public ISpecification Specification => _configurator;

        public IReceivePipeConfigurator Configurator => this;

        public IBuildPipeConfigurator<ReceiveContext> DeadLetterConfigurator { get; }

        public IBuildPipeConfigurator<ExceptionReceiveContext> ErrorConfigurator { get; }

        public void AddPipeSpecification(IPipeSpecification<ReceiveContext> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _configurator.Validate()
                .Concat(DeadLetterConfigurator.Validate())
                .Concat(ErrorConfigurator.Validate());
        }

        public IReceivePipe CreatePipe(IConsumePipe consumePipe, IMessageDeserializer messageDeserializer)
        {
            if (_created)
                throw new ConfigurationException("The ReceivePipeConfiguration can only be used once.");

            _configurator.UseDeadLetter(CreateDeadLetterPipe());
            _configurator.UseRescue(CreateErrorPipe(), x =>
            {
                x.Ignore<OperationCanceledException>();
            });

            _configurator.UseFilter(new DeserializeFilter(messageDeserializer, consumePipe));

            _created = true;

            return new ReceivePipe(_configurator.Build(), consumePipe);
        }

        IPipe<ReceiveContext> CreateDeadLetterPipe()
        {
            var deadLetterPipe = DeadLetterConfigurator.Build();
            if (deadLetterPipe.IsNotEmpty())
                return deadLetterPipe;

            DeadLetterConfigurator.UseFilter(new DeadLetterTransportFilter());

            return DeadLetterConfigurator.Build();
        }

        IPipe<ExceptionReceiveContext> CreateErrorPipe()
        {
            var errorPipe = ErrorConfigurator.Build();
            if (errorPipe.IsNotEmpty())
                return errorPipe;

            ErrorConfigurator.UseFilter(new GenerateFaultFilter());
            ErrorConfigurator.UseFilter(new ErrorTransportFilter());

            return ErrorConfigurator.Build();
        }
    }
}
