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
    using GreenPipes;
    using Logging;
    using Microsoft.ServiceBus.Messaging;
    using Transport;


    public class PrepareQueueClientFilter :
        IFilter<NamespaceContext>
    {
        static readonly ILog _log = Logger.Get<PrepareQueueClientFilter>();
        readonly ReceiveSettings _settings;

        public PrepareQueueClientFilter(ReceiveSettings settings)
        {
            _settings = settings;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        async Task IFilter<NamespaceContext>.Send(NamespaceContext context, IPipe<NamespaceContext> next)
        {
            var queuePath = context.GetQueuePath(_settings.QueueDescription);

            var inputAddress = _settings.GetInputAddress(context.ServiceAddress, _settings.Path);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Creating queue client for {0}", inputAddress);

            QueueClient queueClient = null;

            try
            {
                var messagingFactory = _settings.RequiresSession
                    ? await context.SessionMessagingFactory.ConfigureAwait(false)
                    : await context.MessagingFactory.ConfigureAwait(false);

                queueClient = messagingFactory.CreateQueueClient(queuePath);

                queueClient.PrefetchCount = _settings.PrefetchCount;

                ClientContext clientContext = new QueueClientContext(queueClient, inputAddress);

                context.GetOrAddPayload(() => clientContext);

                await next.Send(context).ConfigureAwait(false);
            }
            finally
            {
                if (queueClient != null && !queueClient.IsClosed)
                    await queueClient.CloseAsync().ConfigureAwait(false);
            }
        }
    }
}