namespace MassTransit.Host.Activities
{
    using System;
    using Courier;
    using Hosting;


    public class ActivityService<TActivity, TArguments, TLog> :
        IBusServiceConfigurator
        where TActivity : class, IActivity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        readonly string _activityName;
        readonly ICompensateActivityFactory<TActivity, TLog> _compensateActivityFactory;
        readonly int _compensateConsumerLimit;
        readonly string _compensateQueueName;
        readonly IExecuteActivityFactory<TActivity, TArguments> _executeActivityFactory;
        readonly int _executeConsumerLimit;
        readonly string _executeQueueName;

        public ActivityService(IConfigurationProvider configuration, IActivityQueueNameProvider queueNameProvider,
            IExecuteActivityFactory<TActivity, TArguments> executeActivityFactory, ICompensateActivityFactory<TActivity, TLog> compensateActivityFactory)
        {
            _executeActivityFactory = executeActivityFactory;
            _compensateActivityFactory = compensateActivityFactory;

            _activityName = GetActivityName();

            _executeQueueName = queueNameProvider.GetExecuteActivityQueueName(_activityName);
            _executeConsumerLimit = GetExecuteConsumerLimit(configuration);

            _compensateQueueName = queueNameProvider.GetCompensateActivityQueueName(_activityName);
            _compensateConsumerLimit = GetCompensateConsumerLimit(configuration);
        }

        public virtual void Configure(IServiceConfigurator configurator)
        {
            var compensateAddress = CreateCompensateReceiveEndpoint(configurator);

            CreateExecuteReceiveEndpoint(configurator, compensateAddress);
        }

        static string GetActivityName()
        {
            var activityName = typeof(TActivity).Name;
            if (activityName.EndsWith("Service", StringComparison.OrdinalIgnoreCase))
                activityName = activityName.Substring(0, activityName.Length - "Service".Length);
            return activityName;
        }

        int GetExecuteConsumerLimit(IConfigurationProvider configurationProvider)
        {
            string settingName = $"{_activityName}ConsumerLimit";

            return configurationProvider.GetSetting(settingName, Environment.ProcessorCount);
        }

        int GetCompensateConsumerLimit(IConfigurationProvider configurationProvider)
        {
            string settingName = $"{_activityName}ConsumerLimit";

            return configurationProvider.GetSetting(settingName, Environment.ProcessorCount / 2);
        }

        protected virtual void CreateExecuteReceiveEndpoint(IServiceConfigurator configurator, Uri compensateAddress)
        {

            configurator.ReceiveEndpoint(_executeQueueName, _executeConsumerLimit, x =>
            {
                x.ExecuteActivityHost(compensateAddress, _executeActivityFactory);
            });
        }

        protected virtual Uri CreateCompensateReceiveEndpoint(IServiceConfigurator configurator)
        {

            Uri inputAddress = null;

            configurator.ReceiveEndpoint(_compensateQueueName, _compensateConsumerLimit, x =>
            {
                inputAddress = x.InputAddress;
                x.CompensateActivityHost(_compensateActivityFactory);
            });

            return inputAddress;
        }
    }
}
