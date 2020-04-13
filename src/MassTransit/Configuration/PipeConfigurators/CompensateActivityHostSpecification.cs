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


    public class CompensateActivityHostSpecification<TActivity, TLog> :
        ICompensateActivityConfigurator<TActivity, TLog>,
        IReceiveEndpointSpecification
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly ICompensateActivityFactory<TActivity, TLog> _activityFactory;
        readonly IBuildPipeConfigurator<CompensateActivityContext<TActivity, TLog>> _activityPipeConfigurator;
        readonly IBuildPipeConfigurator<CompensateContext<TLog>> _compensatePipeConfigurator;
        readonly RoutingSlipConfigurator _routingSlipConfigurator;
        readonly ActivityConfigurationObservable _configurationObservers;
        readonly ActivityObservable _observers;

        public CompensateActivityHostSpecification(ICompensateActivityFactory<TActivity, TLog> activityFactory, IActivityConfigurationObserver observer)
        {
            _activityFactory = activityFactory;

            _activityPipeConfigurator = new PipeConfigurator<CompensateActivityContext<TActivity, TLog>>();
            _compensatePipeConfigurator = new PipeConfigurator<CompensateContext<TLog>>();
            _routingSlipConfigurator = new RoutingSlipConfigurator();
            _observers = new ActivityObservable();

            _configurationObservers = new ActivityConfigurationObservable();
            _configurationObservers.Connect(observer);
        }

        public void AddPipeSpecification(IPipeSpecification<CompensateActivityContext<TActivity, TLog>> specification)
        {
            _activityPipeConfigurator.AddPipeSpecification(specification);
        }

        public void Log(Action<ICompensateLogConfigurator<TLog>> configure)
        {
            var configurator = new CompensateLogConfigurator<TLog>(_compensatePipeConfigurator);

            configure?.Invoke(configurator);
        }

        public void ActivityLog(Action<ICompensateActivityLogConfigurator<TLog>> configure)
        {
            var configurator = new CompensateActivityLogConfigurator<TActivity, TLog>(this);

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
                observer.CompensateActivityConfigured(this);
                return true;
            });
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            _activityPipeConfigurator.UseFilter(new CompensateActivityFilter<TActivity, TLog>(_observers));

            IPipe<CompensateActivityContext<TActivity, TLog>> compensateActivityPipe = _activityPipeConfigurator.Build();

            _compensatePipeConfigurator.UseFilter(new CompensateActivityFactoryFilter<TActivity, TLog>(_activityFactory, compensateActivityPipe));

            var compensatePipe = _compensatePipeConfigurator.Build();

            var host = new CompensateActivityHost<TActivity, TLog>(compensatePipe);
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
