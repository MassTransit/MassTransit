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
namespace MassTransit.RabbitMqTransport.Configuration
{
    /// <summary>
    /// The binding configuration
    /// </summary>
    public interface IExchangeBindingConfigurator :
        IExchangeConfigurator
    {
        /// <summary>
        /// Sets the routing key for the binding
        /// </summary>
        string RoutingKey { set; }

        /// <summary>
        /// Sets a dynamic routingkeyDeterminer which creates routingkeys based on messages that are subscribed to
        /// </summary>
        IRoutingkeyFormatter RoutingkeyFormatter { set; }
    }
}