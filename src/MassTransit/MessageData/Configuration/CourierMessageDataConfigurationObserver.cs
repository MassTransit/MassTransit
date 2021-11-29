namespace MassTransit.MessageData.Configuration
{
    using System;
    using MassTransit.Configuration;


    public class CourierMessageDataConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        readonly bool _includeMessages;
        readonly IMessageDataRepository _repository;

        public CourierMessageDataConfigurationObserver(IConsumePipeConfigurator configurator, IMessageDataRepository repository, bool includeMessages)
            : base(configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            _repository = repository;
            _includeMessages = includeMessages;

            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            if (!_includeMessages)
                return;

            IPipeSpecification<ConsumeContext<TMessage>> specification = new GetMessageDataTransformSpecification<TMessage>(_repository);

            configurator.AddPipeSpecification(specification);
        }

        public override void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
        {
            IPipeSpecification<ExecuteContext<TArguments>> specification = new GetMessageDataTransformSpecification<TArguments>(_repository);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
        {
            IPipeSpecification<ExecuteContext<TArguments>> specification = new GetMessageDataTransformSpecification<TArguments>(_repository);

            configurator.Arguments(x => x.AddPipeSpecification(specification));
        }

        public override void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
        {
            IPipeSpecification<CompensateContext<TLog>> specification = new GetMessageDataTransformSpecification<TLog>(_repository);

            configurator.Log(x => x.AddPipeSpecification(specification));
        }
    }
}
