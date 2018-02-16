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
namespace MassTransit.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Topology;
    using Transports;


    public class HostCollection<TConfiguration> :
        IHostCollection<TConfiguration>
        where TConfiguration : IHostConfiguration
    {
        readonly IList<TConfiguration> _hosts;

        public HostCollection()
        {
            _hosts = new List<TConfiguration>();
        }

        public TConfiguration[] Hosts => _hosts.ToArray();

        public int Count => _hosts.Count;

        public bool TryGetHost(Uri address, out TConfiguration host)
        {
            foreach (var hostConfiguration in _hosts)
            {
                if (hostConfiguration.Host.Matches(address))
                {
                    host = hostConfiguration;
                    return host != null;
                }
            }

            host = default(TConfiguration);
            return false;
        }

        public void Add(TConfiguration configuration)
        {
            _hosts.Add(configuration);
        }

        public IBusTopology GetBusTopology()
        {
            if (_hosts.Count == 0)
                throw new ConfigurationException("At least one host must be configured");

            return _hosts[0].Host.Topology;
        }

        public IBusHostControl[] GetHosts()
        {
            return _hosts.Select(x => x.Host).ToArray();
        }

        public bool TryGetHost(Uri address, out IBusHostControl host)
        {
            if (TryGetHost(address, out TConfiguration hostConfiguration))
            {
                host = hostConfiguration.Host;
                return true;
            }

            host = default(IBusHostControl);
            return false;
        }

        public bool TryGetHost(IHost host, out TConfiguration hostConfiguration)
        {
            foreach (var candidate in _hosts)
            {
                if (ReferenceEquals(candidate.Host, host))
                {
                    hostConfiguration = candidate;
                    return true;
                }
            }

            hostConfiguration = default(TConfiguration);
            return false;
        }

        public TConfiguration this[int index] => _hosts[index];

        public IEnumerator<IBusHostControl> GetEnumerator()
        {
            return _hosts.Select(host => host.Host).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}