namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Conductor.Configuration;
    using Turnout;
    using Turnout.Configuration;


    public static class TurnoutConfigurationExtensions
    {
        /// <summary>
        /// Configures Turnout on the service instance, which supports executing long-running jobs without blocking the consumer pipeline.
        /// Turnout uses multiple state machines to track jobs, each of which runs on its own dedicated receive endpoint. Multiple service
        /// instances will use the competing consumer pattern, so a shared saga repository should be configured.
        /// </summary>
        /// <typeparam name="T">The transport receive endpoint configurator type</typeparam>
        /// <param name="configurator">The Conductor service instance</param>
        /// <param name="configure"></param>
        public static IServiceInstanceConfigurator<T> Turnout<T>(this IServiceInstanceConfigurator<T> configurator, Action<ITurnoutConfigurator<T>> configure)
            where T : IReceiveEndpointConfigurator
        {
            var turnoutConfigurator = new TurnoutConfigurator<T>(configurator);

            configure?.Invoke(turnoutConfigurator);

            return configurator;
        }

        /// <summary>
        /// The job factory is used to execute the job, returning an awaitable <see cref="Task"/>.
        /// Configures the job factory using the specified delegate.
        /// </summary>
        /// <typeparam name="T">The job type</typeparam>
        /// <param name="configurator">The turnout configurator</param>
        /// <param name="method">A function that returns a Task for the job</param>
        public static void SetJobFactory<T>(this ITurnoutJobConfigurator<T> configurator, Func<JobContext<T>, Task> method)
            where T : class
        {
            configurator.JobFactory = new AsyncMethodJobFactory<T>(method);
        }
    }
}
