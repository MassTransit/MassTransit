namespace MassTransit.PipeConfigurators
{
    using System;
    using System.Collections.Generic;
    using Automatonymous;
    using ConsumeConfigurators;
    using Courier;
    using Courier.Contracts;
    using GreenPipes.Util;
    using Saga;
    using SagaConfigurators;


    /// <summary>
    /// Combines the separate configuration observers into a single observer that is for each message type, called once, to configure each
    /// message pipeline only once. Only outputs the individual message events for configuring the pipeline.
    /// </summary>
    public class ConfigurationObserver :
        Connectable<IMessageConfigurationObserver>,
        IConsumerConfigurationObserver,
        ISagaConfigurationObserver,
        IHandlerConfigurationObserver,
        IActivityConfigurationObserver
    {
        readonly IConsumePipeConfigurator _configurator;
        readonly HashSet<Type> _messageTypes;

        protected ConfigurationObserver(IConsumePipeConfigurator configurator)
        {
            _configurator = configurator;

            _messageTypes = new HashSet<Type>();

            configurator.ConnectConsumerConfigurationObserver(this);
            configurator.ConnectSagaConfigurationObserver(this);
            configurator.ConnectHandlerConfigurationObserver(this);
            configurator.ConnectActivityConfigurationObserver(this);
        }

        void IConsumerConfigurationObserver.ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
        {
        }

        void IConsumerConfigurationObserver.ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
        {
            NotifyObserver<TMessage>();
        }

        void IHandlerConfigurationObserver.HandlerConfigured<TMessage>(IHandlerConfigurator<TMessage> configurator)
        {
            NotifyObserver<TMessage>();
        }

        void ISagaConfigurationObserver.SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
        {
        }

        public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
            where TInstance : class, ISaga, SagaStateMachineInstance
        {
        }

        void ISagaConfigurationObserver.SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
        {
            NotifyObserver<TMessage>();
        }

        public virtual void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator,
            Uri compensateAddress)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            NotifyObserver<RoutingSlip>();
        }

        public virtual void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            NotifyObserver<RoutingSlip>();
        }

        public virtual void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            NotifyObserver<RoutingSlip>();
        }

        void NotifyObserver<TMessage>()
            where TMessage : class
        {
            if (_messageTypes.Contains(typeof(TMessage)))
                return;

            _messageTypes.Add(typeof(TMessage));

            All(observer =>
            {
                observer.MessageConfigured<TMessage>(_configurator);

                return true;
            });
        }
    }
}
