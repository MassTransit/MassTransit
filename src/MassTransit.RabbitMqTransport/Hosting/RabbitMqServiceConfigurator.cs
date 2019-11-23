namespace MassTransit.RabbitMqTransport.Hosting
{
    using System;
    using System.Net.Mime;
    using Automatonymous;
    using ConsumeConfigurators;
    using EndpointConfigurators;
    using GreenPipes;
    using MassTransit.Builders;
    using MassTransit.Hosting;
    using MassTransit.Topology;
    using Saga;
    using SagaConfigurators;


    /// <summary>
    /// A hosted service can specify receive endpoints using the service configurator
    /// </summary>
    public class RabbitMqServiceConfigurator :
        IServiceConfigurator
    {
        readonly IRabbitMqBusFactoryConfigurator _configurator;
        readonly int _defaultConsumerLimit;

        public RabbitMqServiceConfigurator(IRabbitMqBusFactoryConfigurator configurator)
        {
            _configurator = configurator;
            _defaultConsumerLimit = Environment.ProcessorCount * 4;
        }

        public bool AutoStart
        {
            set => _configurator.AutoStart = value;
        }

        public void ReceiveEndpoint(string queueName, int consumerLimit, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            _configurator.ReceiveEndpoint(queueName, x =>
            {
                x.PrefetchCount = (ushort)consumerLimit;

                configureEndpoint(x);
            });
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        public void AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
            where T : class
        {
            _configurator.AddPipeSpecification(specification);
        }

        public void AddPrePipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _configurator.AddPrePipeSpecification(specification);
        }

        public ConnectHandle ConnectBusObserver(IBusObserver observer)
        {
            return _configurator.ConnectBusObserver(observer);
        }

        public IMessageTopologyConfigurator MessageTopology => _configurator.MessageTopology;

        public ISendTopologyConfigurator SendTopology => _configurator.SendTopology;

        public IPublishTopologyConfigurator PublishTopology => _configurator.PublishTopology;

        public void AddBusFactorySpecification(IBusFactorySpecification specification)
        {
            _configurator.AddBusFactorySpecification(specification);
        }

        public void Message<T>(Action<IMessageTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            _configurator.Message(configureTopology);
        }

        public void Send<T>(Action<IMessageSendTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            ((IBusFactoryConfigurator)_configurator).Send(configureTopology);
        }

        public void Publish<T>(Action<IMessagePublishTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            ((IBusFactoryConfigurator)_configurator).Publish(configureTopology);
        }

        public void SetMessageSerializer(SerializerFactory serializerFactory)
        {
            _configurator.SetMessageSerializer(serializerFactory);
        }

        public void AddMessageDeserializer(ContentType contentType, DeserializerFactory deserializerFactory)
        {
            _configurator.AddMessageDeserializer(contentType, deserializerFactory);
        }

        public void ClearMessageDeserializers()
        {
            _configurator.ClearMessageDeserializers();
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            _configurator.ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            ReceiveEndpoint(queueName, _defaultConsumerLimit, configureEndpoint);
        }

        ConnectHandle IConsumerConfigurationObserverConnector.ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _configurator.ConnectConsumerConfigurationObserver(observer);
        }

        public void ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            _configurator.ConfigureSend(callback);
        }

        public void ConfigurePublish(Action<IPublishPipeConfigurator> callback)
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

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return _configurator.ConnectSagaConfigurationObserver(observer);
        }

        public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            _configurator.SagaConfigured(configurator);
        }

        void ISagaConfigurationObserver.StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
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

        ConnectHandle IEndpointConfigurationObserverConnector.ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            return _configurator.ConnectEndpointConfigurationObserver(observer);
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
