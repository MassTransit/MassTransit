// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport
{
    using System;


    /// <summary>
    /// Configures an exchange for RabbitMQ
    /// </summary>
    public interface IExchangeConfigurator
    {
        /// <summary>
        /// Specify the queue should be durable (survives broker restart) or in-memory
        /// </summary>
        /// <value>True for a durable queue, False for an in-memory queue</value>
        bool Durable { set; }

        /// <summary>
        /// Specify that the queue (and the exchange of the same name) should be created as auto-delete
        /// </summary>
        bool AutoDelete { set; }

        /// <summary>
        /// Specify the exchange type for the endpoint
        /// </summary>
        string ExchangeType { set; }

        /// <summary>
        /// Set an exchange argument passed to the broker on queue declaration
        /// </summary>
        /// <param name="key">The argument key</param>
        /// <param name="value">The argument value</param>
        void SetExchangeArgument(string key, object value);

        /// <summary>
        /// Set the exchange argument to the TimeSpan (which is converted to milliseconds)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetExchangeArgument(string key, TimeSpan value);

        RabbitMqEndpointAddress GetEndpointAddress(Uri hostAddress);
    }
}
