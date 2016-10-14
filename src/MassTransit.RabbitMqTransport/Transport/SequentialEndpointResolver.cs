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
namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Logging;
    using RabbitMQ.Client;


    /// <summary>
    /// Creates an IHostnameSelector which sequentially chooses the next host name from the provided list based on index
    /// </summary>
    public class SequentialEndpointResolver :
        IRabbitMqEndpointResolver
    {
        static readonly ILog _log = Logger.Get<SequentialEndpointResolver>();

        readonly string[] _hostNames;
        string _lastHost;
        int _nextHostIndex;

        public SequentialEndpointResolver(string[] hostNames)
        {
            if (hostNames == null)
                throw new ArgumentNullException(nameof(hostNames));
            if (hostNames.Length == 0)
                throw new ArgumentException("At least one host name must be specified", nameof(hostNames));
            if(hostNames.All(string.IsNullOrWhiteSpace))
                throw new ArgumentException("At least one non-blank host name must be specified", nameof(hostNames));

            _hostNames = hostNames;
            _nextHostIndex = 0;
            _lastHost = "";
        }

        public string LastHost => _lastHost;

        public IEnumerable<AmqpTcpEndpoint> All()
        {
            do
            {
                _lastHost = _hostNames[_nextHostIndex % _hostNames.Length];
            }
            while (string.IsNullOrWhiteSpace(_lastHost));

            if (_log.IsDebugEnabled)
                _log.Debug($"Returning next host: {_lastHost}");

            Interlocked.Increment(ref _nextHostIndex);

            yield return new AmqpTcpEndpoint(_lastHost);
        }
    }
}