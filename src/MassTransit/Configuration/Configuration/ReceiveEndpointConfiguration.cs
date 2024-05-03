namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Observables;
    using Transports;


    public abstract class ReceiveEndpointConfiguration :
        EndpointConfiguration,
        IReceiveEndpointConfiguration
    {
        readonly Lazy<IConsumePipe> _consumePipe;
        readonly HashSet<IReceiveEndpointDependency> _dependencies;
        readonly HashSet<IReceiveEndpointDependent> _dependents;
        readonly List<string> _lateConfigurationKeys;
        readonly List<IReceiveEndpointSpecification> _specifications;
        IReceiveEndpoint _receiveEndpoint;

        protected ReceiveEndpointConfiguration(IHostConfiguration hostConfiguration, IEndpointConfiguration endpointConfiguration)
            : base(endpointConfiguration)
        {
            ConfigureConsumeTopology = true;
            PublishFaults = true;

            _consumePipe = new Lazy<IConsumePipe>(() => Consume.Specification.BuildConsumePipe());
            _specifications = new List<IReceiveEndpointSpecification>();
            _lateConfigurationKeys = new List<string>();
            _dependencies = new HashSet<IReceiveEndpointDependency>();
            _dependents = new HashSet<IReceiveEndpointDependent>();

            EndpointObservers = new ReceiveEndpointObservable();
            ReceiveObservers = new ReceiveObservable();
            TransportObservers = new ReceiveTransportObservable();

            ConnectConsumerConfigurationObserver(hostConfiguration.BusConfiguration);
            ConnectSagaConfigurationObserver(hostConfiguration.BusConfiguration);
            ConnectHandlerConfigurationObserver(hostConfiguration.BusConfiguration);
            ConnectActivityConfigurationObserver(hostConfiguration.BusConfiguration);
        }

        public ReceiveEndpointObservable EndpointObservers { get; }
        public ReceiveObservable ReceiveObservers { get; }
        public ReceiveTransportObservable TransportObservers { get; }

        public bool ConfigureConsumeTopology { get; set; }
        public bool PublishFaults { get; set; }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return EndpointObservers.Connect(observer);
        }

        public void AddDependent(IReceiveEndpointDependent dependent)
        {
            _dependents.Add(dependent);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in _specifications.SelectMany(x => x.Validate()))
                yield return result;

            foreach (var result in _lateConfigurationKeys.Select(x => this.Failure(x, "was modified after being used")))
                yield return result;

            foreach (var result in base.Validate())
                yield return result;
        }

        public IConsumePipe ConsumePipe => _consumePipe.Value;

        public abstract Uri HostAddress { get; }
        public abstract Uri InputAddress { get; }

        public virtual IReceiveEndpoint ReceiveEndpoint
        {
            get
            {
                if (_receiveEndpoint == null)
                    throw new InvalidOperationException("The receive endpoint has not been built.");

                return _receiveEndpoint;
            }

            protected set => _receiveEndpoint = value;
        }

        public virtual IReceivePipe CreateReceivePipe()
        {
            return Receive.CreatePipe(CreateConsumePipe(), Serialization.CreateSerializerCollection());
        }

        public abstract ReceiveEndpointContext CreateReceiveEndpointContext();

        public Task DependenciesReady => Task.WhenAll(_dependencies.Select(x => x.Ready));

        public Task DependentsCompleted => Task.WhenAll(_dependents.Select(x => x.Completed));

        public void ConfigureMessageTopology<T>(bool enabled = true)
            where T : class
        {
            Topology.Consume.GetMessageTopology<T>().ConfigureConsumeTopology = enabled;
        }

        public void ConfigureMessageTopology(Type messageType, bool enabled = true)
        {
            Topology.Consume.GetMessageTopology(messageType).ConfigureConsumeTopology = enabled;
        }

        public void AddDependency(IReceiveEndpointDependency dependency)
        {
            _dependencies.Add(dependency);
        }

        protected void ApplySpecifications(IReceiveEndpointBuilder builder)
        {
            for (var i = 0; i < _specifications.Count; i++)
                _specifications[i].Configure(builder);
        }

        public void AddEndpointSpecification(IReceiveEndpointSpecification specification)
        {
            _specifications.Add(specification);
        }

        protected virtual IConsumePipe CreateConsumePipe()
        {
            return _consumePipe.Value;
        }

        protected void Changed(string key)
        {
            if (IsAlreadyConfigured())
                _lateConfigurationKeys.Add(key);
        }

        protected virtual bool IsAlreadyConfigured()
        {
            return false;
        }
    }
}
