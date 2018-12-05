// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Configurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Transport;


    public class RabbitMqClusterConfigurator :
        IRabbitMqClusterConfigurator
    {
        List<string> _clusterMembers;

        public RabbitMqClusterConfigurator()
        {
            _clusterMembers = new List<string>();
        }

        public string[] ClusterMembers
        {
            get => _clusterMembers.ToArray();
            set => _clusterMembers = value.ToList();
        }

        public void Node(string clusterNodeHostname)
        {
            if (string.IsNullOrWhiteSpace(clusterNodeHostname))
                throw new ArgumentException("Cluster node hostname cannot be empty.");

            _clusterMembers.Add(clusterNodeHostname);
        }

        public IRabbitMqEndpointResolver GetHostNameSelector()
        {
            if (_clusterMembers.Count <= 0)
                return null;

            return new SequentialEndpointResolver(_clusterMembers.ToArray());
        }
    }
}