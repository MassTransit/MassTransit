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
namespace MassTransit.RabbitMqTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Logging;
    using RabbitMQ.Client;
    using Util;


    public class RabbitMqConnectionContext :
        ConnectionContext,
        IDisposable
    {
        static readonly ILog _log = Logger.Get<RabbitMqConnectionContext>();
        readonly IConnection _connection;
        readonly RabbitMqHostSettings _hostSettings;
        readonly ITaskParticipant _participant;
        readonly PayloadCache _payloadCache;
        readonly QueuedTaskScheduler _taskScheduler;

        public RabbitMqConnectionContext(IConnection connection, RabbitMqHostSettings hostSettings, ITaskSupervisor supervisor)
        {
            _connection = connection;
            _hostSettings = hostSettings;
            _payloadCache = new PayloadCache();

            _participant = supervisor.CreateParticipant($"{TypeMetadataCache<RabbitMqConnectionContext>.ShortName} - {_hostSettings.ToDebugString()}");

            _taskScheduler = new QueuedTaskScheduler(TaskScheduler.Default, 1);

            connection.ConnectionShutdown += OnConnectionShutdown;
        }

        public RabbitMqHostSettings HostSettings => _hostSettings;

        public async Task<IModel> CreateModel()
        {
            return await Task.Factory.StartNew(() => _connection.CreateModel(),
                _participant.StoppedToken, TaskCreationOptions.HideScheduler, _taskScheduler).ConfigureAwait(false);
        }

        public bool HasPayloadType(Type contextType)
        {
            return _payloadCache.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload context)
            where TPayload : class
        {
            return _payloadCache.TryGetPayload(out context);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            where TPayload : class
        {
            return _payloadCache.GetOrAddPayload(payloadFactory);
        }

        public IConnection Connection => _connection;

        public CancellationToken CancellationToken => _participant.StoppedToken;

        public void Dispose()
        {
            _connection.ConnectionShutdown -= OnConnectionShutdown;

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Disconnecting: {0}", _hostSettings.ToDebugString());

            Close(200, "Connection Disposed");

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Disconnected: {0}", _hostSettings.ToDebugString());

            _participant.SetComplete();
        }

        void OnConnectionShutdown(object connection, ShutdownEventArgs reason)
        {
            Close(reason.ReplyCode, reason.ReplyText);

            _participant.SetComplete();
        }

        void Close(ushort replyCode, string message)
        {
            _connection.Cleanup(replyCode, message);
        }
    }
}