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
    using Configuration;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Logging;
    using Microsoft.Extensions.Logging;
    using RabbitMQ.Client;
    using Topology;
    using Util;


    public class RabbitMqConnectionContext :
        BasePipeContext,
        ConnectionContext,
        IAsyncDisposable
    {
        static readonly ILogger _logger = Logger.Get<RabbitMqConnectionContext>();

        readonly IConnection _connection;
        readonly LimitedConcurrencyLevelTaskScheduler _taskScheduler;

        public RabbitMqConnectionContext(IConnection connection, IRabbitMqHostConfiguration configuration, string description,
            CancellationToken cancellationToken)
            : base(new PayloadCache(), cancellationToken)
        {
            _connection = connection;

            Description = description;
            HostAddress = configuration.HostAddress;
            PublisherConfirmation = configuration.PublisherConfirmation;
            Topology = configuration.Topology;

            StopTimeout = TimeSpan.FromSeconds(30);

            _taskScheduler = new LimitedConcurrencyLevelTaskScheduler(1);

            connection.ConnectionShutdown += OnConnectionShutdown;
        }

        IConnection ConnectionContext.Connection => _connection;
        public string Description { get; }
        public Uri HostAddress { get; }
        public bool PublisherConfirmation { get; }
        public TimeSpan StopTimeout { get; }
        public IRabbitMqHostTopology Topology { get; }

        public async Task<IModel> CreateModel(CancellationToken cancellationToken)
        {
            using (var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken))
            {
                var model = await Task.Factory.StartNew(() => _connection.CreateModel(), tokenSource.Token, TaskCreationOptions.None, _taskScheduler)
                    .ConfigureAwait(false);

                return model;
            }
        }

        async Task<ModelContext> ConnectionContext.CreateModelContext(CancellationToken cancellationToken)
        {
            var model = await CreateModel(cancellationToken).ConfigureAwait(false);

            return new RabbitMqModelContext(this, model, cancellationToken);
        }

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            _connection.ConnectionShutdown -= OnConnectionShutdown;

            _logger.LogDebug("Disconnecting: {0}", Description);

            _connection.Cleanup(200, "Connection Disposed");

            _logger.LogDebug("Disconnected: {0}", Description);

            return TaskUtil.Completed;
        }

        void OnConnectionShutdown(object connection, ShutdownEventArgs reason)
        {
            _connection.Cleanup(reason.ReplyCode, reason.ReplyText);
        }
    }
}
