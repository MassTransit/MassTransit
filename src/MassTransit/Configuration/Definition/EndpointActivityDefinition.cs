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


    public class EndpointActivityDefinition<TActivity, TArguments, TLog> :
        EndpointExecuteActivityDefinition<TActivity, TArguments>,
        IActivityDefinition<TActivity, TArguments, TLog>
        where TActivity : class, Activity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        readonly IEndpointDefinition<CompensateActivity<TLog>> _compensateEndpointDefinition;

        public EndpointActivityDefinition(IEndpointDefinition<ExecuteActivity<TArguments>> endpointDefinition,
            IEndpointDefinition<CompensateActivity<TLog>> compensateEndpointDefinition)
            : base(endpointDefinition)
        {
            _compensateEndpointDefinition = compensateEndpointDefinition;
        }

        public void Configure(IReceiveEndpointConfigurator endpointConfigurator,
            ICompensateActivityConfigurator<TActivity, TLog> compensateActivityConfigurator)
        {
        }

        public Type LogType => typeof(TLog);

        public string GetCompensateEndpointName(IEndpointNameFormatter formatter)
        {
            return _compensateEndpointDefinition.GetEndpointName(formatter);
        }
    }
}