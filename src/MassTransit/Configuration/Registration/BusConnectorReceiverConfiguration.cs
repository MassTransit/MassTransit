namespace MassTransit.Registration
{
    using Configuration;


    public class BusConnectorReceiverConfiguration :
        ReceiverConfiguration
    {
        public BusConnectorReceiverConfiguration(IReceiveEndpointConfiguration endpointConfiguration)
            : base(endpointConfiguration)
        {
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            foreach (var specification in Specifications)
                specification.Configure(builder);
        }
    }
}
