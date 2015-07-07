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
namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Integration;
    using Logging;
    using MassTransit.Pipeline;
    using Transports;
    using Util;


    public class RabbitMqHost :
        IRabbitMqHost,
        IBusHostControl
    {
        readonly RabbitMqConnectionCache _connectionCache;
        readonly RabbitMqHostSettings _hostSettings;
        readonly ILog _log = Logger.Get<RabbitMqHost>();
        readonly IMessageNameFormatter _messageNameFormatter;

        public RabbitMqHost(RabbitMqHostSettings hostSettings)
        {
            _hostSettings = hostSettings;

            _connectionCache = new RabbitMqConnectionCache(hostSettings);
            _messageNameFormatter = new RabbitMqMessageNameFormatter();
        }

        public HostHandle Start()
        {
            var signal = new StopSignal();

            IPipe<ConnectionContext> connectionPipe = Pipe.ExecuteAsync<ConnectionContext>(async context =>
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Connection established to {0}", _hostSettings.ToDebugString());

                await signal.Stopped;

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Closing connection to host {0}", _hostSettings.ToDebugString());

                // this is a bad thing, we need to cascade everything to a stopped state before closing.
                context.Connection.Close(200, "Host stopped");
            });

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Starting connection to {0}", _hostSettings.ToDebugString());

            _connectionCache.Send(connectionPipe, signal.CancellationToken);

            return new Handle(signal);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("host");
            scope.Set(new
            {
                Type = "RabbitMQ",
                _hostSettings.Host,
                _hostSettings.Port,
                _hostSettings.VirtualHost,
                _hostSettings.Username,
                Password = new string('*', _hostSettings.Password.Length),
                _hostSettings.Heartbeat,
                _hostSettings.Ssl,
            });

            if (_hostSettings.Ssl)
            {
                scope.Set(new
                {
                    _hostSettings.SslServerName,
                });
            }

            _connectionCache.Probe(scope);
        }

        public IMessageNameFormatter MessageNameFormatter
        {
            get { return _messageNameFormatter; }
        }

        public IConnectionCache ConnectionCache
        {
            get { return _connectionCache; }
        }

        public RabbitMqHostSettings Settings
        {
            get { return _hostSettings; }
        }


        class Handle :
            HostHandle
        {
            readonly StopSignal _stopSignal;

            public Handle(StopSignal stopSignal)
            {
                _stopSignal = stopSignal;
            }

            void IDisposable.Dispose()
            {
                _stopSignal.Stop();
            }

            async Task HostHandle.Stop(CancellationToken cancellationToken)
            {
                _stopSignal.Stop();
            }
        }
    }
}