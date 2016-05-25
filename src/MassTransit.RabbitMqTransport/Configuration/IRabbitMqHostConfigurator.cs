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
namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using Configurators;


    public interface IRabbitMqHostConfigurator
    {
        /// <summary>
        /// Configure the use of SSL to connection to RabbitMQ
        /// </summary>
        /// <param name="configureSsl"></param>
        void UseSsl(Action<IRabbitMqSslConfigurator> configureSsl);

        /// <summary>
        /// Specifies the heartbeat interval, in seconds, used to maintain the connection to RabbitMQ.
        /// Setting this value to zero will disable heartbeats, allowing the connection to timeout
        /// after an inactivity period.
        /// </summary>
        /// <param name="requestedHeartbeat"></param>
        void Heartbeat(ushort requestedHeartbeat);

        /// <summary>
        /// Sets the username for the connection to RabbitMQ
        /// </summary>
        /// <param name="username"></param>
        void Username(string username);

        /// <summary>
        /// Sets the password for the connection to RabbitMQ
        /// </summary>
        /// <param name="password"></param>
        void Password(string password);

        /// <summary>
        /// Configure a RabbitMQ High-Availability cluster which will cycle hosts when connections are interrupted.
        /// </summary>
        /// <param name="configureCluster">The cluster configuration</param>
        void UseCluster(Action<IRabbitMqClusterConfigurator> configureCluster);
    }
}