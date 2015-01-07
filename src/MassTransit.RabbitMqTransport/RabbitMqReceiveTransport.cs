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
namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Internals.Extensions;
    using Logging;
    using MassTransit.Pipeline;
    using Pipeline;
    using Policies;
    using Transports;


    public class RabbitMqReceiveTransport :
        IReceiveTransport
    {
        static readonly ILog _log = Logger.Get<RabbitMqReceiveTransport>();

        readonly IConnectionCache _connectionCache;
        readonly ExchangeBindingSettings[] _exchangeBindings;
        readonly Uri _inputAddress;
        readonly IRetryPolicy _retryPolicy;
        readonly ReceiveSettings _settings;

        public RabbitMqReceiveTransport(IConnectionCache connectionCache, ReceiveSettings settings, Uri inputAddress,
            IRetryPolicy retryPolicy, params ExchangeBindingSettings[] exchangeBindings)
        {
            _connectionCache = connectionCache;
            _retryPolicy = retryPolicy;
            _settings = settings;
            _inputAddress = inputAddress;
            _exchangeBindings = exchangeBindings;
        }

        public Uri InputAddress
        {
            get { return _inputAddress; }
        }

        /// <summary>
        /// Start the receive transport, returning a Task that can be awaited to signal the transport has 
        /// completely shutdown once the cancellation token is cancelled.
        /// </summary>
        /// <param name="receivePipe"></param>
        /// <param name="cancellationToken">The cancellation token that is cancelled to terminate the receive transport</param>
        /// <returns>A task that is completed once the transport is shut down</returns>
        public async Task<ReceiveTransportHandle> Start(IPipe<ReceiveContext> receivePipe, CancellationToken cancellationToken)
        {
            var handle = new Handle();

            IPipe<ConnectionContext> transportPipe = Pipe.New<ConnectionContext>(x =>
            {
                // we want to retry the connection at intervals defined by the retry policy in the event of connection failures
                x.Retry(_retryPolicy, handle.StopToken);

                // we want our consumer to connect to the transport once connected
                x.RabbitMqConsumer(receivePipe, _settings, _exchangeBindings);
            });

            Receiver(handle, transportPipe);

            return handle;
        }

        async void Receiver(Handle handle, IPipe<ConnectionContext> transportPipe)
        {
            await Repeat.UntilCancelled(handle.StopToken, async () =>
            {
                try
                {
                    await _connectionCache.Send(transportPipe, handle.StopToken);
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

            handle.Stopped();
        }


        class Handle :
            ReceiveTransportHandle
        {
            readonly CancellationTokenSource _stop;
            readonly TaskCompletionSource<bool> _stopped;

            public Handle()
            {
                _stop = new CancellationTokenSource();
                _stopped = new TaskCompletionSource<bool>();
            }

            public CancellationToken StopToken
            {
                get { return _stop.Token; }
            }

            void IDisposable.Dispose()
            {
                _stop.Cancel();
            }

            async Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                _stop.Cancel();

                await _stopped.Task.WithCancellation(cancellationToken);
            }

            public void Stopped()
            {
                _stopped.TrySetResult(true);
            }
        }
    }
}