namespace MassTransit.EndpointConfigurators
{
    using System;
    using System.Net.Mime;
    using Automatonymous;
    using ConsumeConfigurators;
    using GreenPipes;
    using SagaConfigurators;
    using Transports;


    /// <summary>
    /// This is simply a delegate to the endpoint configurator for a management endpoint.
    /// A management endpoint is just a type of receive endpoint that can be used to communicate
    /// with middleware, etc.
    /// </summary>
    public class ManagementEndpointConfigurator :
        IManagementEndpointConfigurator
    {
        readonly IReceiveEndpointConfigurator _configurator;

        public ManagementEndpointConfigurator(IReceiveEndpointConfigurator configurator)
        {
            _configurator = configurator;
        }

        void IPipeConfigurator<ConsumeContext>.AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        void IConsumePipeConfigurator.AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        void IConsumePipeConfigurator.AddPrePipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _configurator.AddPrePipeSpecification(specification);
        }

        public bool AutoStart
        {
            set => _configurator.AutoStart = value;
        }

        ConnectHandle IConsumerConfigurationObserverConnector.ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _configurator.ConnectConsumerConfigurationObserver(observer);
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

        public void ClearMessageDeserializers()
        {
            _configurator.ClearMessageDeserializers();
        }

        public void AddDependency(IReceiveEndpointObserverConnector connector)
        {
            _configurator.AddDependency(connector);
        }

        Uri IReceiveEndpointConfigurator.InputAddress => _configurator.InputAddress;

        void ISendPipelineConfigurator.ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            _configurator.ConfigureSend(callback);
        }

        void IPublishPipelineConfigurator.ConfigurePublish(Action<IPublishPipeConfigurator> callback)
        {
            _configurator.ConfigurePublish(callback);
        }

        void IConsumerConfigurationObserver.ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
        {
            _configurator.ConsumerConfigured(configurator);
        }

        void IConsumerConfigurationObserver.ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
        {
            _configurator.ConsumerMessageConfigured(configurator);
        }

        ConnectHandle ISagaConfigurationObserverConnector.ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return _configurator.ConnectSagaConfigurationObserver(observer);
        }

        void ISagaConfigurationObserver.SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
        {
            _configurator.SagaConfigured(configurator);
        }

        void ISagaConfigurationObserver.StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator,
            SagaStateMachine<TInstance> stateMachine)
        {
            _configurator.StateMachineSagaConfigured(configurator, stateMachine);
        }

        void ISagaConfigurationObserver.SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
        {
            _configurator.SagaMessageConfigured(configurator);
        }

        ConnectHandle IHandlerConfigurationObserverConnector.ConnectHandlerConfigurationObserver(IHandlerConfigurationObserver observer)
        {
            return _configurator.ConnectHandlerConfigurationObserver(observer);
        }

        void IHandlerConfigurationObserver.HandlerConfigured<TMessage>(IHandlerConfigurator<TMessage> configurator)
        {
            _configurator.HandlerConfigured(configurator);
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _configurator.ConnectReceiveEndpointObserver(observer);
        }

        ConnectHandle IActivityConfigurationObserverConnector.ConnectActivityConfigurationObserver(IActivityConfigurationObserver observer)
        {
            return _configurator.ConnectActivityConfigurationObserver(observer);
        }

        void IActivityConfigurationObserver.ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
        {
            _configurator.ActivityConfigured(configurator, compensateAddress);
        }

        void IActivityConfigurationObserver.ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
        {
            _configurator.ExecuteActivityConfigured(configurator);
        }

        void IActivityConfigurationObserver.CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
        {
            _configurator.CompensateActivityConfigured(configurator);
        }
    }
}
