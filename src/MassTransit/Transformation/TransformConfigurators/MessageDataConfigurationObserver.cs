namespace MassTransit.Transformation.TransformConfigurators
{
    using System;
    using ConsumeConfigurators;
    using Courier;
    using GreenPipes;
    using MessageData;
    using PipeConfigurators;


    public class MessageDataConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        readonly IMessageDataRepository _repository;

        public MessageDataConfigurationObserver(IConsumePipeConfigurator configurator, IMessageDataRepository repository)
            : base(configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            _repository = repository;

            Connect(this);
        }

        public override void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
        {
            IPipeSpecification<ExecuteContext<TArguments>> specification = new MessageDataTransformSpecification<TArguments>(_repository);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
        {
            IPipeSpecification<ExecuteContext<TArguments>> specification = new MessageDataTransformSpecification<TArguments>(_repository);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
        {
            IPipeSpecification<CompensateContext<TLog>> specification = new MessageDataTransformSpecification<TLog>(_repository);

            configurator.Log(x => x.AddPipeSpecification(specification));
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            IPipeSpecification<ConsumeContext<TMessage>> specification = new MessageDataTransformSpecification<TMessage>(_repository);

            configurator.AddPipeSpecification(specification);
        }
    }
}
