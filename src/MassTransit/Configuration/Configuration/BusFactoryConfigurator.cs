namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;


    public abstract class BusFactoryConfigurator :
        IConsumePipeConfigurator,
        ISendPipelineConfigurator,
        IPublishPipelineConfigurator,
        IEndpointConfigurationObserverConnector,
        IBusObserverConnector,
        IReceiveObserverConnector,
        IConsumeObserverConnector,
        ISendObserverConnector,
        IPublishObserverConnector
    {
        readonly IBusConfiguration _busConfiguration;

        protected BusFactoryConfigurator(IBusConfiguration busConfiguration)
        {
            _busConfiguration = busConfiguration;

            busConfiguration.BusEndpointConfiguration.Consume.Configurator.AutoStart = false;

            if (LogContext.Current == null)
                LogContext.ConfigureCurrentLogContext();
        }

        public IMessageTopologyConfigurator MessageTopology => _busConfiguration.Topology.Message;
        public IConsumeTopologyConfigurator ConsumeTopology => _busConfiguration.Topology.Consume;
        public ISendTopologyConfigurator SendTopology => _busConfiguration.Topology.Send;
        public IPublishTopologyConfigurator PublishTopology => _busConfiguration.Topology.Publish;

        public bool DeployTopologyOnly
        {
            set => _busConfiguration.HostConfiguration.DeployTopologyOnly = value;
        }

        public int? ConcurrentMessageLimit
        {
            set => _busConfiguration.Transport.Configurator.ConcurrentMessageLimit = value;
        }

        public int PrefetchCount
        {
            set => _busConfiguration.Transport.Configurator.PrefetchCount = value;
        }

        public ContentType DefaultContentType
        {
            set => _busConfiguration.Serialization.DefaultContentType = value;
        }

        public ContentType SerializerContentType
        {
            set => _busConfiguration.Serialization.SerializerContentType = value;
        }

        public ConnectHandle ConnectBusObserver(IBusObserver observer)
        {
            return _busConfiguration.ConnectBusObserver(observer);
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _busConfiguration.HostConfiguration.ConnectConsumeObserver(observer);
        }

        public virtual bool AutoStart
        {
            set => _busConfiguration.BusEndpointConfiguration.Consume.Configurator.AutoStart = value;
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _busConfiguration.Consume.Configurator.AddPipeSpecification(specification);
        }

        public void AddPrePipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _busConfiguration.Consume.Configurator.AddPrePipeSpecification(specification);
        }

        public void AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
            where T : class
        {
            _busConfiguration.Consume.Configurator.AddPipeSpecification(specification);
        }

        public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _busConfiguration.Consume.Configurator.ConnectConsumerConfigurationObserver(observer);
        }

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return _busConfiguration.Consume.Configurator.ConnectSagaConfigurationObserver(observer);
        }

        public ConnectHandle ConnectHandlerConfigurationObserver(IHandlerConfigurationObserver observer)
        {
            return _busConfiguration.Consume.Configurator.ConnectHandlerConfigurationObserver(observer);
        }

        public ConnectHandle ConnectActivityConfigurationObserver(IActivityConfigurationObserver observer)
        {
            return _busConfiguration.ConnectActivityConfigurationObserver(observer);
        }

        public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
            where TConsumer : class
        {
            _busConfiguration.Consume.Configurator.ConsumerConfigured(configurator);
        }

        public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
            where TConsumer : class
            where TMessage : class
        {
            _busConfiguration.Consume.Configurator.ConsumerMessageConfigured(configurator);
        }

        public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            _busConfiguration.Consume.Configurator.SagaConfigured(configurator);
        }

        public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
            where TInstance : class, ISaga, SagaStateMachineInstance
        {
            _busConfiguration.Consume.Configurator.StateMachineSagaConfigured(configurator, stateMachine);
        }

        public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
            where TSaga : class, ISaga
            where TMessage : class
        {
            _busConfiguration.Consume.Configurator.SagaMessageConfigured(configurator);
        }

        public void HandlerConfigured<TMessage>(IHandlerConfigurator<TMessage> configurator)
            where TMessage : class
        {
            _busConfiguration.Consume.Configurator.HandlerConfigured(configurator);
        }

        public void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            _busConfiguration.Consume.Configurator.ActivityConfigured(configurator, compensateAddress);
        }

        public void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            _busConfiguration.Consume.Configurator.ExecuteActivityConfigured(configurator);
        }

        public void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            _busConfiguration.Consume.Configurator.CompensateActivityConfigured(configurator);
        }

        public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            return _busConfiguration.ConnectEndpointConfigurationObserver(observer);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _busConfiguration.HostConfiguration.ConnectPublishObserver(observer);
        }

        public void ConfigurePublish(Action<IPublishPipeConfigurator> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(_busConfiguration.Publish.Configurator);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _busConfiguration.HostConfiguration.ConnectReceiveObserver(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _busConfiguration.HostConfiguration.ConnectSendObserver(observer);
        }

        public void ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(_busConfiguration.Send.Configurator);
        }

        public void Message<T>(Action<IMessageTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            IMessageTopologyConfigurator<T> configurator = _busConfiguration.Topology.Message.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public void Send<T>(Action<IMessageSendTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            IMessageSendTopologyConfigurator<T> configurator = _busConfiguration.Topology.Send.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public void Publish<T>(Action<IMessagePublishTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            IMessagePublishTopologyConfigurator<T> configurator = _busConfiguration.Topology.Publish.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return _busConfiguration.Validate()
                .Concat(_busConfiguration.HostConfiguration.Validate())
                .Concat(_busConfiguration.BusEndpointConfiguration.Validate());
        }

        public void AddSerializer(ISerializerFactory factory, bool isSerializer = true)
        {
            _busConfiguration.Serialization.AddSerializer(factory, isSerializer);
        }

        public void AddDeserializer(ISerializerFactory factory, bool isDefault = false)
        {
            _busConfiguration.Serialization.AddDeserializer(factory);
        }

        public void ClearSerialization()
        {
            _busConfiguration.Serialization.Clear();
        }
    }
}
