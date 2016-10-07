// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes;
    using Logging;
    using Util;


    /// <summary>
    /// Creates a message session receiver
    /// </summary>
    public class MessageSessionReceiverFilter :
        IFilter<NamespaceContext>
    {
        static readonly ILog _log = Logger.Get<MessageReceiverFilter>();
        readonly IPipe<ReceiveContext> _receivePipe;

        public MessageSessionReceiverFilter(IPipe<ReceiveContext> receivePipe)
        {
            _receivePipe = receivePipe;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        async Task IFilter<NamespaceContext>.Send(NamespaceContext context, IPipe<NamespaceContext> next)
        {
            var clientContext = context.GetPayload<ClientContext>();

            var clientSettings = context.GetPayload<ClientSettings>();

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Creating message receiver for {0}", clientContext.InputAddress);

            using (var scope = context.CreateScope($"{TypeMetadataCache<MessageReceiverFilter>.ShortName} - {clientContext.InputAddress}"))
            {
                var receiver = new SessionReceiver(context, clientContext, _receivePipe, clientSettings, scope);

                await scope.Ready.ConfigureAwait(false);

                await context.Ready(new Ready(clientContext.InputAddress)).ConfigureAwait(false);

                scope.SetReady();

                try
                {
                    await scope.Completed.ConfigureAwait(false);
                }
                finally
                {
                    ReceiverMetrics metrics = receiver;

                    await context.Completed(new Completed(clientContext.InputAddress, metrics)).ConfigureAwait(false);

                    if (_log.IsDebugEnabled)
                    {
                        _log.DebugFormat("Consumer {0}: {1} received, {2} concurrent", clientContext.InputAddress,
                            metrics.DeliveryCount,
                            metrics.ConcurrentDeliveryCount);
                    }
                }
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