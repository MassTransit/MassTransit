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
namespace MassTransit.RabbitMqTransport.Topology
{
    public interface IRoutingKeyFormatter
    {
        /// <summary>
        /// Format the routing key for the send context, so that it can be passed to RabbitMQ
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The message send context</param>
        /// <returns>The routing key to specify in the transport</returns>
        string FormatRoutingKey<T>(SendContext<T> context)
            where T : class;
    }
}