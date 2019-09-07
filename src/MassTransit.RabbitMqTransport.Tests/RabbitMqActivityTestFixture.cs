namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Collections.Generic;
    using Courier;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public abstract class RabbitMqActivityTestFixture :
        RabbitMqTestFixture
    {
        protected RabbitMqActivityTestFixture()
        {
            ActivityTestContexts = new Dictionary<Type, ActivityTestContext>();

            RabbitMqTestHarness.PreCreateBus += PreCreateBus;
        }

        protected IDictionary<Type, ActivityTestContext> ActivityTestContexts { get; private set; }

        void PreCreateBus(BusTestHarness harness)
        {
            SetupActivities(harness);
        }

        class BusFactoryConfigurator :
            ActivityTestContextConfigurator
        {
            readonly IRabbitMqBusFactoryConfigurator _configurator;
            readonly IRabbitMqHost _host;

            public BusFactoryConfigurator(IRabbitMqHost host, IRabbitMqBusFactoryConfigurator configurator)
            {
                _host = host;
                _configurator = configurator;
            }

            public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configure)
            {
                _configurator.ReceiveEndpoint(_host, queueName, x =>
                {
                    x.PrefetchCount = 1;
                    x.PurgeOnStartup = true;
                    configure(x);
                });
            }
        }


        protected void AddActivityContext<T, TArguments, TLog>(Func<T> activityFactory,
            Action<IExecuteActivityConfigurator<T, TArguments>> configureExecute = null,
            Action<ICompensateActivityConfigurator<T, TLog>> configureCompensate = null)
            where TArguments : class
            where TLog : class
            where T : class, IActivity<TArguments, TLog>
        {
            var context = new ActivityTestContext<T, TArguments, TLog>(BusTestHarness, activityFactory, configureExecute, configureCompensate);

            ActivityTestContexts.Add(typeof(T), context);
        }

        protected void AddActivityContext<T, TArguments>(Func<T> activityFactory, Action<IExecuteActivityConfigurator<T, TArguments>> configure = null)
            where TArguments : class
            where T : class, IExecuteActivity<TArguments>
        {
            var context = new ActivityTestContext<T, TArguments>(BusTestHarness, activityFactory, configure);

            ActivityTestContexts.Add(typeof(T), context);
        }

        protected ActivityTestContext GetActivityContext<T>()
        {
            return ActivityTestContexts[typeof(T)];
        }

        protected virtual void SetupActivities(BusTestHarness testHarness)
        {
        }
    }
}
