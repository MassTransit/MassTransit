namespace MassTransit.ConsumePipeSpecifications
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Automatonymous;
    using ConsumeConfigurators;
    using Context.Converters;
    using Courier;
    using GreenPipes;
    using GreenPipes.Builders;
    using GreenPipes.Filters;
    using Metadata;
    using Pipeline;
    using Pipeline.Pipes;
    using Saga;
    using SagaConfigurators;


    public class ConsumePipeSpecification :
        IConsumePipeConfigurator,
        IConsumePipeSpecification
    {
        readonly IList<IPipeSpecification<ConsumeContext>> _consumeContextSpecifications;
        readonly ConsumerConfigurationObservable _consumerObservers;
        readonly HandlerConfigurationObservable _handlerObservers;
        readonly object _lock = new object();
        readonly ConcurrentDictionary<Type, IMessageConsumePipeSpecification> _messageSpecifications;
        readonly ConsumePipeSpecificationObservable _observers;
        readonly SagaConfigurationObservable _sagaObservers;
        readonly IList<IPipeSpecification<ConsumeContext>> _specifications;
        readonly ActivityConfigurationObservable _activityObservers;

        public ConsumePipeSpecification()
        {
            _specifications = new List<IPipeSpecification<ConsumeContext>>();
            _consumeContextSpecifications = new List<IPipeSpecification<ConsumeContext>>();
            _messageSpecifications = new ConcurrentDictionary<Type, IMessageConsumePipeSpecification>();
            _observers = new ConsumePipeSpecificationObservable();

            _consumerObservers = new ConsumerConfigurationObservable();
            _sagaObservers = new SagaConfigurationObservable();
            _handlerObservers = new HandlerConfigurationObservable();
            _activityObservers = new ActivityConfigurationObservable();

            AutoStart = true;
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            lock (_lock)
            {
                _specifications.Add(specification);

                foreach (var messageSpecification in _messageSpecifications.Values)
                    messageSpecification.AddPipeSpecification(specification);
            }
        }

        public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _consumerObservers.Connect(observer);
        }

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return _sagaObservers.Connect(observer);
        }

        public ConnectHandle ConnectHandlerConfigurationObserver(IHandlerConfigurationObserver observer)
        {
            return _handlerObservers.Connect(observer);
        }

        public ConnectHandle ConnectActivityConfigurationObserver(IActivityConfigurationObserver observer)
        {
            return _activityObservers.Connect(observer);
        }

        public void AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
            where T : class
        {
            IMessageConsumePipeSpecification<T> messageSpecification = GetMessageSpecification<T>();

            messageSpecification.AddPipeSpecification(specification);
        }

        public void AddPrePipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _consumeContextSpecifications.Add(specification);
        }

        public bool AutoStart { get; set; }

        public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
            where TConsumer : class
        {
            _consumerObservers.ConsumerConfigured(configurator);
        }

        public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
            where TConsumer : class
            where TMessage : class
        {
            _consumerObservers.ConsumerMessageConfigured(configurator);
        }

        public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            _sagaObservers.SagaConfigured(configurator);
        }

        public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
            where TInstance : class, ISaga, SagaStateMachineInstance
        {
            _sagaObservers.StateMachineSagaConfigured(configurator, stateMachine);
        }

        public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
            where TSaga : class, ISaga
            where TMessage : class
        {
            _sagaObservers.SagaMessageConfigured(configurator);
        }

        public void HandlerConfigured<TMessage>(IHandlerConfigurator<TMessage> configurator)
            where TMessage : class
        {
            _handlerObservers.HandlerConfigured(configurator);
        }

        public void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            _activityObservers.ActivityConfigured(configurator, compensateAddress);
        }

        public void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            _activityObservers.ExecuteActivityConfigured(configurator);
        }

        public void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            _activityObservers.CompensateActivityConfigured(configurator);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _specifications.SelectMany(x => x.Validate())
                .Concat(_messageSpecifications.Values.SelectMany(x => x.Validate()));
        }

        public IMessageConsumePipeSpecification<T> GetMessageSpecification<T>()
            where T : class
        {
            var specification = _messageSpecifications.GetOrAdd(typeof(T), CreateMessageSpecification<T>);

            return specification.GetMessageSpecification<T>();
        }

        public ConnectHandle ConnectConsumePipeSpecificationObserver(IConsumePipeSpecificationObserver observer)
        {
            return _observers.Connect(observer);
        }

        public IConsumePipe BuildConsumePipe()
        {
            var filter = new DynamicFilter<ConsumeContext, Guid>(new ConsumeContextConverterFactory(), GetRequestId);

            var builder = new PipeBuilder<ConsumeContext>();
            foreach (IPipeSpecification<ConsumeContext> specification in _consumeContextSpecifications)
                specification.Apply(builder);

            builder.AddFilter(filter);

            return new ConsumePipe(filter, builder.Build(), AutoStart);
        }

        static Guid GetRequestId(ConsumeContext context)
        {
            return context.RequestId ?? Guid.Empty;
        }

        IMessageConsumePipeSpecification CreateMessageSpecification<T>(Type type)
            where T : class
        {
            var specification = new MessageConsumePipeSpecification<T>();

            foreach (IPipeSpecification<ConsumeContext> pipeSpecification in _specifications)
                specification.AddPipeSpecification(pipeSpecification);

            _observers.MessageSpecificationCreated(specification);

            var connector = new ImplementedMessageTypeConnector<T>(this, specification);

            ImplementedMessageTypeCache<T>.EnumerateImplementedTypes(connector);

            return specification;
        }


        class ImplementedMessageTypeConnector<TMessage> :
            IImplementedMessageType
            where TMessage : class
        {
            readonly MessageConsumePipeSpecification<TMessage> _messageSpecification;
            readonly IConsumePipeSpecification _specification;

            public ImplementedMessageTypeConnector(IConsumePipeSpecification specification, MessageConsumePipeSpecification<TMessage> messageSpecification)
            {
                _specification = specification;
                _messageSpecification = messageSpecification;
            }

            public void ImplementsMessageType<T>(bool direct)
                where T : class
            {
                IMessageConsumePipeSpecification<T> implementedTypeSpecification = _specification.GetMessageSpecification<T>();

                _messageSpecification.AddImplementedMessageSpecification(implementedTypeSpecification);
            }
        }
    }
}
