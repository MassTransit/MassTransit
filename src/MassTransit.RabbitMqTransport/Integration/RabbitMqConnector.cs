// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Integration
{
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Contexts;
    using Logging;
    using MassTransit.Pipeline;
    using Pipeline;
    using Policies;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Exceptions;


    /// <summary>
    /// Establishes connections to RabbitMQ using the specified retry policy
    /// </summary>
    public class RabbitMqConnector :
        IRabbitMqConnector
    {
        readonly ConnectionFactory _connectionFactory;
        readonly RabbitMqHostSettings _hostSettings;
        readonly ILog _log = Logger.Get<RabbitMqConnector>();
        readonly IRetryPolicy _retryPolicy;

        public RabbitMqConnector(RabbitMqHostSettings hostSettings, IRetryPolicy retryPolicy)
        {
            _hostSettings = hostSettings;
            _retryPolicy = retryPolicy;

            _connectionFactory = hostSettings.GetConnectionFactory();
        }

        public Task Connect(IPipe<ConnectionContext> pipe, CancellationToken cancellationToken)
        {
            return _retryPolicy.Retry(async () =>
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Connecting: {0}", _connectionFactory.ToDebugString());

                try
                {
                    using (IConnection connection = _connectionFactory.CreateConnection())
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Connected: {0}", _connectionFactory.ToDebugString());

                        using (var connectionContext = new RabbitMqConnectionContext(connection, _connectionFactory, cancellationToken))
                        {
                            connectionContext.GetOrAddPayload(() => _hostSettings);

                            await pipe.Send(connectionContext);
                        }

                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Disconnecting: {0}", _connectionFactory.ToDebugString());
                    }

                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Disconnected: {0}", _connectionFactory.ToDebugString());
                }
                catch (BrokerUnreachableException ex)
                {
                    throw new RabbitMqConnectionException("Connect failed: " + _connectionFactory.ToDebugString(), ex);
                }
            }, cancellationToken);
        }
    }
}