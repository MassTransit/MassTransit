// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Events;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Logging;
    using MassTransit.Pipeline.Observables;
    using MassTransit.Pipeline.Pipes;
    using Policies;
    using Topology;
    using Transports;
    using Util;


    public class RabbitMqReceiveTransport :
        IReceiveTransport
    {
        static readonly ILog _log = Logger.Get<RabbitMqReceiveTransport>();
        readonly IRabbitMqHost _host;
        readonly Uri _inputAddress;
        readonly IManagementPipe _managementPipe;
        readonly ReceiveObservable _receiveObservable;
        readonly ReceiveTransportObservable _receiveTransportObservable;
        readonly ReceiveSettings _settings;
        readonly IRabbitMqReceiveEndpointTopology _topology;

        public RabbitMqReceiveTransport(IRabbitMqHost host, ReceiveSettings settings, IManagementPipe managementPipe, IRabbitMqReceiveEndpointTopology topology)
        {
            _host = host;
            _settings = settings;
            _topology = topology;
            _managementPipe = managementPipe;

            _receiveObservable = new ReceiveObservable();
            _receiveTransportObservable = new ReceiveTransportObservable();

            _inputAddress = topology.InputAddress;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("transport");
            scope.Add("type", "RabbitMQ");
            scope.Set(_settings);
            var topologyScope = scope.CreateScope("topology");
            _topology.BrokerTopology.Probe(topologyScope);
        }

        /// <summary>
        /// Start the receive transport, returning a Task that can be awaited to signal the transport has 
        /// completely shutdown once the cancellation token is cancelled.
        /// </summary>
        /// <param name="receivePipe"></param>
        /// <returns>A task that is completed once the transport is shut down</returns>
        public ReceiveTransportHandle Start(IPipe<ReceiveContext> receivePipe)
        {
            var supervisor = new TaskSupervisor($"{TypeCache<RabbitMqReceiveTransport>.ShortName} - {_inputAddress}");

            IPipe<ConnectionContext> pipe = Pipe.New<ConnectionContext>(x =>
            {
                x.RabbitMqConsumer(receivePipe, _settings, _receiveObservable, _receiveTransportObservable, supervisor, _managementPipe, _host, _topology);
            });

            Task.Factory.StartNew(() => Receiver(pipe, supervisor), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

            return new Handle(supervisor);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveObservable.Connect(observer);
        }

        public ConnectHandle ConnectReceiveTransportObserver(IReceiveTransportObserver observer)
        {
            return _receiveTransportObservable.Connect(observer);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _topology.PublishEndpointProvider.ConnectPublishObserver(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            var sendHandle = _topology.SendEndpointProvider.ConnectSendObserver(observer);
            var publishHandle = _topology.PublishEndpointProvider.ConnectSendObserver(observer);

            return new MultipleConnectHandle(sendHandle, publishHandle);
        }

        async Task Receiver(IPipe<ConnectionContext> transportPipe, TaskSupervisor supervisor)
        {
            try
            {
                await _host.ConnectionRetryPolicy.RetryUntilCancelled(async () =>
                {
                    try
                    {
                        await _host.ConnectionCache.Send(transportPipe, supervisor.StoppingToken)
                            .ConfigureAwait(false);
                    }
                    catch (RabbitMqConnectionException ex)
                    {
                        if (_log.IsErrorEnabled)
                            _log.ErrorFormat("RabbitMQ connection failed: {0}", ex.Message);

                        await _receiveTransportObservable.Faulted(new ReceiveTransportFaultedEvent(_inputAddress, ex)).ConfigureAwait(false);

                        throw;
                    }
                    catch (TaskCanceledException)
                    {
                    }
                    catch (Exception ex)
                    {
                        if (_log.IsErrorEnabled)
                            _log.ErrorFormat("RabbitMQ receive transport failed: {0}", ex.Message);

                        await _receiveTransportObservable.Faulted(new ReceiveTransportFaultedEvent(_inputAddress, ex)).ConfigureAwait(false);

                        throw;
                    }
                }, supervisor.StoppingToken).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
            }
        }


        class Handle :
            ReceiveTransportHandle
        {
            readonly TaskSupervisor _supervisor;

            public Handle(TaskSupervisor supervisor)
            {
                _supervisor = supervisor;
            }

            Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                return _supervisor.Stop("Stop Receive Transport", cancellationToken);
            }
        }
    }
}