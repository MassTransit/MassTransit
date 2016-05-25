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
namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Logging;


    /// <summary>
    /// Creates an IHostnameSelector which sequentially chooses the next host name from the provided list based on index
    /// </summary>
    public class SequentialHostnameSelector :
        IRabbitMqHostNameSelector
    {
        static readonly ILog _log = Logger.Get<SequentialHostnameSelector>();
        string _lastHost;
        int _nextHostIndex;

        public SequentialHostnameSelector()
        {
            _nextHostIndex = 0;
            _lastHost = "";
        }

        public string NextFrom(IList<string> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (options.All(string.IsNullOrWhiteSpace))
                throw new ArgumentException("There must be at least one host to use a hostname selector.", nameof(options));

            do
            {
                _lastHost = options[_nextHostIndex % options.Count];
            }
            while (string.IsNullOrWhiteSpace(_lastHost));
            
            if (_log.IsDebugEnabled)
                _log.Debug($"Returning next host: {_lastHost}");

            Interlocked.Increment(ref _nextHostIndex);

            return _lastHost;
        }

        public string LastHost => _lastHost;
    }
}