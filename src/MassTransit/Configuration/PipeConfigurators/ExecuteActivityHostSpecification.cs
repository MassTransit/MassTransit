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
        readonly Func<IPipe<RequestContext>, IFilter<ConsumeContext<RoutingSlip>>> _filterFactory;
        readonly List<IPipeSpecification<ExecuteActivityContext<TActivity, TArguments>>> _pipeSpecifications;
        readonly RoutingSlipConfigurator _routingSlipConfigurator;

        public ExecuteActivityHostSpecification(IExecuteActivityFactory<TActivity, TArguments> activityFactory)
        {
            _activityFactory = activityFactory;

            _pipeSpecifications = new List<IPipeSpecification<ExecuteActivityContext<TActivity, TArguments>>>();
            _routingSlipConfigurator = new RoutingSlipConfigurator();
            _filterFactory = executePipe => new ExecuteActivityHost<TActivity, TArguments>(_activityFactory, executePipe);
        }

        public ExecuteActivityHostSpecification(IExecuteActivityFactory<TActivity, TArguments> activityFactory, Uri compensateAddress)
        {
            _activityFactory = activityFactory;

            _pipeSpecifications = new List<IPipeSpecification<ExecuteActivityContext<TActivity, TArguments>>>();
            _routingSlipConfigurator = new RoutingSlipConfigurator();
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
    }
}
