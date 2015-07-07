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
namespace RapidTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Logging;
    using MassTransit.Monitoring.Introspection;
    using MassTransit.Pipeline;
    using MassTransit.RabbitMqTransport;


    /// <summary>
    /// The host service bus is a per-service bus instance that is created and added to the 
    /// root container. It can be used by any services for service-level event notifications, such as
    /// cache item expirations that need to be received by every service instance on every server running
    /// the service
    /// </summary>
    public class HostBus :
        IHostBus
    {
        readonly IBusControl _bus;
        readonly ILog _log = Logger.Get<HostBus>();

        public HostBus(RabbitMqHostSettings hostSettings, HostBusSettings settings)
        {
            _log.DebugFormat("Creating host bus on queue: {0}", settings.QueueName);

            _bus = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                x.Host(hostSettings);

//                transportConfigurator.Configure(settings.QueueName, 1);
            });

            _log.DebugFormat("HostBus created: {0}", _bus.Address);
        }

        ConnectHandle IConsumePipeConnector.ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            return _bus.ConnectConsumePipe(pipe);
        }

        ConnectHandle IRequestPipeConnector.ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
        {
            return _bus.ConnectRequestPipe(requestId, pipe);
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return _bus.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _bus.ConnectConsumeObserver(observer);
        }

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            return _bus.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            return _bus.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _bus.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            return _bus.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _bus.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            return _bus.Publish(message, messageType, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            return _bus.Publish(message, messageType, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            return _bus.Publish<T>(values, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            return _bus.Publish(values, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _bus.Publish<T>(values, publishPipe, cancellationToken);
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _bus.GetSendEndpoint(address);
        }

        public Uri Address
        {
            get { return _bus.Address; }
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _bus.ConnectReceiveObserver(observer);
        }

        public ConnectHandle Connect(IPublishObserver observer)
        {
            return _bus.Connect(observer);
        }

        public async void Probe(ProbeContext context)
        {
            
        }
    }
}