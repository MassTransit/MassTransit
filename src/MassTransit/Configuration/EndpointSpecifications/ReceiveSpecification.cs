namespace MassTransit.EndpointSpecifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using Automatonymous;
    using Configuration;
    using ConsumeConfigurators;
    using GreenPipes;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using Saga;
    using SagaConfigurators;


    public abstract class ReceiveSpecification :
        IConsumePipeConfigurator,
        ISendPipelineConfigurator,
        IPublishPipelineConfigurator
    {
        protected readonly IEndpointConfiguration Configuration;
        protected readonly IBuildPipeConfigurator<ReceiveContext> ReceivePipeConfigurator;
        protected readonly IList<IReceiveEndpointSpecification> Specifications;

        protected ReceiveSpecification(IEndpointConfiguration configuration)
        {
            Configuration = configuration;

            Specifications = new List<IReceiveEndpointSpecification>();
            ReceivePipeConfigurator = new PipeConfigurator<ReceiveContext>();
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            Configuration.Consume.Configurator.AddPipeSpecification(specification);
        }

        public void AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
            where T : class
        {
            Configuration.Consume.Configurator.AddPipeSpecification(specification);
        }

        public void AddPrePipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            Configuration.Consume.Configurator.AddPrePipeSpecification(specification);
        }

        public bool AutoStart
        {
            set => Configuration.AutoStart = value;
        }

        public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return Configuration.Consume.Configurator.ConnectConsumerConfigurationObserver(observer);
        }

        void IConsumerConfigurationObserver.ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
        {
            Configuration.Consume.Configurator.ConsumerConfigured(configurator);
        }

        void IConsumerConfigurationObserver.ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
        {
            Configuration.Consume.Configurator.ConsumerMessageConfigured(configurator);
        }

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return Configuration.Consume.Configurator.ConnectSagaConfigurationObserver(observer);
        }

        public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            Configuration.Consume.Configurator.SagaConfigured(configurator);
        }

        public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
            where TInstance : class, ISaga, SagaStateMachineInstance
        {
            Configuration.Consume.Configurator.StateMachineSagaConfigured(configurator, stateMachine);
        }

        public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
            where TSaga : class, ISaga
            where TMessage : class
        {
            Configuration.Consume.Configurator.SagaMessageConfigured(configurator);
        }

        public ConnectHandle ConnectHandlerConfigurationObserver(IHandlerConfigurationObserver observer)
        {
            return Configuration.Consume.Configurator.ConnectHandlerConfigurationObserver(observer);
        }

        public void HandlerConfigured<TMessage>(IHandlerConfigurator<TMessage> configurator)
            where TMessage : class
        {
            Configuration.Consume.Configurator.HandlerConfigured(configurator);
        }

        ConnectHandle IActivityConfigurationObserverConnector.ConnectActivityConfigurationObserver(IActivityConfigurationObserver observer)
        {
            return Configuration.Consume.Configurator.ConnectActivityConfigurationObserver(observer);
        }

        void IActivityConfigurationObserver.ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator,
            Uri compensateAddress)
        {
            Configuration.Consume.Configurator.ActivityConfigured(configurator, compensateAddress);
        }

        void IActivityConfigurationObserver.ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
        {
            Configuration.Consume.Configurator.ExecuteActivityConfigured(configurator);
        }

        void IActivityConfigurationObserver.CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
        {
            Configuration.Consume.Configurator.CompensateActivityConfigured(configurator);
        }

        public void ConfigurePublish(Action<IPublishPipeConfigurator> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(Configuration.Publish.Configurator);
        }

        public void ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(Configuration.Send.Configurator);
        }

        public void AddEndpointSpecification(IReceiveEndpointSpecification specification)
        {
            Specifications.Add(specification);
        }

        public void SetMessageSerializer(SerializerFactory serializerFactory)
        {
            Configuration.Serialization.SetSerializer(serializerFactory);
        }

        public void AddMessageDeserializer(ContentType contentType, DeserializerFactory deserializerFactory)
        {
            Configuration.Serialization.AddDeserializer(contentType, deserializerFactory);
        }

        public void ClearMessageDeserializers()
        {
            Configuration.Serialization.ClearDeserializers();
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return Specifications.SelectMany(x => x.Validate())
                .Concat(Configuration.Validate());
        }

    }
}
