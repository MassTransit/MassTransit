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
namespace MassTransit.ActiveMqTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.NMS;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Logging;
    using Topology;
    using Util;


    public class RabbitMqConnectionContext :
        BasePipeContext,
        ConnectionContext,
        IAsyncDisposable
    {
        static readonly ILog _log = Logger.Get<RabbitMqConnectionContext>();

        readonly IConnection _connection;
        readonly LimitedConcurrencyLevelTaskScheduler _taskScheduler;

        public RabbitMqConnectionContext(IConnection connection, ActiveMqHostSettings hostSettings, IActiveMqHostTopology topology, string description,
            CancellationToken cancellationToken)
            : base(new PayloadCache(), cancellationToken)
        {
            _connection = connection;
            HostSettings = hostSettings;
            Topology = topology;

            Description = description;

            _taskScheduler = new LimitedConcurrencyLevelTaskScheduler(1);

            connection.ConnectionInterruptedListener += OnConnectionInterrupted;
            connection.ConnectionResumedListener += OnConnectionResumed;
        }

        public IActiveMqHostTopology Topology { get; }
        public ActiveMqHostSettings HostSettings { get; }
        public string Description { get; }
        public Uri HostAddress => HostSettings.HostAddress;

        public Task<ISession> CreateSession()
        {
            return Task.Factory.StartNew(() => _connection.CreateSession(), CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        IConnection ConnectionContext.Connection => _connection;

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            _connection.ConnectionInterruptedListener -= OnConnectionInterrupted;
            _connection.ConnectionResumedListener -= OnConnectionResumed;

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Disconnecting: {0}", Description);

            _connection.Close();

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Disconnected: {0}", Description);

            return TaskUtil.Completed;
        }

        void OnConnectionInterrupted()
        {
        }

        void OnConnectionResumed()
        {
        }
    }
}