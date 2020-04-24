namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Context;
    using GreenPipes;
    using Transports;


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

            if (LogContext.Current == null)
                LogContext.ConfigureCurrentLogContext();

            this.ThrowOnSkippedMessages();
            this.RethrowFaultedMessages();
        }

        public Uri InputAddress => _configuration.InputAddress;

        public bool ConfigureConsumeTopology
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
