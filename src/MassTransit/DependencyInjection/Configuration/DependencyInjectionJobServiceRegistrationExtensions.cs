namespace MassTransit.Configuration
{
    using DependencyInjection.Registration;
    using JobService;
    using Microsoft.Extensions.DependencyInjection;


    public static class DependencyInjectionJobServiceRegistrationExtensions
    {
        public static IJobServiceRegistration RegisterJobService(this IServiceCollection collection, IContainerRegistrar registrar)
        {
            collection.AddOptions<JobConsumerOptions>();
            return registrar.GetOrAddRegistration<IJobServiceRegistration>(typeof(JobService), _ => new JobServiceRegistration());
        }
    }
}
