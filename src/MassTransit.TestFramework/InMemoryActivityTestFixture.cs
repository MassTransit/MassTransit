namespace MassTransit.TestFramework
{
    using System;
    using System.Collections.Generic;
    using Testing;


    public abstract class InMemoryActivityTestFixture :
        InMemoryTestFixture
    {
        protected InMemoryActivityTestFixture()
        {
            ActivityTestContexts = new Dictionary<Type, ActivityTestContext>();

            InMemoryTestHarness.PreCreateBus += PreCreateBus;
        }

        protected IDictionary<Type, ActivityTestContext> ActivityTestContexts { get; private set; }

        void PreCreateBus(BusTestHarness harness)
        {
            SetupActivities(harness);
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

        protected ActivityTestContext AddActivityContext<T, TArguments>(Func<T> activityFactory,
            Action<IExecuteActivityConfigurator<T, TArguments>> configureExecute = null)
            where TArguments : class
            where T : class, IExecuteActivity<TArguments>
        {
            var context = new ActivityTestContext<T, TArguments>(BusTestHarness, activityFactory, configureExecute);

            ActivityTestContexts.Add(typeof(T), context);

            return context;
        }

        protected ActivityTestContext GetActivityContext<T>()
        {
            return ActivityTestContexts[typeof(T)];
        }

        protected abstract void SetupActivities(BusTestHarness testHarness);
    }
}
