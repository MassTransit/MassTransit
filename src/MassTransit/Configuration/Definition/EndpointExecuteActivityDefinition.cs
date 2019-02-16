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


    public class EndpointExecuteActivityDefinition<TActivity, TArguments> :
        IExecuteActivityDefinition<TActivity, TArguments>
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IEndpointDefinition<ExecuteActivity<TArguments>> _endpointDefinition;

        public EndpointExecuteActivityDefinition(IEndpointDefinition<ExecuteActivity<TArguments>> endpointDefinition)
        {
            _endpointDefinition = endpointDefinition;
        }

        public void Configure(IReceiveEndpointConfigurator endpointConfigurator, IExecuteActivityConfigurator<TActivity, TArguments> consumerConfigurator)
        {
        }

        public Type ActivityType => typeof(TActivity);
        public Type ArgumentType => typeof(TArguments);

        public string GetExecuteEndpointName(IEndpointNameFormatter formatter)
        {
            return _endpointDefinition.GetEndpointName(formatter);
        }
    }
}
