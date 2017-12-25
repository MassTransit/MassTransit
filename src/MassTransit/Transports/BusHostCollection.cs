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
namespace MassTransit.Transports
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;


    public class BusHostCollection<THost> :
        IBusHostCollection
        where THost : IBusHostControl
    {
        readonly List<THost> _hosts;

        public BusHostCollection(params THost[] hosts)
        {
            _hosts = new List<THost>(hosts);
        }

        public THost this[int index] => _hosts[index];
        public int Count => _hosts.Count;

        IEnumerator<IBusHostControl> IEnumerable<IBusHostControl>.GetEnumerator()
        {
            foreach (THost host in _hosts)
            {
                yield return host;
            }
        }

        public IEnumerable<T> GetHosts<T>()
            where T : class, IHost
        {
            foreach (var host in _hosts)
            {
                T result = host as T;
                if (result != null)
                    yield return result;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _hosts.GetEnumerator();
        }

        public void Add(THost host)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            _hosts.Add(host);
        }

        public IEnumerable<THost> Hosts => _hosts;

        IHost IBusHostCollection.GetHost(Uri address)
        {
            return GetHost(address);
        }

        public THost GetHost(Uri address)
        {
            var host = _hosts.SingleOrDefault(x => x.Matches(address));
            if (host == null)
                throw new EndpointNotFoundException($"The host was not found for the specified address: {address}");

            return host;
        }

        public IEnumerable<THost> GetHosts(Uri address)
        {
            foreach (THost candidateHost in _hosts)
            {
                if (candidateHost.Matches(address))
                {
                    yield return candidateHost;
                }
            }
        }
    }
}