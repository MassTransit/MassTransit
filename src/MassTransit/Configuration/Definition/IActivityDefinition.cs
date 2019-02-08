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


    public interface IActivityDefinition
    {
        /// <summary>
        /// The Activity type
        /// </summary>
        Type ActivityType { get; }

        /// <summary>
        /// The argument type
        /// </summary>
        Type ArgumentType { get; }

        /// <summary>
        /// The log type
        /// </summary>
        Type LogType { get; }

        /// <summary>
        /// Return the endpoint name for the execute activity
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns></returns>
        string GetExecuteEndpointName(IEndpointNameFormatter formatter);

        /// <summary>
        /// Return the endpoint name for the compensate activity
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns></returns>
        string GetCompensateEndpointName(IEndpointNameFormatter formatter);
    }


    public interface IActivityDefinition<TActivity, TArguments, TLog> :
        IActivityDefinition
        where TActivity : class, Activity<TArguments, TLog>
        where TLog : class
        where TArguments : class
    {
        /// <summary>
        /// Configure the execute activity
        /// </summary>
        /// <param name="endpointConfigurator"></param>
        /// <param name="executeActivityConfigurator"></param>
        void Configure(IReceiveEndpointConfigurator endpointConfigurator, IExecuteActivityConfigurator<TActivity, TArguments> executeActivityConfigurator);

        /// <summary>
        /// Configure the compensate activity
        /// </summary>
        /// <param name="endpointConfigurator"></param>
        /// <param name="compensateActivityConfigurator"></param>
        void Configure(IReceiveEndpointConfigurator endpointConfigurator, ICompensateActivityConfigurator<TActivity, TLog> compensateActivityConfigurator);
    }
}
