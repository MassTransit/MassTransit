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
namespace MassTransit.Transports
{
    using System;
    using System.Collections.Concurrent;


    /// <summary>
    /// Caches InMemory transport instances so that they are only created and used once
    /// </summary>
    public class InMemoryTransportCache :
        IReceiveTransportProvider,
        ISendTransportProvider,
        IDisposable
    {
        readonly ConcurrentDictionary<string, InMemoryTransport> _transports;

        public InMemoryTransportCache()
        {
            _transports = new ConcurrentDictionary<string, InMemoryTransport>(StringComparer.OrdinalIgnoreCase);
        }

        public void Dispose()
        {
            foreach (InMemoryTransport transport in _transports.Values)
                transport.Dispose();
        }

        public IReceiveTransport GetReceiveTransport(string queueName)
        {
            return _transports.GetOrAdd(queueName, name => new InMemoryTransport(new Uri(new Uri("loopback://localhost/"), name)));
        }

        public ISendTransport GetSendTransport(Uri address)
        {
            string queueName = address.AbsolutePath;
            if (queueName.StartsWith("/"))
                queueName = queueName.Substring(1);

            return _transports.GetOrAdd(queueName, name => new InMemoryTransport(new Uri(new Uri("loopback://localhost/"), name)));
        }
    }
}