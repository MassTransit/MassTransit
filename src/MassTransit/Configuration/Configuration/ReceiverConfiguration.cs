namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Transports;


    public class ReceiverConfiguration :
        EndpointConfiguration,
        IReceiveEndpointConfigurator
    {
        readonly IReceiveEndpointConfiguration _configuration;
        protected readonly List<IReceiveEndpointSpecification> Specifications;

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

        public void AddDependency(IReceiveEndpointDependency dependent)
        {
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _configuration.ConnectReceiveEndpointObserver(observer);
        }

        public void AddDependent(IReceiveEndpointDependent dependent)
        {
        }

        public void ConfigureMessageTopology<T>(bool enabled = true)
            where T : class
        {
        }

        public void ConfigureMessageTopology(Type messageType, bool enabled = true)
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
