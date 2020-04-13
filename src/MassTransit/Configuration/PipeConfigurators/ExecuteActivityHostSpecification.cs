namespace MassTransit.PipeConfigurators
{
    using System;
    using System.Collections.Generic;
    using ConsumeConfigurators;
    using Courier;
    using Courier.Hosts;
    using Courier.Pipeline;
    using GreenPipes;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;


    public class ExecuteActivityHostSpecification<TActivity, TArguments> :
        IExecuteActivityConfigurator<TActivity, TArguments>,
        IReceiveEndpointSpecification
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IExecuteActivityFactory<TActivity, TArguments> _activityFactory;
        readonly Uri _compensateAddress;
        readonly IBuildPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> _activityPipeConfigurator;
        readonly IBuildPipeConfigurator<ExecuteContext<TArguments>> _executePipeConfigurator;
        readonly RoutingSlipConfigurator _routingSlipConfigurator;
        readonly ActivityConfigurationObservable _configurationObservers;
        readonly ActivityObservable _observers;

        public ExecuteActivityHostSpecification(IExecuteActivityFactory<TActivity, TArguments> activityFactory, IActivityConfigurationObserver observer)
        {
            _activityFactory = activityFactory;

            _activityPipeConfigurator = new PipeConfigurator<ExecuteActivityContext<TActivity, TArguments>>();
            _executePipeConfigurator = new PipeConfigurator<ExecuteContext<TArguments>>();
            _routingSlipConfigurator = new RoutingSlipConfigurator();
            _observers = new ActivityObservable();

            _configurationObservers = new ActivityConfigurationObservable();
            _configurationObservers.Connect(observer);
        }

        public ExecuteActivityHostSpecification(IExecuteActivityFactory<TActivity, TArguments> activityFactory, Uri compensateAddress,
            IActivityConfigurationObserver observer)
            : this(activityFactory, observer)
        {
            _compensateAddress = compensateAddress;
        }

        public void AddPipeSpecification(IPipeSpecification<ExecuteActivityContext<TActivity, TArguments>> specification)
        {
            _activityPipeConfigurator.AddPipeSpecification(specification);
        }

        public void Arguments(Action<IExecuteArgumentsConfigurator<TArguments>> configure)
        {
            var configurator = new ExecuteArgumentsConfigurator<TArguments>(_executePipeConfigurator);

            configure?.Invoke(configurator);
        }

        public void ActivityArguments(Action<IExecuteActivityArgumentsConfigurator<TArguments>> configure)
        {
            var configurator = new ExecuteActivityArgumentsConfigurator<TActivity, TArguments>(_activityPipeConfigurator);

            configure?.Invoke(configurator);
        }

        public void RoutingSlip(Action<IRoutingSlipConfigurator> configure)
        {
            configure?.Invoke(_routingSlipConfigurator);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in _routingSlipConfigurator.Validate())
                yield return result;
            foreach (var result in _activityPipeConfigurator.Validate())
                yield return result;

            _configurationObservers.All(observer =>
            {
                if (_compensateAddress == null)
                    observer.ExecuteActivityConfigured(this);
                else
                    observer.ActivityConfigured(this, _compensateAddress);

                return true;
            });
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            _activityPipeConfigurator.UseFilter(new ExecuteActivityFilter<TActivity, TArguments>(_observers));

            IPipe<ExecuteActivityContext<TActivity, TArguments>> executeActivityPipe = _activityPipeConfigurator.Build();

            _executePipeConfigurator.UseFilter(new ExecuteActivityFactoryFilter<TActivity, TArguments>(_activityFactory, executeActivityPipe));

            var executePipe = _executePipeConfigurator.Build();

            var host = new ExecuteActivityHost<TActivity, TArguments>(executePipe, _compensateAddress);
            _routingSlipConfigurator.UseFilter(host);

            builder.ConnectConsumePipe(_routingSlipConfigurator.Build());
        }

        public ConnectHandle ConnectActivityConfigurationObserver(IActivityConfigurationObserver observer)
        {
            return _configurationObservers.Connect(observer);
        }

        public ConnectHandle ConnectActivityObserver(IActivityObserver observer)
        {
            return _observers.Connect(observer);
        }
    }
}
