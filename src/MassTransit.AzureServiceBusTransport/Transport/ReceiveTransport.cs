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
namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;
    using Pipeline;
    using Policies;
    using Transports;


    public class ReceiveTransport :
        Supervisor,
        IReceiveTransport
    {
        readonly IClientContextSupervisor _clientContextSupervisor;
        readonly IPipe<ClientContext> _clientPipe;
        readonly ServiceBusReceiveEndpointContext _context;
        readonly IServiceBusHost _host;
        readonly ClientSettings _settings;

        public ReceiveTransport(IServiceBusHost host, ClientSettings settings, IClientContextSupervisor clientContextSupervisor,
            IPipe<ClientContext> clientPipe,
            ServiceBusReceiveEndpointContext context)
        {
            _host = host;
            _settings = settings;
            _clientContextSupervisor = clientContextSupervisor;
            _clientPipe = clientPipe;
            _context = context;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("transport");
            scope.Set(new
            {
                Type = "Azure Service Bus",
                _settings.Path,
                _settings.PrefetchCount,
                _settings.MaxConcurrentCalls
            });
        }

        public ReceiveTransportHandle Start()
        {
            Task.Factory.StartNew(Receiver, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

            return new Handle(this);
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _context.ConnectReceiveObserver(observer);
        }

        ConnectHandle IReceiveTransportObserverConnector.ConnectReceiveTransportObserver(IReceiveTransportObserver observer)
        {
            return _context.ConnectReceiveTransportObserver(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _context.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }

        async Task Receiver()
        {
            var inputAddress = _settings.GetInputAddress(_host.Settings.ServiceUri, _settings.Path);

            while (!IsStopping)
            {
                try
                {
                    await _host.RetryPolicy.Retry(async () =>
                    {
                        try
                        {
                            await _clientContextSupervisor.Send(_clientPipe, Stopped).ConfigureAwait(false);
                        }
                        catch (OperationCanceledException)
                        {
                        }
                        catch (Exception ex)
                        {
                            LogContext.Error?.Log(ex, "Receive transport faulted: {InputAddress}", _context.InputAddress);

                            await _context.TransportObservers.Faulted(new ReceiveTransportFaultedEvent(inputAddress, ex)).ConfigureAwait(false);

                            throw;
                        }
                    }, Stopping).ConfigureAwait(false);
                }
                catch
                {
                    // i said, nothing to see here
                }
            }
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
