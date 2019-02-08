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
    using Courier;


    public class ActivityDefinition<TActivity, TArguments, TLog> :
        ExecuteActivityDefinition<TActivity, TArguments>,
        IActivityDefinition<TActivity, TArguments, TLog>
        where TActivity : class, Activity<TArguments, TLog>
        where TLog : class
        where TArguments : class
    {
        string _compensateEndpointName;

        protected ActivityDefinition()
        {
        }

        /// <summary>
        /// Specify the endpoint name (which may be a queue, or a subscription, depending upon the transport) on which the saga
        /// should be configured. Setting to null will use the supplied <see cref="IEndpointNameFormatter"/> to generate the
        /// endpoint name.
        /// </summary>
        protected string CompensateEndpointName
        {
            set => _compensateEndpointName = value;
        }

        public void Configure(IReceiveEndpointConfigurator endpointConfigurator,
            ICompensateActivityConfigurator<TActivity, TLog> compensateActivityConfigurator)
        {
            ConfigureConcurrencyLimit(compensateActivityConfigurator.RoutingSlip);

            ConfigureCompensateActivity(endpointConfigurator, compensateActivityConfigurator);
        }

        public string GetCompensateEndpointName(IEndpointNameFormatter formatter)
        {
            return !string.IsNullOrWhiteSpace(_compensateEndpointName)
                ? _compensateEndpointName
                : formatter.CompensateActivity<TActivity, TLog>();
        }

        public Type LogType => typeof(TLog);

        /// <summary>
        /// Called when the compensate activity is being configured on the endpoint.
        /// </summary>
        /// <param name="endpointConfigurator">The receive endpoint configurator for the consumer</param>
        /// <param name="compensateActivityConfigurator"></param>
        protected virtual void ConfigureCompensateActivity(IReceiveEndpointConfigurator endpointConfigurator,
            ICompensateActivityConfigurator<TActivity, TLog> compensateActivityConfigurator)
        {
        }
    }
}