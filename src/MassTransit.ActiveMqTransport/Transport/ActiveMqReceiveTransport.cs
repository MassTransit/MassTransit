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
    using MassTransit.Pipeline.Observables;
    using Policies;
    using Topology;
    using Transports;


    public class ActiveMqReceiveTransport :
        Supervisor,
        IReceiveTransport
    {
        static readonly ILog _log = Logger.Get<ActiveMqReceiveTransport>();
        readonly IPipe<ConnectionContext> _connectionPipe;
        readonly IActiveMqHost _host;
        readonly Uri _inputAddress;
        readonly ReceiveObservable _receiveObservable;
        readonly ReceiveTransportObservable _receiveTransportObservable;
        readonly ReceiveSettings _settings;
        readonly ActiveMqReceiveEndpointContext _context;

        public ActiveMqReceiveTransport(IActiveMqHost host, ReceiveSettings settings, IPipe<ConnectionContext> connectionPipe,
            ActiveMqReceiveEndpointContext context, ReceiveObservable receiveObservable, ReceiveTransportObservable receiveTransportObservable)
        {
            _host = host;
            _settings = settings;
            _context = context;
            _connectionPipe = connectionPipe;

            _receiveObservable = receiveObservable;
            _receiveTransportObservable = receiveTransportObservable;

            _inputAddress = context.InputAddress;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("transport");
            scope.Add("type", "RabbitMQ");
            scope.Set(_settings);
            var topologyScope = scope.CreateScope("topology");
            _context.BrokerTopology.Probe(topologyScope);
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
            return _context.ConnectPublishObserver(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }

        async Task Receiver()
        {
            while (!IsStopping)
            {
                await _host.ConnectionRetryPolicy.Retry(async () =>
                {
                    try
                    {
                        await _host.ConnectionCache.Send(_connectionPipe, Stopped).ConfigureAwait(false);
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
                        throw await ConvertToActiveMqConnectionException(ex, "ReceiveTranport Faulted, Restarting").ConfigureAwait(false);
                    }
                }, Stopping);
            }
        }

        async Task<ActiveMqConnectException> ConvertToActiveMqConnectionException(Exception ex, string message)
        {
            if (_log.IsDebugEnabled)
                _log.Debug(message, ex);

            var exception = new ActiveMqConnectException(message + _host.ConnectionCache, ex);

            await NotifyFaulted(exception);

            return exception;
        }

        Task NotifyFaulted(Exception exception)
        {
            if (_log.IsErrorEnabled)
                _log.ErrorFormat("ActiveMQ Connect Failed: {0}", exception.Message);

            return _receiveTransportObservable.Faulted(new ReceiveTransportFaultedEvent(_inputAddress, exception));
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