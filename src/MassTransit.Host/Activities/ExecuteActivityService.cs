namespace MassTransit.Host.Activities
{
    using System;
    using Courier;
    using Hosting;
    using Topshelf;


    /// <summary>
    /// For an activity that has no compensation, only create the execute portion of the activity
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TArguments"></typeparam>
    public class ExecuteActivityService<TActivity, TArguments> :
        ServiceControl,
        IDisposable
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly string _activityName;
        readonly IExecuteActivityFactory<TActivity,TArguments> _executeActivityFactory;
        readonly int _executeConsumerLimit;
        readonly string _executeQueueName;
        readonly IServiceConfigurator _serviceFactory;
        bool _disposed;

        public ExecuteActivityService(IConfigurationProvider configuration, IServiceConfigurator serviceFactory,
            IActivityQueueNameProvider activityUriProvider, IExecuteActivityFactory<TActivity, TArguments> executeActivityFactory)
        {

            _serviceFactory = serviceFactory;
            _executeActivityFactory = executeActivityFactory;

            _activityName = GetActivityName();

            _executeQueueName = activityUriProvider.GetExecuteActivityQueueName(_activityName);
            _executeConsumerLimit = GetExecuteConsumerLimit(configuration);
        }

        public virtual void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
        }

        public virtual bool Start(HostControl hostControl)
        {
            return true;
        }

        public virtual bool Stop(HostControl hostControl)
        {
            return true;
        }

        string GetActivityName()
        {
            string activityName = typeof(TActivity).Name;
            if (activityName.EndsWith("Service", StringComparison.OrdinalIgnoreCase))
                activityName = activityName.Substring(0, activityName.Length - "Service".Length);
            return activityName;
        }

        int GetExecuteConsumerLimit(IConfigurationProvider configurationProvider)
        {
            string settingName = $"{_activityName}ConsumerLimit";

            return configurationProvider.GetSetting(settingName, Environment.ProcessorCount);
        }

        protected virtual void CreateExecuteServiceBus()
        {
            _serviceFactory.ReceiveEndpoint(_executeQueueName, _executeConsumerLimit, x =>
            {
                x.ExecuteActivityHost<TActivity, TArguments>(_executeActivityFactory);
            });
        }
    }
}
