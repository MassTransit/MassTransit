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
namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Events;
    using Exceptions;
    using GreenPipes;
    using GreenPipes.Agents;
    using Logging;
    using MassTransit.Pipeline.Observables;
    using Policies;
    using Topology;
    using Transports;


    public class AmazonSqsReceiveTransport :
        Supervisor,
        IReceiveTransport
    {
        static readonly ILog _log = Logger.Get<AmazonSqsReceiveTransport>();
        readonly IPipe<ConnectionContext> _connectionPipe;
        readonly IAmazonSqsHost _host;
        readonly Uri _inputAddress;
        readonly ReceiveObservable _receiveObservable;
        readonly ReceiveTransportObservable _receiveTransportObservable;
        readonly ReceiveSettings _settings;
        readonly AmazonSqsReceiveEndpointContext _context;

        public AmazonSqsReceiveTransport(IAmazonSqsHost host, ReceiveSettings settings, IPipe<ConnectionContext> connectionPipe,
            AmazonSqsReceiveEndpointContext context, ReceiveObservable receiveObservable, ReceiveTransportObservable receiveTransportObservable)
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
            scope.Add("type", "AmazonSQS");
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
                try
                {
                    await _host.ConnectionRetryPolicy.Retry(async () =>
                    {
                        try
                        {
                            await _host.ConnectionCache.Send(_connectionPipe, Stopped).ConfigureAwait(false);
                        }
                        catch (OperationCanceledException)
                        {
                        }
                        catch (Exception ex)
                        {
                            throw await ConvertToAmazonSqsConnectionException(ex, "ReceiveTransport Faulted, Restarting ").ConfigureAwait(false);
                        }
                    }, Stopping);
                }
                catch
                {
                    // seriously, nothing to see here
                }
            }
        }

        async Task<AmazonSqsConnectException> ConvertToAmazonSqsConnectionException(Exception ex, string message)
        {
            if (_log.IsDebugEnabled)
                _log.Debug(message, ex);

            var exception = new AmazonSqsConnectException(message + _host.ConnectionCache, ex);

            await NotifyFaulted(exception);

            return exception;
        }

        Task NotifyFaulted(Exception exception)
        {
            if (_log.IsErrorEnabled)
                _log.ErrorFormat("AmazonSQS Connect Failed: {0}", exception.Message);

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
