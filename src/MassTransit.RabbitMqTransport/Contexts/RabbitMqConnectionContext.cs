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
    using System.Threading.Tasks;
    using Context;
    using Logging;
    using RabbitMQ.Client;
    using Util;


    public class RabbitMqConnectionContext :
        BasePipeContext,
        ConnectionContext
    {
        static readonly ILog _log = Logger.Get<RabbitMqConnectionContext>();

        readonly IConnection _connection;
        readonly RabbitMqHostSettings _hostSettings;
        readonly ITaskParticipant _participant;
        readonly LimitedConcurrencyLevelTaskScheduler _taskScheduler;

        public RabbitMqConnectionContext(IConnection connection, RabbitMqHostSettings hostSettings, ITaskSupervisor supervisor)
            : this(connection, hostSettings,
                supervisor.CreateParticipant($"{TypeMetadataCache<RabbitMqConnectionContext>.ShortName} - {hostSettings.ToDebugString()}"))
        {
        }

        RabbitMqConnectionContext(IConnection connection, RabbitMqHostSettings hostSettings, ITaskParticipant participant)
            : base(new PayloadCache(), participant.StoppedToken)
        {
            _connection = connection;
            _hostSettings = hostSettings;

            _participant = participant;

            _taskScheduler = new LimitedConcurrencyLevelTaskScheduler(1);

            connection.ConnectionShutdown += OnConnectionShutdown;
        }

        public RabbitMqHostSettings HostSettings => _hostSettings;

        public Task<IModel> CreateModel()
        {
            return Task.Factory.StartNew(() => _connection.CreateModel(),
                _participant.StoppedToken, TaskCreationOptions.HideScheduler, _taskScheduler);
        }

        public IConnection Connection => _connection;

        public void Dispose()
        {
            _connection.ConnectionShutdown -= OnConnectionShutdown;

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Disconnecting: {0}", _hostSettings.ToDebugString());

            _connection.Cleanup(200, "Connection Disposed");

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Disconnected: {0}", _hostSettings.ToDebugString());

            _participant.SetComplete();
        }

        void OnConnectionShutdown(object connection, ShutdownEventArgs reason)
        {
            _connection.Cleanup(reason.ReplyCode, reason.ReplyText);

            _participant.SetComplete();
        }
    }
}