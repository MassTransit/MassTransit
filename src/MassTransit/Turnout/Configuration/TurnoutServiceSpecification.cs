namespace MassTransit.Turnout.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mime;
    using Automatonymous;
    using ConsumeConfigurators;
    using Courier;
    using GreenPipes;
    using Saga;
    using SagaConfigurators;
    using Transports;


    public class TurnoutServiceSpecification<TCommand> :
        ITurnoutServiceConfigurator<TCommand>,
        ISpecification
        where TCommand : class
    {
        readonly IReceiveEndpointConfigurator _configurator;
        readonly IJobRegistry _jobRegistry;
        readonly Lazy<IJobService> _jobService;

        public TurnoutServiceSpecification(IReceiveEndpointConfigurator configurator)
        {
            _configurator = configurator;

            SuperviseInterval = TimeSpan.FromMinutes(1);
            _jobRegistry = new JobRegistry();
            PartitionCount = 8;

            _jobService = new Lazy<IJobService>(CreateJobService);
        }

        public IJobRegistry JobRegistry => _jobRegistry;

        public Uri ManagementAddress { private get; set; }

        public IJobService Service => _jobService.Value;

        public bool AutoStart
        {
            set => _configurator.AutoStart = value;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (JobFactory == null)
                yield return this.Failure("JobFactory", "must be specified");

            if (ManagementAddress == null)
                yield return this.Failure("ControlAddress", "must be a valid address");

            if (SuperviseInterval < TimeSpan.FromSeconds(1))
                yield return this.Failure("SuperviseInterval", "must be >= 1 second");

            if (PartitionCount < 1)
                yield return this.Failure("PartitionCount", "must be > 0");
        }

        public TimeSpan SuperviseInterval { private get; set; }

        public IJobFactory<TCommand> JobFactory { get; set; }

        void IPipeConfigurator<ConsumeContext>.AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        void IConsumePipeConfigurator.AddPipeSpecification<T1>(IPipeSpecification<ConsumeContext<T1>> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        void IConsumePipeConfigurator.AddPrePipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _configurator.AddPrePipeSpecification(specification);
        }

        public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _configurator.ConnectConsumerConfigurationObserver(observer);
        }

        public bool ConfigureConsumeTopology
        {
            set => _configurator.ConfigureConsumeTopology = value;
        }

        void IReceiveEndpointConfigurator.AddEndpointSpecification(IReceiveEndpointSpecification configurator)
        {
            _configurator.AddEndpointSpecification(configurator);
        }

        void IReceiveEndpointConfigurator.SetMessageSerializer(SerializerFactory serializerFactory)
        {
            _configurator.SetMessageSerializer(serializerFactory);
        }

        void IReceiveEndpointConfigurator.AddMessageDeserializer(ContentType contentType, DeserializerFactory deserializerFactory)
        {
            _configurator.AddMessageDeserializer(contentType, deserializerFactory);
        }

        void IReceiveEndpointConfigurator.ClearMessageDeserializers()
        {
            _configurator.ClearMessageDeserializers();
        }

        public void AddDependency(IReceiveEndpointObserverConnector connector)
        {
            _configurator.AddDependency(connector);
        }

        Uri IReceiveEndpointConfigurator.InputAddress => _configurator.InputAddress;

        public int PartitionCount { get; set; }

        void ISendPipelineConfigurator.ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            _configurator.ConfigureSend(callback);
        }

        void IPublishPipelineConfigurator.ConfigurePublish(Action<IPublishPipeConfigurator> callback)
        {
            _configurator.ConfigurePublish(callback);
        }

        IJobService CreateJobService()
        {
            return new JobService(_jobRegistry, _configurator.InputAddress, ManagementAddress, SuperviseInterval);
        }

        public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
            where TConsumer : class
        {
            _configurator.ConsumerConfigured(configurator);
        }

        public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
            where TConsumer : class
            where TMessage : class
        {
            _configurator.ConsumerMessageConfigured(configurator);
        }

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return _configurator.ConnectSagaConfigurationObserver(observer);
        }

        public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            _configurator.SagaConfigured(configurator);
        }

        public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
            where TInstance : class, ISaga, SagaStateMachineInstance
        {
            _configurator.StateMachineSagaConfigured(configurator, stateMachine);
        }

        public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
            where TSaga : class, ISaga
            where TMessage : class
        {
            _configurator.SagaMessageConfigured(configurator);
        }

        public ConnectHandle ConnectHandlerConfigurationObserver(IHandlerConfigurationObserver observer)
        {
            return _configurator.ConnectHandlerConfigurationObserver(observer);
        }

        public void HandlerConfigured<TMessage>(IHandlerConfigurator<TMessage> configurator)
            where TMessage : class
        {
            _configurator.HandlerConfigured(configurator);
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _configurator.ConnectReceiveEndpointObserver(observer);
        }

        public ConnectHandle ConnectActivityConfigurationObserver(IActivityConfigurationObserver observer)
        {
            return _configurator.ConnectActivityConfigurationObserver(observer);
        }

        public void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator,
            Uri compensateAddress)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            _configurator.ActivityConfigured(configurator, compensateAddress);
        }

        public void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            _configurator.ExecuteActivityConfigured(configurator);
        }

        public void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            _configurator.CompensateActivityConfigured(configurator);
        }

        void IReceivePipelineConfigurator.ConfigureReceive(Action<IReceivePipeConfigurator> callback)
        {
            _configurator.ConfigureReceive(callback);
        }

        void IReceivePipelineConfigurator.ConfigureDeadLetter(Action<IPipeConfigurator<ReceiveContext>> callback)
        {
            _configurator.ConfigureDeadLetter(callback);
        }

        void IReceivePipelineConfigurator.ConfigureError(Action<IPipeConfigurator<ExceptionReceiveContext>> callback)
        {
            _configurator.ConfigureError(callback);
        }
    }
}
