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
    using Configuration;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Logging;
    using Microsoft.Extensions.Logging;
    using Topology;
    using Util;


    public class ActiveMqConnectionContext :
        BasePipeContext,
        ConnectionContext,
        IAsyncDisposable
    {
        static readonly ILogger _logger = Logger.Get<ActiveMqConnectionContext>();

        readonly IConnection _connection;
        readonly IActiveMqHostConfiguration _configuration;
        readonly LimitedConcurrencyLevelTaskScheduler _taskScheduler;

        public ActiveMqConnectionContext(IConnection connection, IActiveMqHostConfiguration configuration, CancellationToken cancellationToken)
            : base(new PayloadCache(), cancellationToken)
        {
            _connection = connection;
            _configuration = configuration;

            _taskScheduler = new LimitedConcurrencyLevelTaskScheduler(1);
        }

        public string Description => _configuration.Description;
        public Uri HostAddress => _configuration.HostAddress;
        public IActiveMqHostTopology Topology => _configuration.Topology;

        public Task<ISession> CreateSession()
        {
            return Task.Factory.StartNew(() => _connection.CreateSession(), CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        IConnection ConnectionContext.Connection => _connection;

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Disconnecting: {0}", Description);

            try
            {
                _connection.Close();

                _connection.Dispose();
            }
            catch (Exception exception)
            {
                _logger.LogError($"Close Connection Faulted: {Description}", exception);
            }

            _logger.LogDebug("Disconnected: {0}", Description);

            return TaskUtil.Completed;
        }
    }
}
