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
    using Logging;
    using MassTransit.Pipeline;
    using Policies;
    using Topology;
    using Transports;
    using Util;


    public class RabbitMqReceiveTransport :
        IReceiveTransport
    {
        static readonly ILog _log = Logger.Get<RabbitMqReceiveTransport>();
        readonly ReceiveEndpointObservable _endpointObservers;
        readonly ExchangeBindingSettings[] _exchangeBindings;
        readonly IRabbitMqHost _host;
        readonly ReceiveObservable _receiveObservers;
        readonly ReceiveSettings _settings;

        public RabbitMqReceiveTransport(IRabbitMqHost host, ReceiveSettings settings,
            params ExchangeBindingSettings[] exchangeBindings)
        {
            _host = host;
            _settings = settings;
            _exchangeBindings = exchangeBindings;
            _receiveObservers = new ReceiveObservable();
            _endpointObservers = new ReceiveEndpointObservable();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("transport");
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
            var supervisor = new TaskSupervisor();

            var pipe = Pipe.New<ConnectionContext>(
                    x => x.RabbitMqConsumer(receivePipe, _settings, _receiveObservers, _endpointObservers, _exchangeBindings, supervisor));

            Receiver(pipe, supervisor);

            return new Handle(supervisor);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveObservers.Connect(observer);
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _endpointObservers.Connect(observer);
        }

        async void Receiver(IPipe<ConnectionContext> transportPipe, TaskSupervisor supervisor)
        {
            await Repeat.UntilCancelled(supervisor.StopToken, async () =>
            {
                try
                {
                    await _host.ConnectionCache.Send(transportPipe, supervisor.StopToken).ConfigureAwait(false);
                }
                catch (RabbitMqConnectionException ex)
                {
                    if (_log.IsErrorEnabled)
                        _log.ErrorFormat("RabbitMQ connection failed: {0}", ex.Message);

                    var inputAddress = _host.Settings.GetInputAddress(_settings);

                    await _endpointObservers.Faulted(new Faulted(inputAddress, ex)).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception ex)
                {
                    if (_log.IsErrorEnabled)
                        _log.ErrorFormat("RabbitMQ receive transport failed: {0}", ex.Message);

                    var inputAddress = _host.Settings.GetInputAddress(_settings);

                    await _endpointObservers.Faulted(new Faulted(inputAddress, ex)).ConfigureAwait(false);
                }
            }).ConfigureAwait(false);
        }


        class Faulted :
            ReceiveEndpointFaulted
        {
            public Faulted(Uri inputAddress, Exception exception)
            {
                InputAddress = inputAddress;
                Exception = exception;
            }

            public Uri InputAddress { get; }
            public Exception Exception { get; }
        }


        class Handle :
            ReceiveTransportHandle
        {
            readonly TaskSupervisor _supervisor;

            public Handle(TaskSupervisor supervisor)
            {
                _supervisor = supervisor;
            }

            async Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                await _supervisor.Stop("Receive Transport Stopping").ConfigureAwait(false);

                await _supervisor.Completed.ConfigureAwait(false);
            }
        }
    }
}