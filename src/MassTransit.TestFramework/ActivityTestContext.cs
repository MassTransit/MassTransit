namespace MassTransit.TestFramework
{
    using System;
    using MassTransit.Courier;
    using Testing;


    public interface ActivityTestContext
    {
        string Name { get; }

        Uri ExecuteUri { get; }
        event Action<IReceiveEndpointConfigurator> OnConfigureExecuteReceiveEndpoint;
        event Action<IReceiveEndpointConfigurator> OnConfigureCompensateReceiveEndpoint;
    }


    public class ActivityTestContext<TActivity, TArguments, TLog> :
        ActivityTestContext
        where TActivity : class, IActivity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        readonly ActivityTestHarness<TActivity, TArguments, TLog> _harness;

        public ActivityTestContext(BusTestHarness testHarness, Func<TActivity> activityFactory,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate)
        {
            var factory = new FactoryMethodActivityFactory<TActivity, TArguments, TLog>(_ => activityFactory(), _ => activityFactory());

            _harness = new ActivityTestHarness<TActivity, TArguments, TLog>(testHarness, factory, configureExecute, configureCompensate);
        }

        public Uri ExecuteUri => _harness.ExecuteAddress;

        public event Action<IReceiveEndpointConfigurator> OnConfigureExecuteReceiveEndpoint
        {
            add => _harness.OnConfigureExecuteReceiveEndpoint += value;
            remove => _harness.OnConfigureExecuteReceiveEndpoint -= value;
        }

        public event Action<IReceiveEndpointConfigurator> OnConfigureCompensateReceiveEndpoint
        {
            add => _harness.OnConfigureCompensateReceiveEndpoint += value;
            remove => _harness.OnConfigureCompensateReceiveEndpoint -= value;
        }

        public string Name => _harness.Name;
    }


    public class ActivityTestContext<TActivity, TArguments> :
        ActivityTestContext
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly ExecuteActivityTestHarness<TActivity, TArguments> _harness;

        public ActivityTestContext(BusTestHarness testHarness, Func<TActivity> activityFactory,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute)
        {
            var factory = new FactoryMethodExecuteActivityFactory<TActivity, TArguments>(_ => activityFactory());

            _harness = new ExecuteActivityTestHarness<TActivity, TArguments>(testHarness, factory, configureExecute);
        }

        public event Action<IReceiveEndpointConfigurator> OnConfigureExecuteReceiveEndpoint
        {
            add => _harness.OnConfigureExecuteReceiveEndpoint += value;
            remove => _harness.OnConfigureExecuteReceiveEndpoint -= value;
        }

        public event Action<IReceiveEndpointConfigurator> OnConfigureCompensateReceiveEndpoint
        {
            add { }
            remove { }
        }

        public Uri ExecuteUri => _harness.ExecuteAddress;
        public string Name => _harness.Name;
    }
}
