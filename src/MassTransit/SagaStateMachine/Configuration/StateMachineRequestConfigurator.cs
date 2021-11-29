namespace MassTransit.Configuration
{
    using System;


    public class StateMachineRequestConfigurator<TRequest> :
        IRequestConfigurator,
        RequestSettings
        where TRequest : class
    {
        public StateMachineRequestConfigurator()
        {
            Timeout = TimeSpan.FromSeconds(30);
        }

        public RequestSettings Settings
        {
            get
            {
                if (ServiceAddress == null && EndpointConvention.TryGetDestinationAddress<TRequest>(out var serviceAddress))
                    ServiceAddress = serviceAddress;

                return this;
            }
        }

        public Uri ServiceAddress { get; set; }
        public TimeSpan Timeout { get; set; }
    }
}
