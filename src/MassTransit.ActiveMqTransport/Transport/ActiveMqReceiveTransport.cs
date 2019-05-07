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
namespace MassTransit.ActiveMqTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Contexts;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;
    using Logging;
    using Microsoft.Extensions.Logging;
    using Policies;
    using Topology;
    using Transports;


    public class ActiveMqReceiveTransport :
        Supervisor,
        IReceiveTransport
    {
        static readonly ILogger _logger = Logger.Get<ActiveMqReceiveTransport>();
        readonly IPipe<ConnectionContext> _connectionPipe;
        readonly IActiveMqHost _host;
        readonly Uri _inputAddress;
        readonly ReceiveSettings _settings;
        readonly ActiveMqReceiveEndpointContext _receiveEndpointContext;

        public ActiveMqReceiveTransport(IActiveMqHost host, ReceiveSettings settings, IPipe<ConnectionContext> connectionPipe,
            ActiveMqReceiveEndpointContext receiveEndpointContext)
        {
            _host = host;
            _settings = settings;
            _receiveEndpointContext = receiveEndpointContext;
            _connectionPipe = connectionPipe;

            _inputAddress = receiveEndpointContext.InputAddress;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("transport");
            scope.Add("type", "ActiveMQ");
            scope.Set(_settings);
            var topologyScope = scope.CreateScope("topology");
            _receiveEndpointContext.BrokerTopology.Probe(topologyScope);
        }

        /// <summary>
        /// Start the receive transport, returning a Task that can be awaited to signal the transport has
        /// completely shutdown once the cancellation token is cancelled.
        /// </summary>
        /// <returns>A task that is completed once the transport is shut down</returns>
        public ReceiveTransportHandle Start()
        {
            Task.Factory.StartNew(Receiver, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

            return new Handle(this);
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveEndpointContext.ConnectReceiveObserver(observer);
        }

        ConnectHandle IReceiveTransportObserverConnector.ConnectReceiveTransportObserver(IReceiveTransportObserver observer)
        {
            return _receiveEndpointContext.ConnectReceiveTransportObserver(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _receiveEndpointContext.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _receiveEndpointContext.ConnectSendObserver(observer);
        }

        async Task Receiver()
        {
            while (!IsStopping)
            {
                try
                {
                    await _host.ConnectionRetryPolicy.Retry(async () =>
                    {
                        try
                        {
                            await _host.ConnectionContextSupervisor.Send(_connectionPipe, Stopped).ConfigureAwait(false);
                        }
                        catch (NMSConnectionException ex)
                        {
                            throw await ConvertToActiveMqConnectionException(ex, "NMSConnectionException").ConfigureAwait(false);
                        }
                        catch (OperationCanceledException)
                        {
                        }
                        catch (Exception ex)
                        {
                            throw await ConvertToActiveMqConnectionException(ex, "ReceiveTransport Faulted, Restarting").ConfigureAwait(false);
                        }
                    }, Stopping).ConfigureAwait(false);
                }
                catch
                {
                    // seriously, nothing to see here
                }
            }
        }

        async Task<ActiveMqConnectException> ConvertToActiveMqConnectionException(Exception ex, string message)
        {
            _logger.LogDebug(message, ex);

            var exception = new ActiveMqConnectException(message + _host.ConnectionContextSupervisor, ex);

            await NotifyFaulted(exception).ConfigureAwait(false);

            return exception;
        }

        Task NotifyFaulted(Exception exception)
        {
            _logger.LogError("ActiveMQ Connect Failed: {0}", exception.Message);

            return _receiveEndpointContext.TransportObservers.Faulted(new ReceiveTransportFaultedEvent(_inputAddress, exception));
        }


        class Handle :
            ReceiveTransportHandle
        {
            readonly IAgent _agent;

            public Handle(IAgent agent)
            {
                _agent = agent;
            }

            Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                return _agent.Stop("Stop Receive Transport", cancellationToken);
            }
        }
    }
}
