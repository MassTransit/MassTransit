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
    using ConsumeConfigurators;
    using Courier;
    using GreenPipes;


    public class ExecuteActivityDefinition<TActivity, TArguments> :
        IExecuteActivityDefinition<TActivity, TArguments>
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        int? _concurrencyLimit;
        string _executeEndpointName;

        protected ExecuteActivityDefinition()
        {
        }

        /// <summary>
        /// Specify the endpoint name (which may be a queue, or a subscription, depending upon the transport) on which the saga
        /// should be configured. Setting to null will use the supplied <see cref="IEndpointNameFormatter"/> to generate the
        /// endpoint name.
        /// </summary>
        protected string ExecuteEndpointName
        {
            set => _executeEndpointName = value;
        }

        /// <summary>
        /// Specify a concurrency limit, which is applied to the entire consumer, saga, or activity, regardless of message type.
        /// </summary>
        protected int ConcurrencyLimit
        {
            set => _concurrencyLimit = value;
        }

        public void Configure(IReceiveEndpointConfigurator endpointConfigurator,
            IExecuteActivityConfigurator<TActivity, TArguments> executeActivityConfigurator)
        {
            ConfigureConcurrencyLimit(executeActivityConfigurator.RoutingSlip);

            ConfigureExecuteActivity(endpointConfigurator, executeActivityConfigurator);
        }

        public string GetExecuteEndpointName(IEndpointNameFormatter formatter)
        {
            return !string.IsNullOrWhiteSpace(_executeEndpointName)
                ? _executeEndpointName
                : formatter.ExecuteActivity<TActivity, TArguments>();
        }

        public Type ActivityType => typeof(TActivity);
        public Type ArgumentType => typeof(TArguments);

        protected void ConfigureConcurrencyLimit(Action<Action<IRoutingSlipConfigurator>> callback)
        {
            if (_concurrencyLimit.HasValue)
                callback(x => x.UseConcurrencyLimit(_concurrencyLimit.Value));
        }

        /// <summary>
        /// Called when the compensate activity is being configured on the endpoint.
        /// </summary>
        /// <param name="endpointConfigurator">The receive endpoint configurator for the consumer</param>
        /// <param name="executeActivityConfigurator"></param>
        protected virtual void ConfigureExecuteActivity(IReceiveEndpointConfigurator endpointConfigurator,
            IExecuteActivityConfigurator<TActivity, TArguments> executeActivityConfigurator)
        {
        }
    }
}
