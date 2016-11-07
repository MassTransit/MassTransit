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


    public class CompensateActivityHostSpecification<TActivity, TLog> :
        ICompensateActivityConfigurator<TActivity, TLog>,
        IReceiveEndpointSpecification
        where TActivity : class, CompensateActivity<TLog>
        where TLog : class
    {
        readonly CompensateActivityFactory<TActivity, TLog> _activityFactory;
        readonly Func<IPipe<CompensateActivityContext<TActivity, TLog>>, IFilter<ConsumeContext<RoutingSlip>>> _filterFactory;
        readonly List<IPipeSpecification<CompensateActivityContext<TActivity, TLog>>> _pipeSpecifications;
        readonly RoutingSlipConfigurator _routingSlipConfigurator;

        public CompensateActivityHostSpecification(CompensateActivityFactory<TActivity, TLog> activityFactory)
        {
            _activityFactory = activityFactory;

            _pipeSpecifications = new List<IPipeSpecification<CompensateActivityContext<TActivity, TLog>>>();
            _routingSlipConfigurator = new RoutingSlipConfigurator();
            _filterFactory = compensatePipe => new CompensateActivityHost<TActivity, TLog>(_activityFactory, compensatePipe);
        }

        public void AddPipeSpecification(IPipeSpecification<CompensateActivityContext<TActivity, TLog>> specification)
        {
            _pipeSpecifications.Add(specification);
        }

        public void Log(Action<ICompensateActivityLogConfigurator<TLog>> configure)
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
            if (_filterFactory == null)
                yield return this.Failure("FilterFactory", "must not be null");

            foreach (var result in _pipeSpecifications.SelectMany(x => x.Validate()))
                yield return result;
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            IPipe<CompensateActivityContext<TActivity, TLog>> compensateActivityPipe =
                _pipeSpecifications.Build(new CompensateActivityFilter<TActivity, TLog>());

            IPipe<ConsumeContext<RoutingSlip>> messagePipe = Pipe.New<ConsumeContext<RoutingSlip>>(x =>
            {
                x.UseFilter(_filterFactory(compensateActivityPipe));
            });

            builder.ConnectConsumePipe(messagePipe);
        }
    }
}