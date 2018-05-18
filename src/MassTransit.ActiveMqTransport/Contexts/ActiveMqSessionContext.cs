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
    using Apache.NMS.Util;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Logging;
    using Topology;
    using Util;


    public class ActiveMqSessionContext :
        BasePipeContext,
        SessionContext,
        IAsyncDisposable
    {
        static readonly ILog _log = Logger.Get<ActiveMqSessionContext>();

        readonly ConnectionContext _connectionContext;
        readonly IActiveMqHost _host;
        readonly ISession _session;
        readonly LimitedConcurrencyLevelTaskScheduler _taskScheduler;
        readonly MessageProducerCache _messageProducerCache;

        public ActiveMqSessionContext(ConnectionContext connectionContext, ISession session, IActiveMqHost host, CancellationToken cancellationToken)
            : base(new PayloadCacheScope(connectionContext), cancellationToken)
        {
            _connectionContext = connectionContext;
            _session = session;
            _host = host;

            _taskScheduler = new LimitedConcurrencyLevelTaskScheduler(1);

            _messageProducerCache = new MessageProducerCache();
        }

        public async Task DisposeAsync(CancellationToken cancellationToken)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Closing session: {0}", _connectionContext.Description);

            if (_session != null)
            {
                try
                {
                    await _messageProducerCache.Stop(cancellationToken).ConfigureAwait(false);

                    _session.Close();
                }
                catch (Exception ex)
                {
                    _log.Warn("Session close faulted", ex);
                }

                _session.Dispose();
            }
        }

        IActiveMqPublishTopology SessionContext.PublishTopology => _host.Topology.PublishTopology;

        ISession SessionContext.Session => _session;

        ConnectionContext SessionContext.ConnectionContext => _connectionContext;

        public Task<ITopic> GetTopic(string topicName)
        {
            return Task.Factory.StartNew(() => SessionUtil.GetTopic(_session, topicName), CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        public Task<IQueue> GetQueue(string queueName)
        {
            return Task.Factory.StartNew(() => SessionUtil.GetQueue(_session, queueName), CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        public Task<IDestination> GetDestination(string destination, DestinationType destinationType)
        {
            return Task.Factory.StartNew(() => SessionUtil.GetDestination(_session, destination, destinationType), CancellationToken, TaskCreationOptions.None,
                _taskScheduler);
        }

        public Task<IMessageProducer> CreateMessageProducer(IDestination destination)
        {
            return _messageProducerCache.GetMessageProducer(destination, x =>
                Task.Factory.StartNew(() => _session.CreateProducer(x), CancellationToken, TaskCreationOptions.None, _taskScheduler));
        }

        public Task<IMessageConsumer> CreateMessageConsumer(IDestination destination, string selector, bool noLocal)
        {
            return Task.Factory.StartNew(() => _session.CreateConsumer(destination, selector, noLocal), CancellationToken, TaskCreationOptions.None,
                _taskScheduler);
        }

        public Task DeleteTopic(string topicName)
        {
            return Task.Factory.StartNew(() => SessionUtil.DeleteTopic(_session, topicName), CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }

        public Task DeleteQueue(string queueName)
        {
            return Task.Factory.StartNew(() => SessionUtil.DeleteQueue(_session, queueName), CancellationToken, TaskCreationOptions.None, _taskScheduler);
        }
    }
}