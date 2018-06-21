// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
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
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly ExecuteActivityFactory<TActivity, TArguments> _activityFactory;
        readonly Func<IPipe<RequestContext>, IFilter<ConsumeContext<RoutingSlip>>> _filterFactory;
        readonly List<IPipeSpecification<ExecuteActivityContext<TActivity, TArguments>>> _pipeSpecifications;
        readonly RoutingSlipConfigurator _routingSlipConfigurator;

        public ExecuteActivityHostSpecification(ExecuteActivityFactory<TActivity, TArguments> activityFactory)
        {
            _activityFactory = activityFactory;

            _pipeSpecifications = new List<IPipeSpecification<ExecuteActivityContext<TActivity, TArguments>>>();
            _routingSlipConfigurator = new RoutingSlipConfigurator();
            _filterFactory = executePipe => new ExecuteActivityHost<TActivity, TArguments>(_activityFactory, executePipe);
        }

        public ExecuteActivityHostSpecification(ExecuteActivityFactory<TActivity, TArguments> activityFactory, Uri compensateAddress)
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