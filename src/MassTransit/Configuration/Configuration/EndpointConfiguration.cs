namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using Courier;


    public class EndpointConfiguration :
        IEndpointConfiguration
    {
        protected EndpointConfiguration(ITopologyConfiguration topology)
        {
            Topology = topology;

            Consume = new ConsumePipeConfiguration(topology.Consume);
            Send = new SendPipeConfiguration(topology.Send);
            Publish = new PublishPipeConfiguration(topology.Publish);
            Receive = new ReceivePipeConfiguration();

            Serialization = new SerializationConfiguration();
            Transport = new TransportConfiguration();
        }

        protected EndpointConfiguration(IEndpointConfiguration parentConfiguration, ITopologyConfiguration topology)
        {
            Topology = topology;

            Consume = new ConsumePipeConfiguration(parentConfiguration.Consume.Specification);
            Send = new SendPipeConfiguration(parentConfiguration.Send.Specification);
            Publish = new PublishPipeConfiguration(parentConfiguration.Publish.Specification);
            Receive = new ReceivePipeConfiguration();

            Serialization = parentConfiguration.Serialization.CreateSerializationConfiguration();

            Transport = new TransportConfiguration(parentConfiguration.Transport);
        }

        protected EndpointConfiguration(IEndpointConfiguration endpointConfiguration)
        {
            Topology = endpointConfiguration.Topology;

            Consume = endpointConfiguration.Consume;
            Send = endpointConfiguration.Send;
            Publish = endpointConfiguration.Publish;
            Receive = endpointConfiguration.Receive;

            Serialization = endpointConfiguration.Serialization;

            Transport = endpointConfiguration.Transport;
        }

        public int? ConcurrentMessageLimit
        {
            get => Transport.ConcurrentMessageLimit;
            set => Transport.Configurator.ConcurrentMessageLimit = value;
        }

        public int PrefetchCount
        {
            get => Transport.PrefetchCount;
            set => Transport.Configurator.PrefetchCount = value;
        }

        public ContentType DefaultContentType
        {
            set => Serialization.DefaultContentType = value;
        }

        public ContentType SerializerContentType
        {
            set => Serialization.SerializerContentType = value;
        }

        public bool AutoStart
        {
            set => Consume.Configurator.AutoStart = value;
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            Consume.Configurator.AddPipeSpecification(specification);
        }

        public void AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
            where T : class
        {
            Consume.Configurator.AddPipeSpecification(specification);
        }

        public void AddPrePipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            Consume.Configurator.AddPrePipeSpecification(specification);
        }

        public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return Consume.Configurator.ConnectConsumerConfigurationObserver(observer);
        }

        void IConsumerConfigurationObserver.ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
        {
            Consume.Configurator.ConsumerConfigured(configurator);
        }

        void IConsumerConfigurationObserver.ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
        {
            Consume.Configurator.ConsumerMessageConfigured(configurator);
        }

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return Consume.Configurator.ConnectSagaConfigurationObserver(observer);
        }

        public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            Consume.Configurator.SagaConfigured(configurator);
        }

        public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
            where TInstance : class, ISaga, SagaStateMachineInstance
        {
            Consume.Configurator.StateMachineSagaConfigured(configurator, stateMachine);
        }

        public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
            where TSaga : class, ISaga
            where TMessage : class
        {
            Consume.Configurator.SagaMessageConfigured(configurator);
        }

        public ConnectHandle ConnectHandlerConfigurationObserver(IHandlerConfigurationObserver observer)
        {
            return Consume.Configurator.ConnectHandlerConfigurationObserver(observer);
        }

        public void HandlerConfigured<TMessage>(IHandlerConfigurator<TMessage> configurator)
            where TMessage : class
        {
            Consume.Configurator.HandlerConfigured(configurator);
        }

        public ConnectHandle ConnectActivityConfigurationObserver(IActivityConfigurationObserver observer)
        {
            return Consume.Configurator.ConnectActivityConfigurationObserver(observer);
        }

        public void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator,
            Uri compensateAddress)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            Consume.Configurator.ActivityConfigured(configurator, compensateAddress);
        }

        public void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            Consume.Configurator.ExecuteActivityConfigured(configurator);
        }

        public void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            Consume.Configurator.CompensateActivityConfigured(configurator);
        }

        public void ConfigurePublish(Action<IPublishPipeConfigurator> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(Publish.Configurator);
        }

        public void ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(Send.Configurator);
        }

        public void ConfigureReceive(Action<IReceivePipeConfigurator> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(Receive.Configurator);
        }

        public void ConfigureDeadLetter(Action<IPipeConfigurator<ReceiveContext>> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(Receive.DeadLetterConfigurator);
        }

        public void ConfigureError(Action<IPipeConfigurator<ExceptionReceiveContext>> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(Receive.ErrorConfigurator);
        }

        public void ConfigureTransport(Action<ITransportConfigurator> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(Transport.Configurator);
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return Send.Specification.Validate()
                .Concat(Publish.Specification.Validate())
                .Concat(Consume.Specification.Validate())
                .Concat(Receive.Specification.Validate())
                .Concat(Topology.Validate())
                .Concat(Serialization.Validate())
                .Concat(Transport.Validate());
        }

        public IConsumePipeConfiguration Consume { get; }
        public ISendPipeConfiguration Send { get; }
        public IPublishPipeConfiguration Publish { get; }
        public IReceivePipeConfiguration Receive { get; }
        public ITopologyConfiguration Topology { get; }
        public ISerializationConfiguration Serialization { get; }
        public ITransportConfiguration Transport { get; }

        public void AddSerializer(ISerializerFactory factory, bool isSerializer = true)
        {
            Serialization.AddSerializer(factory, isSerializer);
        }

        public void AddDeserializer(ISerializerFactory factory, bool isDefault = false)
        {
            Serialization.AddDeserializer(factory, isDefault);
        }

        public void ClearSerialization()
        {
            Serialization.Clear();
        }
    }
}
