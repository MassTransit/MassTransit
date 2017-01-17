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
    using System;
    using MassTransit.Topology;


    public interface IRabbitMqHostTopology :
        IHostTopology
    {
        IExchangeTypeSelector ExchangeTypeSelector { get; }

        /// <summary>
        /// Throws if the entity name is invalid, either a queue or exchange name
        /// </summary>
        /// <param name="name">The name to validate</param>
        void ThrowIfEntityNameInvalid(string name);

        /// <summary>
        /// Determines if the entity name specified is valid
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsValidEntityName(string name);

        /// <summary>
        /// Returns the destination address for the specified exchange
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="configure">Callback to configure exchange settings</param>
        /// <returns></returns>
        Uri GetDestinationAddress(string exchangeName, Action<RabbitMqTransport.IExchangeConfigurator> configure = null);

        /// <summary>
        /// Returns the destination address for the specified message type
        /// </summary>
        /// <param name="messageType">The message type</param>
        /// <param name="configure">Callback to configure exchange settings</param>
        /// <returns></returns>
        Uri GetDestinationAddress(Type messageType, Action<RabbitMqTransport.IExchangeConfigurator> configure = null);

        /// <summary>
        /// Returns the settings for sending to the specified address. Will parse any arguments
        /// off the query string to properly configure the settings, including exchange and queue
        /// durability, etc.
        /// </summary>
        /// <param name="address">The RabbitMQ endpoint address</param>
        /// <returns>The send settings for the address</returns>
        SendSettings GetSendSettings(Uri address);

        /// <summary>
        /// Returns the settings for sending the specified message type. 
        /// </summary>
        /// <param name="messageType">The message type</param>
        /// <param name="configure"></param>
        /// <returns>The send settings for the address</returns>
        SendSettings GetSendSettings(Type messageType, Action<RabbitMqTransport.IExchangeConfigurator> configure = null);

        Uri GetDeadLetterAddress(ReceiveSettings settings);

        Uri GetErrorAddress(ReceiveSettings settings);
    }
}