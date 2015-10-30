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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;


    public interface IServiceBusHostConfigurator
    {
        /// <summary>
        /// Sets the TokenProvider for the host
        /// </summary>
        TokenProvider TokenProvider { set; }

        /// <summary>
        /// Sets the operation timeout for the messaging factory
        /// </summary>
        TimeSpan OperationTimeout { set; }

        /// <summary>
        /// The minimum back off interval for the exponential retry policy
        /// </summary>
        TimeSpan RetryMinBackoff { set; }

        /// <summary>
        /// The maximum back off interval for the exponential retry policy
        /// </summary>
        TimeSpan RetryMaxBackoff { set; }

        /// <summary>
        /// The retry limit for service bus operations
        /// </summary>
        int RetryLimit { set; }

        /// <summary>
        /// Sets the messaging protocol to use with the messaging factory, defaults to AMQP.
        /// </summary>
        TransportType TransportType { set; }
        
        /// <summary>
        /// Sets the batch flush interval to use with the messaging factory.
        /// </summary>
        TimeSpan BatchFlushInterval { set; }
    }
}
