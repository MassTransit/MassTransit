namespace MassTransit.Conductor.Configuration
{
    using System;
    using ConsumeConfigurators;
    using JobService;
    using MassTransit.Definition;


    public class ServiceInstanceOptions
    {
        readonly OptionsSet _optionsSet;

        public ServiceInstanceOptions()
        {
            EndpointNameFormatter = DefaultEndpointNameFormatter.Instance;
            _optionsSet = new OptionsSet();
        }

        public bool InstanceEndpointEnabled { get; private set; }
        public bool InstanceServiceEndpointEnabled { get; private set; }
        public IEndpointNameFormatter EndpointNameFormatter { get; private set; }

        /// <summary>
        /// Create a single instance-specific control endpoint. By default, each service endpoint has
        /// a separate Control endpoint (SubmitOrderControl, or submit-order-control in kebab) that is
        /// shared by all service endpoint instances. A single control endpoint (Instance_xyz) can be
        /// created instead used by all service endpoints in an instance. Can reduce queue sprawl when
        /// only a few instances are used with many service endpoints.
        /// </summary>
        public ServiceInstanceOptions EnableInstanceEndpoint()
        {
            InstanceEndpointEnabled = true;

            return this;
        }

        /// <summary>
        /// Enable the job service endpoints, so that <see cref="IJobConsumer{TJob}"/> consumers
        /// can be configured.
        /// </summary>
        public ServiceInstanceOptions EnableJobServiceEndpoints()
        {
            _optionsSet.Configure<JobServiceOptions>();

            return this;
        }

        /// <summary>
        /// Create instance-specific service endpoints, so every service endpoint instance will have
        /// a queue for each instance (10 instances, 2 receive endpoints = 22 queues).
        /// </summary>
        public ServiceInstanceOptions EnableInstanceServiceEndpoint()
        {
            InstanceServiceEndpointEnabled = true;

            return this;
        }

        public ServiceInstanceOptions SetEndpointNameFormatter(IEndpointNameFormatter endpointNameFormatter)
        {
            EndpointNameFormatter = endpointNameFormatter;

            return this;
        }

        /// <summary>
        /// Configure options on the service instance, which may be used to configure conductor capabilities
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ServiceInstanceOptions ConfigureOptions<T>(Action<T> configure = null)
            where T : IOptions, new()
        {
            _optionsSet.Configure(configure);

            return this;
        }

        /// <summary>
        /// Configure options on the service instance, which may be used to configure conductor capabilities
        /// </summary>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Options<T>(Action<T> configure = null)
            where T : IOptions, new()
        {
            return _optionsSet.Configure(configure);
        }

        public bool TryGetOptions<T>(out T options)
            where T : IOptions
        {
            return _optionsSet.TryGetOptions(out options);
        }
    }
}
