namespace Automatonymous.SagaConfigurators
{
    using System;
    using MassTransit;


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

                if (ServiceAddress == null)
                    throw new AutomatonymousException("The ServiceAddress was not specified.");

                return this;
            }
        }

        public Uri ServiceAddress { get; set; }
        public TimeSpan Timeout { get; set; }
    }
}
