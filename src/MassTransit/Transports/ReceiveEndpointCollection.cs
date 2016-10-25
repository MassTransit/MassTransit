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


    public class ReceiveEndpointCollection :
        IReceiveEndpointCollection

    {
        readonly Dictionary<string, IReceiveEndpoint> _endpoints;

        public ReceiveEndpointCollection()
        {
            _endpoints = new Dictionary<string, IReceiveEndpoint>(StringComparer.OrdinalIgnoreCase);
        }

        public int Count => _endpoints.Count;

        IEnumerator<IReceiveEndpoint> IEnumerable<IReceiveEndpoint>.GetEnumerator()
        {
            foreach (var endpoint in _endpoints.Values)
            {
                yield return endpoint;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _endpoints.GetEnumerator();
        }

        public void Add(string endpointName, IReceiveEndpoint endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));
            
            if(_endpoints.ContainsKey(endpointName))
                throw new ConfigurationException($"A receive endpoint with the same key was already added: {endpointName}");

            _endpoints.Add(endpointName, endpoint);
        }
    }
}