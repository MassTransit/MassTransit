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
namespace MassTransit.Transports.RabbitMq
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Pipeline;
    using Pipeline;
    using Policies;


    public class RabbitMqReceiveTransport :
        IReceiveTransport
    {
        static readonly ILog _log = Logger.Get<RabbitMqReceiveTransport>();

        readonly IRabbitMqConnector _connector;
        readonly IRetryPolicy _retryPolicy;
        readonly ReceiveSettings _settings;
        readonly SubscriptionSettings[] _subscriptions;

        public RabbitMqReceiveTransport(IRabbitMqConnector connector, IRetryPolicy retryPolicy, ReceiveSettings settings,
            params SubscriptionSettings[] subscriptions)
        {
            _connector = connector;
            _retryPolicy = retryPolicy;
            _settings = settings;
            _subscriptions = subscriptions;
        }

        /// <summary>
        /// Start the receive transport, returning a Task that can be awaited to signal the transport has 
        /// completely shutdown once the cancellation token is cancelled.
        /// </summary>
        /// <param name="receivePipeeceiveContext pipe</param>
        /// <param name="cancellationToken">The cancellation token that is cancelled to terminate the receive transport</param>
        /// <returns>A task that is completed once the transport is shut down</returns>
        public Task Start(IPipe<ReceiveContext> receivePipe, CancellationToken cancellationToken)
        {
            IPipe<ConnectionContext> connectionPipe = Pipe.New<ConnectionContext>(x =>
            {
                x.Repeat(cancellationToken);
                x.Retry(_retryPolicy, cancellationToken);

                x.ModelConsumer(receivePipe, _settings, _subscriptions);
            });

            return Repeat.UntilCancelled(cancellationToken, async () =>
            {
                try
                {
                    await _connector.Connect(connectionPipe, cancellationToken);
                }
                catch (RabbitMqConnectionException ex)
                {
                    if (_log.IsErrorEnabled)
                        _log.ErrorFormat("RabbitMQ connection failed: {0}", ex.Message);                    
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception ex)
                {
                    if (_log.IsErrorEnabled)
                        _log.ErrorFormat("RabbitMQ connection failed: {0}", ex.Message);
                }
            });
        }
    }
}