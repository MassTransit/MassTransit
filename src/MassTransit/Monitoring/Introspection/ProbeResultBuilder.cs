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
namespace MassTransit.Monitoring.Introspection
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Contracts;
    using Util;


    public class ProbeResultBuilder :
        ScopeProbeContext,
        IProbeResultBuilder
    {
        readonly HostInfo _host;
        readonly Guid _probeId;
        readonly Guid _resultId;
        readonly DateTime _startTimestamp;

        public ProbeResultBuilder(Guid probeId, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _probeId = probeId;

            _resultId = NewId.NextGuid();
            _startTimestamp = DateTime.UtcNow;
            _host = HostMetadataCache.Host;
        }

        ProbeResult IProbeResultBuilder.Build()
        {
            TimeSpan duration = DateTime.UtcNow - _startTimestamp;

            return new Result(_probeId, _resultId, _startTimestamp, duration, _host, Build());
        }


        class Result :
            ProbeResult
        {
            readonly IDictionary<string, object> _results;
            readonly TimeSpan _duration;
            readonly HostInfo _host;
            readonly Guid _probeId;
            readonly Guid _resultId;
            readonly DateTime _startTimestamp;

            public Result(Guid probeId, Guid resultId, DateTime startTimestamp, TimeSpan duration, HostInfo host, IDictionary<string, object> results)
            {
                _probeId = probeId;
                _resultId = resultId;
                _startTimestamp = startTimestamp;
                _duration = duration;
                _host = host;
                _results = results;
            }

            public Guid ResultId
            {
                get { return _resultId; }
            }

            public Guid ProbeId
            {
                get { return _probeId; }
            }

            public DateTime StartTimestamp
            {
                get { return _startTimestamp; }
            }

            public TimeSpan Duration
            {
                get { return _duration; }
            }

            public HostInfo Host
            {
                get { return _host; }
            }

            public IDictionary<string, object> Results

            {
                get { return _results; }
            }
        }
    }


    public interface IProbeResultBuilder
    {
        ProbeResult Build();
    }
}