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
namespace MassTransit.AzureServiceBusTransport.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using Contexts;
    using Logging;
    using MassTransit.Pipeline;
    using Microsoft.ServiceBus.Messaging;
    using Util;


    /// <summary>
    /// Creates a message receiver and receives messages from the input queue of the endpoint
    /// </summary>
    public class MessageReceiverFilter :
        IFilter<ConnectionContext>
    {
        static readonly ILog _log = Logger.Get<MessageReceiverFilter>();
        readonly IReceiveEndpointObserver _endpointObserver;
        readonly IReceiveObserver _receiveObserver;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly ITaskSupervisor _supervisor;

        public MessageReceiverFilter(IPipe<ReceiveContext> receivePipe, IReceiveObserver receiveObserver, IReceiveEndpointObserver endpointObserver,
            ITaskSupervisor supervisor)
        {
            _receivePipe = receivePipe;
            _receiveObserver = receiveObserver;
            _endpointObserver = endpointObserver;
            _supervisor = supervisor;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        async Task IFilter<ConnectionContext>.Send(ConnectionContext context, IPipe<ConnectionContext> next)
        {
            var receiveSettings = context.GetPayload<ReceiveSettings>();

            var queuePath = context.GetQueuePath(receiveSettings.QueueDescription);

            var inputAddress = context.GetQueueAddress(receiveSettings.QueueDescription);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Creating message receiver for {0}", inputAddress);

            MessageReceiver messageReceiver = null;

            try
            {
                var messagingFactory = await context.MessagingFactory.ConfigureAwait(false);

                messageReceiver = await messagingFactory.CreateMessageReceiverAsync(queuePath, ReceiveMode.PeekLock).ConfigureAwait(false);

                messageReceiver.PrefetchCount = receiveSettings.PrefetchCount;

                using (var scope = _supervisor.CreateScope($"{TypeMetadataCache<MessageReceiverFilter>.ShortName} - {inputAddress}", () => TaskUtil.Completed))
                {
                    var receiver = new Receiver(context, messageReceiver, inputAddress, _receivePipe, receiveSettings, _receiveObserver, scope);

                    await scope.Ready.ConfigureAwait(false);

                    await _endpointObserver.Ready(new Ready(inputAddress)).ConfigureAwait(false);

                    scope.SetReady();

                    try
                    {
                        await scope.Completed.ConfigureAwait(false);
                    }
                    finally
                    {
                        ReceiverMetrics metrics = receiver;

                        await _endpointObserver.Completed(new Completed(inputAddress, metrics)).ConfigureAwait(false);

                        if (_log.IsDebugEnabled)
                        {
                            _log.DebugFormat("Consumer {0}: {1} received, {2} concurrent", queuePath,
                                metrics.DeliveryCount,
                                metrics.ConcurrentDeliveryCount);
                        }
                    }
                }
            }
            finally
            {
                if (messageReceiver != null && !messageReceiver.IsClosed)
                    await messageReceiver.CloseAsync().ConfigureAwait(false);
            }

            await next.Send(context).ConfigureAwait(false);
        }


        class Ready :
            ReceiveEndpointReady
        {
            public Ready(Uri inputAddress)
            {
                InputAddress = inputAddress;
            }

            public Uri InputAddress { get; }
        }


        class Completed :
            ReceiveEndpointCompleted
        {
            public Completed(Uri inputAddress, ReceiverMetrics metrics)
            {
                InputAddress = inputAddress;
                DeliveryCount = metrics.DeliveryCount;
                ConcurrentDeliveryCount = metrics.ConcurrentDeliveryCount;
            }

            public Uri InputAddress { get; }
            public long DeliveryCount { get; }
            public long ConcurrentDeliveryCount { get; }
        }
    }
}