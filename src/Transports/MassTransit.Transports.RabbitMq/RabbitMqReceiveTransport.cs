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
    using Pipeline.Filters;
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

        public async Task Start(IPipe<ReceiveContext> pipe, CancellationToken cancellationToken)
        {
            IRepeatPolicy repeatPolicy = Repeat.UntilCancelled(cancellationToken);
            var retryPolicy = new CancelRetryPolicy(_retryPolicy, cancellationToken);

            IFilter<ModelContext> modelConsumer = new ReceiveConsumerFilter(pipe, _settings);

            IFilter<ConnectionContext> modelFilter = new ReceiveModelFilter(modelConsumer.Combine());
            IFilter<ConnectionContext> retryFilter = new RetryFilter<ConnectionContext>(retryPolicy);

            IPipe<ConnectionContext> receivePipe = retryFilter.Combine(modelFilter);

            IRepeatContext repeatContext = repeatPolicy.GetRepeatContext();
            TimeSpan delay = TimeSpan.Zero;
            do
            {
                if (delay > TimeSpan.Zero)
                    await Task.Delay(delay, repeatContext.CancellationToken);

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
            }
            while (repeatContext.CanRepeat(out delay));
        }
    }
}