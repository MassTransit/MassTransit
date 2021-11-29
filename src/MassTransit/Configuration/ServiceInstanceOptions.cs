namespace MassTransit
{
    using Configuration;


    public class ServiceInstanceOptions :
        OptionsSet
    {
        public ServiceInstanceOptions()
        {
            EndpointNameFormatter = DefaultEndpointNameFormatter.Instance;
        }

        public IEndpointNameFormatter EndpointNameFormatter { get; private set; }

        /// <summary>
        /// Enable the job service endpoints, so that <see cref="IJobConsumer{TJob}" /> consumers
        /// can be configured.
        /// </summary>
        public ServiceInstanceOptions EnableJobServiceEndpoints()
        {
            Options<JobServiceOptions>();

            return this;
        }

        public ServiceInstanceOptions SetEndpointNameFormatter(IEndpointNameFormatter endpointNameFormatter)
        {
            EndpointNameFormatter = endpointNameFormatter;

            return this;
        }
    }
}
