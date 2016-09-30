// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Configurators;
    using Courier;
    using Courier.Contracts;
    using Courier.Hosts;
    using GreenPipes;
    using Pipeline;


    public class ExecuteActivityHostSpecification<TActivity, TArguments> :
        IPipeConfigurator<ConsumeContext<RoutingSlip>>,
        IReceiveEndpointSpecification
        where TActivity : ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly ExecuteActivityFactory<TArguments> _activityFactory;
        readonly Func<IFilter<ConsumeContext<RoutingSlip>>> _filterFactory;
        readonly List<IPipeSpecification<ConsumeContext<RoutingSlip>>> _pipeSpecifications;

        public ExecuteActivityHostSpecification(ExecuteActivityFactory<TArguments> activityFactory)
        {
            _pipeSpecifications = new List<IPipeSpecification<ConsumeContext<RoutingSlip>>>();

            _activityFactory = activityFactory;
            _filterFactory = () => new ExecuteActivityHost<TActivity, TArguments>(_activityFactory);
        }

        public ExecuteActivityHostSpecification(ExecuteActivityFactory<TArguments> activityFactory, Uri compensateAddress)
        {
            _pipeSpecifications = new List<IPipeSpecification<ConsumeContext<RoutingSlip>>>();

            _activityFactory = activityFactory;
            _filterFactory = () => new ExecuteActivityHost<TActivity, TArguments>(_activityFactory, compensateAddress);
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext<RoutingSlip>> specification)
        {
            _pipeSpecifications.Add(specification);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_filterFactory == null)
                yield return this.Failure("FilterFactory", "must not be null");

            foreach (ValidationResult result in _pipeSpecifications.SelectMany(x => x.Validate()))
                yield return result;
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            var builders = new ConsumerPipeBuilder<RoutingSlip>();
            for (int i = 0; i < _pipeSpecifications.Count; i++)
                _pipeSpecifications[i].Apply(builders);

            IPipe<ConsumeContext<RoutingSlip>> messagePipe = Pipe.New<ConsumeContext<RoutingSlip>>(x =>
            {
                foreach (var filter in builders.Filters)
                    x.UseFilter(filter);

                x.UseFilter(_filterFactory());
            });

            builder.ConnectConsumePipe(messagePipe);
        }


        class ConsumerPipeBuilder<T> :
            IPipeBuilder<ConsumeContext<T>>
            where T : class
        {
            readonly IList<IFilter<ConsumeContext<T>>> _filters;

            public ConsumerPipeBuilder()
            {
                _filters = new List<IFilter<ConsumeContext<T>>>();
            }

            public IEnumerable<IFilter<ConsumeContext<T>>> Filters => _filters;

            void IPipeBuilder<ConsumeContext<T>>.AddFilter(IFilter<ConsumeContext<T>> filter)
            {
                _filters.Add(filter);
            }
        }
    }
}