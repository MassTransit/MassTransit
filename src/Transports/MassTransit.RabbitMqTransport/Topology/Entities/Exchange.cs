// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    /// The exchange details used to declare the exchange to RabbitMQ
    /// </summary>
    public interface Exchange
    {
        /// <summary>
        /// The exchange name
        /// </summary>
        string ExchangeName { get; }

        /// <summary>
        /// The exchange type
        /// </summary>
        string ExchangeType { get; }

        /// <summary>
        /// True if the exchange should be durable, and survive a broker restart
        /// </summary>
        bool Durable { get; }

        /// <summary>
        /// True if the exchange should be deleted when the connection is closed
        /// </summary>
        bool AutoDelete { get; }

        /// <summary>
        /// Additional exchange arguments
        /// </summary>
        IDictionary<string, object> ExchangeArguments { get; }
    }
}