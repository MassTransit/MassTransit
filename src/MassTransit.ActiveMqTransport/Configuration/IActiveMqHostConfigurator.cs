// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ActiveMqTransport
{
    using System.Collections.Generic;


    public interface IActiveMqHostConfigurator
    {
        /// <summary>
        /// Sets the username for the connection to ActiveMQ
        /// </summary>
        /// <param name="username"></param>
        void Username(string username);

        /// <summary>
        /// Sets the password for the connection to ActiveMQ
        /// </summary>
        /// <param name="password"></param>
        void Password(string password);

        void UseSsl();

        /// <summary>
        /// Sets a list of hosts to enable the failover transport
        /// </summary>
        /// <param name="hosts"></param>
        void FailoverHosts(string[] hosts);

        /// <summary>
        /// Sets options on the underlying NMS transport
        /// </summary>
        /// <param name="options"></param>
        void TransportOptions(Dictionary<string, string> options);
    }
}