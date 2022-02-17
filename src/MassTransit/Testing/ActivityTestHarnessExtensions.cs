namespace MassTransit.Testing
{
    using System;
    using Courier;


    public static class ActivityTestHarnessExtensions
    {
        /// <summary>
        /// Creates an activity test harness
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TArguments"></typeparam>
        /// <typeparam name="TLog"></typeparam>
        /// <param name="harness"></param>
        /// <returns></returns>
        public static ActivityTestHarness<TActivity, TArguments, TLog> Activity<TActivity, TArguments, TLog>(this BusTestHarness harness)
            where TActivity : class, IActivity<TArguments, TLog>, new()
            where TArguments : class
            where TLog : class
        {
            var activityFactory = new FactoryMethodActivityFactory<TActivity, TArguments, TLog>(x => new TActivity(), x => new TActivity());

            return new ActivityTestHarness<TActivity, TArguments, TLog>(harness, activityFactory, x =>
            {
            }, x =>
            {
            });
        }

        /// <summary>
        /// Creates an activity test harness
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TArguments"></typeparam>
        /// <typeparam name="TLog"></typeparam>
        /// <param name="harness"></param>
        /// <param name="executeFactoryMethod"></param>
        /// <param name="compensateFactoryMethod"></param>
        /// <returns></returns>
        public static ActivityTestHarness<TActivity, TArguments, TLog> Activity<TActivity, TArguments, TLog>(this BusTestHarness harness,
            Func<TArguments, TActivity> executeFactoryMethod, Func<TLog, TActivity> compensateFactoryMethod)
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            var activityFactory = new FactoryMethodActivityFactory<TActivity, TArguments, TLog>(executeFactoryMethod, compensateFactoryMethod);

            return new ActivityTestHarness<TActivity, TArguments, TLog>(harness, activityFactory, x =>
            {
            }, x =>
            {
            });
        }

        /// <summary>
        /// Creates an execute-only activity test harness
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TArguments"></typeparam>
        /// <param name="harness"></param>
        /// <returns></returns>
        public static ExecuteActivityTestHarness<TActivity, TArguments> ExecuteActivity<TActivity, TArguments>(this BusTestHarness harness)
            where TActivity : class, IExecuteActivity<TArguments>, new()
            where TArguments : class
        {
            var activityFactory = new FactoryMethodExecuteActivityFactory<TActivity, TArguments>(x => new TActivity());

            return new ExecuteActivityTestHarness<TActivity, TArguments>(harness, activityFactory, x =>
            {
            });
        }

        /// <summary>
        /// Creates an execute-only activity test harness
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TArguments"></typeparam>
        /// <param name="harness"></param>
        /// <param name="executeFactoryMethod"></param>
        /// <returns></returns>
        public static ExecuteActivityTestHarness<TActivity, TArguments> ExecuteActivity<TActivity, TArguments>(this BusTestHarness harness,
            Func<TArguments, TActivity> executeFactoryMethod)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var activityFactory = new FactoryMethodExecuteActivityFactory<TActivity, TArguments>(executeFactoryMethod);

            return new ExecuteActivityTestHarness<TActivity, TArguments>(harness, activityFactory, x =>
            {
            });
        }
    }
}
