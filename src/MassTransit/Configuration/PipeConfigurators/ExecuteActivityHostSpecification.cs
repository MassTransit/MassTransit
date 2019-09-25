namespace MassTransit.PipeConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using ConsumeConfigurators;
    using Courier;
    using Courier.Contracts;
    using Courier.Hosts;
    using Courier.Pipeline;
    using GreenPipes;


    public class ExecuteActivityHostSpecification<TActivity, TArguments> :
        IExecuteActivityConfigurator<TActivity, TArguments>,
        IReceiveEndpointSpecification
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IExecuteActivityFactory<TActivity, TArguments> _activityFactory;
        readonly Uri _compensateAddress;
        readonly Func<IPipe<RequestContext>, IFilter<ConsumeContext<RoutingSlip>>> _filterFactory;
        readonly List<IPipeSpecification<ExecuteActivityContext<TActivity, TArguments>>> _pipeSpecifications;
        readonly RoutingSlipConfigurator _routingSlipConfigurator;
        readonly ActivityConfigurationObservable _observers;

        public ExecuteActivityHostSpecification(IExecuteActivityFactory<TActivity, TArguments> activityFactory, IActivityConfigurationObserver observer)
        {
            _activityFactory = activityFactory;

            _pipeSpecifications = new List<IPipeSpecification<ExecuteActivityContext<TActivity, TArguments>>>();
            _routingSlipConfigurator = new RoutingSlipConfigurator();
            _filterFactory = executePipe => new ExecuteActivityHost<TActivity, TArguments>(_activityFactory, executePipe);
            _observers = new ActivityConfigurationObservable();

            _observers.Connect(observer);
        }

        public ExecuteActivityHostSpecification(IExecuteActivityFactory<TActivity, TArguments> activityFactory, Uri compensateAddress,
            IActivityConfigurationObserver observer)
            : this(activityFactory, observer)
        {
            _compensateAddress = compensateAddress;

            _filterFactory = executePipe => new ExecuteActivityHost<TActivity, TArguments>(_activityFactory, executePipe, compensateAddress);
        }

        public void AddPipeSpecification(IPipeSpecification<ExecuteActivityContext<TActivity, TArguments>> specification)
        {
            _pipeSpecifications.Add(specification);
        }

        public void Arguments(Action<IExecuteActivityArgumentsConfigurator<TArguments>> configure)
        {
            var configurator = new ExecuteActivityArgumentsConfigurator<TActivity, TArguments>(this);

            configure?.Invoke(configurator);
        }

        public void RoutingSlip(Action<IRoutingSlipConfigurator> configure)
        {
            configure?.Invoke(_routingSlipConfigurator);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            _observers.All(observer =>
            {
                if (_compensateAddress == null)
                    observer.ExecuteActivityConfigured(this);
                else
                    observer.ActivityConfigured(this, _compensateAddress);

                return true;
            });

            if (_filterFactory == null)
                yield return this.Failure("FilterFactory", "must not be null");

            foreach (var result in _pipeSpecifications.SelectMany(x => x.Validate()))
                yield return result;

            foreach (var result in _routingSlipConfigurator.Validate())
                yield return result;
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            IPipe<RequestContext> executeActivityPipe = _pipeSpecifications.Build(new ExecuteActivityFilter<TActivity, TArguments>());

            _routingSlipConfigurator.UseFilter(_filterFactory(executeActivityPipe));

            builder.ConnectConsumePipe(_routingSlipConfigurator.Build());
        }

        public ConnectHandle ConnectActivityConfigurationObserver(IActivityConfigurationObserver observer)
        {
            return _observers.Connect(observer);
        }
    }
}
