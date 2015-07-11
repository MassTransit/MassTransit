// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Integration;
    using Internals.Extensions;
    using Logging;
    using MassTransit.Pipeline;
    using Monitoring.Introspection;
    using Policies;
    using Topology;
    using Transports;


    public class RabbitMqReceiveTransport :
        IReceiveTransport
    {
        static readonly ILog _log = Logger.Get<RabbitMqReceiveTransport>();

        readonly IConnectionCache _connectionCache;
        readonly ExchangeBindingSettings[] _exchangeBindings;
        readonly ReceiveObservable _receiveObservers;
        readonly ReceiveSettings _settings;

        public RabbitMqReceiveTransport(IConnectionCache connectionCache, ReceiveSettings settings,
            params ExchangeBindingSettings[] exchangeBindings)
        {
            _connectionCache = connectionCache;
            _settings = settings;
            _exchangeBindings = exchangeBindings;
            _receiveObservers = new ReceiveObservable();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("transport");
            scope.Add("type", "RabbitMQ");
            scope.Set(_settings);
            scope.Add("bindings", _exchangeBindings);
        }

        /// <summary>
        /// Start the receive transport, returning a Task that can be awaited to signal the transport has 
        /// completely shutdown once the cancellation token is cancelled.
        /// </summary>
        /// <param name="receivePipe"></param>
        /// <returns>A task that is completed once the transport is shut down</returns>
        public ReceiveTransportHandle Start(IPipe<ReceiveContext> receivePipe)
        {
            var stopTokenSource = new CancellationTokenSource();

            IPipe<ConnectionContext> pipe = Pipe.New<ConnectionContext>(x => x.RabbitMqConsumer(receivePipe, _settings, _receiveObservers, _exchangeBindings));

            Task receiverTask = Receiver(pipe, stopTokenSource.Token);

            return new Handle(stopTokenSource, receiverTask);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveObservers.Connect(observer);
        }

        async Task Receiver(IPipe<ConnectionContext> transportPipe, CancellationToken stopToken)
        {
            await Repeat.UntilCancelled(stopToken, async () =>
            {
                try
                {
                    await _connectionCache.Send(transportPipe, stopToken);
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
                        _log.ErrorFormat("RabbitMQ receive transport failed: {0}", ex.Message);
                }
            });
        }


        class Handle :
            ReceiveTransportHandle
        {
            readonly Task _receiverTask;
            readonly CancellationTokenSource _stop;

            public Handle(CancellationTokenSource cancellationTokenSource, Task receiverTask)
            {
                _stop = cancellationTokenSource;
                _receiverTask = receiverTask;
            }

            void IDisposable.Dispose()
            {
                _stop.Cancel();
            }

            async Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                _stop.Cancel();

                await _receiverTask.WithCancellation(cancellationToken);
            }
        }
    }
}