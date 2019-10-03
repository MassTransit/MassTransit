namespace MassTransit.Testing
{
    using System;
    using Courier;


    public class ExecuteActivityTestHarness<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IExecuteActivityFactory<TActivity, TArguments> _activityFactory;
        readonly Action<IExecuteActivityConfigurator<TActivity, TArguments>> _configureExecute;

        public ExecuteActivityTestHarness(BusTestHarness testHarness, IExecuteActivityFactory<TActivity, TArguments> activityFactory,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute)
        {
            _configureExecute = configureExecute;
            _activityFactory = activityFactory;

            Name = GetActivityName();

            ExecuteQueueName = BuildQueueName("execute");

            testHarness.OnConfigureBus += ConfigureBus;
        }

        public string ExecuteQueueName { get; private set; }
        public string Name { get; private set; }
        public Uri ExecuteAddress { get; private set; }

        public event Action<IReceiveEndpointConfigurator> OnConfigureExecuteReceiveEndpoint;

        void ConfigureBus(IBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint(ExecuteQueueName, x =>
            {
                OnConfigureExecuteReceiveEndpoint?.Invoke(x);

                x.ExecuteActivityHost(_activityFactory, _configureExecute);

                ExecuteAddress = x.InputAddress;
            });
        }

        static string GetActivityName()
        {
            var name = typeof(TActivity).Name;
            if (name.EndsWith("Activity"))
                name = name.Substring(0, name.Length - "Activity".Length);
            return name;
        }

        string BuildQueueName(string prefix)
        {
            return $"{prefix}_{typeof(TActivity).Name.ToLowerInvariant()}";
        }
    }
}
