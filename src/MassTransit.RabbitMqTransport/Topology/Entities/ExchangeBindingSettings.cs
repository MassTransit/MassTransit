// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Topology.Entities
{
    using System.Collections.Generic;


    /// <summary>
    /// Settings for a subscription to be bound into the RabbitMQ exchanges
    /// </summary>
    public interface ExchangeBindingSettings
    {
        /// <summary>
        /// The exchange settings
        /// </summary>
        ExchangeSettings Exchange { get; }

        /// <summary>
        /// A routing key for the exchange binding
        /// </summary>
        string RoutingKey { get; }

        /// <summary>
        /// The arguments for the binding
        /// </summary>
        IDictionary<string, object> Arguments { get; }
    }
}