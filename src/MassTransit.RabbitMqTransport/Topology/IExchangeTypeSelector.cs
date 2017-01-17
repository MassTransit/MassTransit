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
    /// <summary>
    /// During a topology build, this will determine the exchange type for a message,
    /// given the exchange name (entity name) and routing key which have already been determined.
    /// </summary>
    public interface IExchangeTypeSelector
    {
        /// <summary>
        /// The default exchange type
        /// </summary>
        string DefaultExchangeType { get; }

        /// <summary>
        /// Returns the exchange type for the send context
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="exchangeName">The exchange name</param>
        /// <returns>The exchange type for the send</returns>
        string GetExchangeType<T>(string exchangeName)
            where T : class;
    }
}