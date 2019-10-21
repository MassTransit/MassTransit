namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Turnout;
    using Turnout.Configuration;
    using Turnout.Contracts;


    public static class TurnoutConfigurationExtensions
    {
        public static void ConfigureTurnoutEndpoints<T>(this IReceiveEndpointConfigurator configurator, IBusFactoryConfigurator busFactoryConfigurator,
            IReceiveEndpointConfigurator turnoutEndpointConfigurator, IReceiveEndpointConfigurator expiredEndpointConfigurator,
            Action<ITurnoutServiceConfigurator<T>> configure)
            where T : class
        {
            configurator.AddDependency(turnoutEndpointConfigurator);
            turnoutEndpointConfigurator.AddDependency(expiredEndpointConfigurator);

            var specification = new TurnoutServiceSpecification<T>(configurator);

            configure(specification);

            specification.ManagementAddress = turnoutEndpointConfigurator.InputAddress;

            busFactoryConfigurator.AddBusFactorySpecification(specification);

            var partitioner = busFactoryConfigurator.CreatePartitioner(specification.PartitionCount);

            expiredEndpointConfigurator.Consumer(() => new JobCustodian<T>(specification.JobRegistry), x =>
            {
                x.Message<SuperviseJob<T>>(m => m.UsePartitioner(partitioner, p => p.Message.JobId));
            });

            turnoutEndpointConfigurator.Consumer(() => new JobSupervisor<T>(specification.Service, specification.JobRegistry), x =>
            {
                x.Message<CancelJob>(m => m.UsePartitioner(partitioner, p => p.Message.JobId));
                x.Message<SuperviseJob<T>>(m => m.UsePartitioner(partitioner, p => p.Message.JobId));
            });

            IJobFactory<T> jobFactory = specification.JobFactory;

            configurator.Consumer(() => new JobProducer<T>(specification.Service, jobFactory));

            busFactoryConfigurator.BusObserver(() => new JobServiceBusObserver(specification.Service));
        }

        /// <summary>
        /// Sets the job factory to the specified delegate
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The turnout configurator</param>
        /// <param name="jobFactory">A function that returns a Task for the job</param>
        public static void SetJobFactory<T>(this ITurnoutServiceConfigurator<T> configurator, Func<JobContext<T>, Task> jobFactory)
            where T : class
        {
            configurator.JobFactory = new DelegateJobFactory<T>(jobFactory);
        }
    }
}
