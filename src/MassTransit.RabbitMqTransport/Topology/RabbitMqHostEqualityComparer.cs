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
namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using System.Collections.Generic;


    public sealed class RabbitMqHostEqualityComparer :
        IEqualityComparer<RabbitMqHostSettings>
    {
        static readonly IEqualityComparer<RabbitMqHostSettings> _instance = new RabbitMqHostEqualityComparer();

        public static IEqualityComparer<RabbitMqHostSettings> Default
        {
            get { return _instance; }
        }

        public bool Equals(RabbitMqHostSettings x, RabbitMqHostSettings y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null))
                return false;
            if (ReferenceEquals(y, null))
                return false;
            return string.Equals(x.Host, y.Host, StringComparison.OrdinalIgnoreCase) && x.Port == y.Port && string.Equals(x.VirtualHost, y.VirtualHost);
        }

        public int GetHashCode(RabbitMqHostSettings obj)
        {
            unchecked
            {
                int hashCode = (obj.Host != null ? obj.Host.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.Port;
                hashCode = (hashCode * 397) ^ (obj.VirtualHost != null ? obj.VirtualHost.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}