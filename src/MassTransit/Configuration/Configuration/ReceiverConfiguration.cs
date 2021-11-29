namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class ReceiverConfiguration :
        EndpointConfiguration,
        IReceiveEndpointConfigurator
    {
        readonly IReceiveEndpointConfiguration _configuration;
        protected readonly IList<IReceiveEndpointSpecification> Specifications;

        protected ReceiverConfiguration(IReceiveEndpointConfiguration endpointConfiguration)
            : base(endpointConfiguration)
        {
            _configuration = endpointConfiguration;

            Specifications = new List<IReceiveEndpointSpecification>();

            this.ThrowOnSkippedMessages();
            this.RethrowFaultedMessages();
        }

        public Uri InputAddress => _configuration.InputAddress;

        public bool ConfigureConsumeTopology
        {
            set { }
        }

        public bool PublishFaults
        {
            set { }
        }

        public void AddDependency(IReceiveEndpointObserverConnector connector)
        {
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _configuration.ConnectReceiveEndpointObserver(observer);
        }

        public void ConfigureMessageTopology<T>(bool enabled = true)
            where T : class
        {
        }

        public void AddEndpointSpecification(IReceiveEndpointSpecification specification)
        {
            Specifications.Add(specification);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return Specifications.SelectMany(x => x.Validate())
                .Concat(base.Validate());
        }
    }
}
