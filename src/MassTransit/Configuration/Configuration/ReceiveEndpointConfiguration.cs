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
        readonly IList<string> _lateConfigurationKeys;
        readonly IList<IReceiveEndpointSpecification> _specifications;
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

        public void AddDependent(IReceiveEndpointObserverConnector dependency)
        {
            var dependant = new ReceiveEndpointDependent(dependency);

            _dependents.Add(dependant);
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

        public void AddDependency(IReceiveEndpointDependentConnector connector)
        {
            var dependency = new ReceiveEndpointDependency(connector);
            connector.AddDependent(this);

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


        class ReceiveEndpointDependency :
            IReceiveEndpointDependency,
            IReceiveEndpointObserver
        {
            readonly ConnectHandle _handle;
            readonly TaskCompletionSource<ReceiveEndpointReady> _ready;

            public ReceiveEndpointDependency(IReceiveEndpointObserverConnector connector)
            {
                _ready = new TaskCompletionSource<ReceiveEndpointReady>(TaskCreationOptions.RunContinuationsAsynchronously);

                _handle = connector.ConnectReceiveEndpointObserver(this);
            }

            public Task Ready => _ready.Task;

            Task IReceiveEndpointObserver.Ready(ReceiveEndpointReady ready)
            {
                _handle.Disconnect();

                _ready.TrySetResult(ready);

                return Task.CompletedTask;
            }

            Task IReceiveEndpointObserver.Stopping(ReceiveEndpointStopping stopping)
            {
                return Task.CompletedTask;
            }

            Task IReceiveEndpointObserver.Completed(ReceiveEndpointCompleted completed)
            {
                return Task.CompletedTask;
            }

            Task IReceiveEndpointObserver.Faulted(ReceiveEndpointFaulted faulted)
            {
                return Task.CompletedTask;
            }
        }


        class ReceiveEndpointDependent :
            IReceiveEndpointDependent,
            IReceiveEndpointObserver
        {
            readonly TaskCompletionSource<ReceiveEndpointCompleted> _completed;
            readonly ConnectHandle _handle;

            public ReceiveEndpointDependent(IReceiveEndpointObserverConnector connector)
            {
                _completed = new TaskCompletionSource<ReceiveEndpointCompleted>(TaskCreationOptions.RunContinuationsAsynchronously);

                _handle = connector.ConnectReceiveEndpointObserver(this);
            }

            public Task Completed => _completed.Task;

            Task IReceiveEndpointObserver.Ready(ReceiveEndpointReady ready)
            {
                return Task.CompletedTask;
            }

            Task IReceiveEndpointObserver.Stopping(ReceiveEndpointStopping stopping)
            {
                return Task.CompletedTask;
            }

            Task IReceiveEndpointObserver.Completed(ReceiveEndpointCompleted completed)
            {
                _handle.Disconnect();

                _completed.TrySetResult(completed);

                return Task.CompletedTask;
            }

            Task IReceiveEndpointObserver.Faulted(ReceiveEndpointFaulted faulted)
            {
                return Task.CompletedTask;
            }
        }
    }
}
