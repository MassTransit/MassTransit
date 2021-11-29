namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Courier;
    using Courier.Contracts;
    using Internals;
    using Util;


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

        void IConsumerConfigurationObserver.ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
        {
        }

        void IConsumerConfigurationObserver.ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
        {
            if (typeof(TMessage).ClosesType(typeof(Batch<>), out Type[] types))
            {
                typeof(ConfigurationObserver)
                    .GetMethod(nameof(BatchConsumerConfigured))
                    .MakeGenericMethod(typeof(TConsumer), types[0])
                    .Invoke(this, new object[] { configurator });
            }
            else
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

        public virtual void BatchConsumerConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, Batch<TMessage>> configurator)
            where TConsumer : class, IConsumer<Batch<TMessage>>
            where TMessage : class
        {
        }

        void NotifyObserver<TMessage>()
            where TMessage : class
        {
            if (_messageTypes.Contains(typeof(TMessage)))
                return;

            _messageTypes.Add(typeof(TMessage));

            ForEach(observer => observer.MessageConfigured<TMessage>(_configurator));
        }
    }
}
