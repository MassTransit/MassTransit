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
namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using System.Collections.Generic;


    public interface SendSettings :
        EntitySettings
    {
        /// <summary>
        /// True if the exchange should be bound to a queue on send (for error queues, etc.)
        /// </summary>
        bool BindToQueue { get; }

        /// <summary>
        /// The queue to create/bind to the exchange
        /// </summary>
        string QueueName { get; }

        /// <summary>
        /// Arguments passed to QueueDeclare
        /// </summary>
        IDictionary<string, object> QueueArguments { get; }

        /// <summary>
        /// The exchange bindings for the send
        /// </summary>
        IEnumerable<ExchangeBindingSettings> ExchangeBindings { get; }

        /// <summary>
        /// Returns the send address for the settings
        /// </summary>
        /// <param name="hostAddress"></param>
        /// <returns></returns>
        Uri GetSendAddress(Uri hostAddress);
    }
}