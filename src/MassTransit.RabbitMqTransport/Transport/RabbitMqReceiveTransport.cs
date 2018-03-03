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
namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;
    using Logging;
    using MassTransit.Pipeline.Observables;
    using Policies;
    using RabbitMQ.Client.Exceptions;
    using Topology;
    using Transports;


    public class RabbitMqReceiveTransport :
        Supervisor,
        IReceiveTransport
    {
        static readonly ILog _log = Logger.Get<RabbitMqReceiveTransport>();
        readonly IPipe<ConnectionContext> _connectionPipe;
        readonly IRabbitMqHost _host;
        readonly Uri _inputAddress;
        readonly ReceiveObservable _receiveObservable;
        readonly ReceiveTransportObservable _receiveTransportObservable;
        readonly ReceiveSettings _settings;
        readonly RabbitMqReceiveEndpointContext _receiveEndpointContext;

        public RabbitMqReceiveTransport(IRabbitMqHost host, ReceiveSettings settings, IPipe<ConnectionContext> connectionPipe,
            RabbitMqReceiveEndpointContext receiveEndpointContext, ReceiveObservable receiveObservable, ReceiveTransportObservable receiveTransportObservable)
        {
            _host = host;
            _settings = settings;
            _receiveEndpointContext = receiveEndpointContext;
            _connectionPipe = connectionPipe;

            _receiveObservable = receiveObservable;
            _receiveTransportObservable = receiveTransportObservable;

            _inputAddress = receiveEndpointContext.InputAddress;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("transport");
            scope.Add("type", "RabbitMQ");
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
            return _receiveEndpointContext.ConnectPublishObserver(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _receiveEndpointContext.ConnectSendObserver(observer);
        }

        async Task Receiver()
        {
            while (!IsStopping)
            {
                await _host.ConnectionRetryPolicy.Retry(async () =>
                {
                    if (IsStopping)
                        return;

                    try
                    {
                        await _host.ConnectionCache.Send(_connectionPipe, Stopped).ConfigureAwait(false);
                    }
                    catch (RabbitMqConnectionException ex)
                    {
                        await NotifyFaulted(ex).ConfigureAwait(false);
                        throw;
                    }
                    catch (BrokerUnreachableException ex)
                    {
                        await ConvertToRabbitMqConnectionException(ex, "RabbitMQ Unreachable").ConfigureAwait(false);
                        throw;
                    }
                    catch (OperationInterruptedException ex)
                    {
                        await ConvertToRabbitMqConnectionException(ex, "Operation interrupted").ConfigureAwait(false);
                        throw;
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception ex)
                    {
                        await ConvertToRabbitMqConnectionException(ex, "ReceiveTranport Faulted, Restarting").ConfigureAwait(false);
                        throw;
                    }
                }, Stopping);
            }
        }

        async Task<RabbitMqConnectionException> ConvertToRabbitMqConnectionException(Exception ex, string message)
        {
            if (_log.IsDebugEnabled)
                _log.Debug(message, ex);

            var exception = new RabbitMqConnectionException(message + _host.ConnectionCache, ex);

            await NotifyFaulted(exception);

            return exception;
        }

        Task NotifyFaulted(RabbitMqConnectionException exception)
        {
            if (_log.IsErrorEnabled)
                _log.ErrorFormat("RabbitMQ Connect Failed: {0}", exception.Message);

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