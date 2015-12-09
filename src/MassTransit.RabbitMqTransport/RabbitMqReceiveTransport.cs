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
    using Management;
    using MassTransit.Pipeline;
    using Policies;
    using Topology;
    using Transports;
    using Util;


    public class RabbitMqReceiveTransport :
        IReceiveTransport
    {
        static readonly ILog _log = Logger.Get<RabbitMqReceiveTransport>();
        readonly ExchangeBindingSettings[] _bindings;
        readonly IRetryPolicy _connectionRetryPolicy;
        readonly IRabbitMqHost _host;
        readonly Mediator<ISetPrefetchCount> _mediator;
        readonly ReceiveEndpointObservable _receiveEndpointObservable;
        readonly ReceiveObservable _receiveObservable;
        readonly ReceiveSettings _settings;

        public RabbitMqReceiveTransport(IRabbitMqHost host, ReceiveSettings settings, Mediator<ISetPrefetchCount> mediator,
            params ExchangeBindingSettings[] bindings)
        {
            _host = host;
            _settings = settings;
            _bindings = bindings;
            _mediator = mediator;

            _receiveObservable = new ReceiveObservable();
            _receiveEndpointObservable = new ReceiveEndpointObservable();

            var exceptionFilter = Retry.Selected<RabbitMqConnectionException>();

            _connectionRetryPolicy = exceptionFilter.Exponential(1000, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(1));
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("transport");
            scope.Add("type", "RabbitMQ");
            scope.Set(_settings);
            scope.Add("bindings", _bindings);
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

            IPipe<ConnectionContext> pipe = Pipe.New<ConnectionContext>(x =>
            {
                x.RabbitMqConsumer(receivePipe, _settings, _receiveObservable, _receiveEndpointObservable, _bindings, supervisor, _mediator);
            });

            Receiver(pipe, supervisor);

            return new Handle(supervisor);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveObservable.Connect(observer);
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _receiveEndpointObservable.Connect(observer);
        }

        async void Receiver(IPipe<ConnectionContext> transportPipe, TaskSupervisor supervisor)
        {
            try
            {
                await Repeat.UntilCancelled(supervisor.StopToken, () => _connectionRetryPolicy.Retry(async () =>
                {
                    try
                    {
                        await _host.ConnectionCache.Send(transportPipe, supervisor.StopToken)
                            .ConfigureAwait(false);
                    }
                    catch (RabbitMqConnectionException ex)
                    {
                        if (_log.IsErrorEnabled)
                            _log.ErrorFormat("RabbitMQ connection failed: {0}", ex.Message);

                        var inputAddress = _host.Settings.GetInputAddress(_settings);

                        await _receiveEndpointObservable.Faulted(new Faulted(inputAddress, ex)).ConfigureAwait(false);
                    }
                    catch (TaskCanceledException)
                    {
                    }
                    catch (Exception ex)
                    {
                        if (_log.IsErrorEnabled)
                            _log.ErrorFormat("RabbitMQ receive transport failed: {0}", ex.Message);

                        var inputAddress = _host.Settings.GetInputAddress(_settings);

                        await _receiveEndpointObservable.Faulted(new Faulted(inputAddress, ex)).ConfigureAwait(false);
                    }
                }));
            }
            catch (TaskCanceledException)
            {
            }
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