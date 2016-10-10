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
    using System.Threading.Tasks;
    using Contexts;
    using Events;
    using GreenPipes;
    using Logging;
    using Transport;
    using Transports;
    using Util;


    /// <summary>
    /// Creates a message receiver and receives messages from the input queue of the endpoint
    /// </summary>
    public class MessageReceiverFilter :
        IFilter<NamespaceContext>
    {
        static readonly ILog _log = Logger.Get<MessageReceiverFilter>();
        readonly IPipe<ReceiveContext> _receivePipe;

        public MessageReceiverFilter(IPipe<ReceiveContext> receivePipe)
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
                var receiver = new Receiver(context, clientContext, _receivePipe, clientSettings, scope);

                await scope.Ready.ConfigureAwait(false);

                await context.Ready(new ReceiveEndpointReadyEvent(clientContext.InputAddress)).ConfigureAwait(false);

                scope.SetReady();

                try
                {
                    await scope.Completed.ConfigureAwait(false);
                }
                finally
                {
                    DeliveryMetrics metrics = receiver.GetDeliveryMetrics();

                    await context.Completed(new ReceiveEndpointCompletedEvent(clientContext.InputAddress, metrics)).ConfigureAwait(false);

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
    }
}