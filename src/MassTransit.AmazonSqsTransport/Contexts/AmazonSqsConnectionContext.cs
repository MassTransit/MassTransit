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
namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Logging;
    using Topology;
    using Util;


    public class AmazonSqsConnectionContext :
        BasePipeContext,
        ConnectionContext,
        IAsyncDisposable
    {
        static readonly ILog _log = Logger.Get<AmazonSqsConnectionContext>();

        readonly LimitedConcurrencyLevelTaskScheduler _taskScheduler;

        public AmazonSqsConnectionContext(IConnection connection, AmazonSqsHostSettings hostSettings, IAmazonSqsHostTopology topology, string description,
            CancellationToken cancellationToken)
            : base(new PayloadCache(), cancellationToken)
        {
            Connection = connection;
            HostSettings = hostSettings;
            Topology = topology;

            Description = description;

            _taskScheduler = new LimitedConcurrencyLevelTaskScheduler(1);
        }

        public IConnection Connection { get; }
        public IAmazonSqsHostTopology Topology { get; }
        public AmazonSqsHostSettings HostSettings { get; }
        public string Description { get; }
        public Uri HostAddress => HostSettings.HostAddress;

        public Task<IAmazonSQS> CreateAmazonSqs()
        {
            return Task.Factory.StartNew(() => Connection.CreateAmazonSqs(), CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        public Task<IAmazonSimpleNotificationService> CreateAmazonSns()
        {
            return Task.Factory.StartNew(() => Connection.CreateAmazonSns(), CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Disconnecting: {0}", Description);

            try
            {
                Connection.Dispose();
            }
            catch (Exception exception)
            {
                if (_log.IsErrorEnabled)
                    _log.Error($"Close _connection Faulted: {Description}", exception);
            }

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Disconnected: {0}", Description);

            return TaskUtil.Completed;
        }
    }
}