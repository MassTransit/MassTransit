namespace DefaultNamespace
{
    using MassTransit;
    using MassTransit.Hosting;

    /// <summary>
    /// Configures the bus settings for the service and all endpoints in the same assembly.
    /// </summary>
    public class ServiceSpecification :
        IServiceSpecification
    {
        public void Configure(IServiceConfigurator configurator)
        {
            // configuration for the entire bus can be specified here
            // such as configurator.UseRetry(Retry.Interval(5, TimeSpan.FromMilliseconds(100)));
        }
    }
}
