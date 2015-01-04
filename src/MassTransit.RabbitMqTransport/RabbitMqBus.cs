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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using MassTransit.Pipeline;
    using Transports;


    public class RabbitMqBus :
        IBusControl
    {
        readonly Uri _address;
        readonly IConsumePipe _consumePipe;
        readonly IPublishEndpoint _publishEndpoint;
        readonly IList<IReceiveEndpoint> _receiveEndpoints;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly RabbitMqHost[] _hosts;

        public RabbitMqBus(Uri address, IConsumePipe consumePipe, ISendEndpointProvider sendEndpointProvider, IEnumerable<IReceiveEndpoint> receiveEndpoints, RabbitMqHost[] hosts, IPublishEndpoint publishEndpoint)
        {
            _address = address;
            _consumePipe = consumePipe;
            _sendEndpointProvider = sendEndpointProvider;
            _hosts = hosts;
            _receiveEndpoints = receiveEndpoints.ToList();
            _publishEndpoint = publishEndpoint;
        }

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, messageType, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, messageType, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish<T>(values, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(values, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish<T>(values, publishPipe, cancellationToken);
        }

        public Uri Address
        {
            get { return _address; }
        }

        IConsumePipe IBus.ConsumePipe
        {
            get { return _consumePipe; }
        }

        Task<ISendEndpoint> ISendEndpointProvider.GetSendEndpoint(Uri address)
        {
            return _sendEndpointProvider.GetSendEndpoint(address);
        }

        public async Task<BusHandle> Start(CancellationToken cancellationToken)
        {
            var receiveEndpointHandles = new List<ReceiveEndpointHandle>();

            Exception exception = null;
            try
            {
                Task<ReceiveEndpointHandle>[] receiveEndpoints = _receiveEndpoints.Select(x => x.Start(cancellationToken)).ToArray();
                foreach (var endpoint in receiveEndpoints)
                {
                    try
                    {
                        ReceiveEndpointHandle handle = await endpoint;

                        receiveEndpointHandles.Add(handle);
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                await Task.WhenAll(receiveEndpointHandles.Select(x => x.Stop(cancellationToken)));

                throw new MassTransitException("The bus could not be started.", exception);
            }

            return new Handle(this, receiveEndpointHandles.ToArray());
        }


        class Handle :
            BusHandle
        {
            readonly RabbitMqBus _bus;
            readonly ReceiveEndpointHandle[] _receiveEndpoints;

            public Handle(RabbitMqBus bus, ReceiveEndpointHandle[] receiveEndpoints)
            {
                _bus = bus;
                _receiveEndpoints = receiveEndpoints;
            }

            public void Dispose()
            {
                Task.WaitAll(_receiveEndpoints.Select(x => x.Stop()).ToArray());
                Task.WaitAll(_bus._hosts.Select(x => x.ConnectionCache.Stop()).ToArray());
                Task.WaitAll(_bus._hosts.Select(x => x.SendConnectionCache.Stop()).ToArray());
            }

            async Task BusHandle.Stop(CancellationToken cancellationToken)
            {
                await Task.WhenAll(_receiveEndpoints.Select(x => x.Stop(cancellationToken)));
                await Task.WhenAll(_bus._hosts.Select(x => x.ConnectionCache.Stop()));
                await Task.WhenAll(_bus._hosts.Select(x => x.SendConnectionCache.Stop()));
            }
        }
    }
}