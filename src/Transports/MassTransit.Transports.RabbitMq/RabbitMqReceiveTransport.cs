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
    using Pipeline;
    using Policies;


    public class RabbitMqReceiveTransport :
        IReceiveTransport
    {
        readonly IRabbitMqConnector _connector;
        readonly ILog _log = Logger.Get<RabbitMqReceiveTransport>();
        readonly IRetryPolicy _retryPolicy;
        readonly ReceiveConsumerSettings _settings;

        public RabbitMqReceiveTransport(IRabbitMqConnector connector, IRetryPolicy retryPolicy, ReceiveConsumerSettings settings)
        {
            _connector = connector;
            _retryPolicy = retryPolicy;
            _settings = settings;
        }

        public Task Start(IPipe<ReceiveContext> pipe, CancellationToken cancellationToken)
        {
            IPipe<ConnectionContext> receivePipe = PipeFactory.New<ConnectionContext>(x =>
            {
                x.Repeat(cancellationToken);
                x.Retry(_retryPolicy, cancellationToken);
                x.ModelConsumer(pipe, _settings);
            });

            return Repeat.UntilCancelled(cancellationToken, async () =>
            {
                try
                {
                    await _connector.Connect(receivePipe, cancellationToken);
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