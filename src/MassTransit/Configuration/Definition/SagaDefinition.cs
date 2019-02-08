// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Definition
{
    using System;
    using GreenPipes;
    using GreenPipes.Filters;
    using Saga;


    public class SagaDefinition<TSaga> :
        ISagaDefinition<TSaga>
        where TSaga : class, ISaga
    {
        readonly Lazy<IFilter<SagaConsumeContext<TSaga>>> _filter;
        int? _concurrencyLimit;
        string _endpointName;

        protected SagaDefinition()
        {
            _filter = new Lazy<IFilter<SagaConsumeContext<TSaga>>>(() =>
                new ConcurrencyLimitFilter<SagaConsumeContext<TSaga>>(_concurrencyLimit.Value));
        }

        /// <summary>
        /// Specify the endpoint name (which may be a queue, or a subscription, depending upon the transport) on which the saga
        /// should be configured.
        /// </summary>
        protected string EndpointName
        {
            set => _endpointName = value;
        }

        protected int ConcurrencyLimit
        {
            set => _concurrencyLimit = value;
        }

        public void Configure(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<TSaga> sagaConfigurator)
        {
            if (_concurrencyLimit.HasValue)
                sagaConfigurator.UseFilter(_filter.Value);

            ConfigureSaga(endpointConfigurator, sagaConfigurator);
        }

        public Type SagaType => typeof(TSaga);

        public string GetEndpointName(IEndpointNameFormatter formatter)
        {
            return !string.IsNullOrWhiteSpace(_endpointName)
                ? _endpointName
                : formatter.Saga<TSaga>();
        }

        /// <summary>
        /// Called when the saga is being configured on the endpoint. Configuration only applies to this saga, and does not apply to
        /// the endpoint.
        /// </summary>
        /// <param name="endpointConfigurator">The receive endpoint configurator for the consumer</param>
        /// <param name="sagaConfigurator">The saga configurator</param>
        protected virtual void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<TSaga> sagaConfigurator)
        {
        }
    }
}
