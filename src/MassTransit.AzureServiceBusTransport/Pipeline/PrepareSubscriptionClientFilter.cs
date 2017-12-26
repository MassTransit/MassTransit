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


    public class PrepareSubscriptionClientFilter :
        IFilter<NamespaceContext>
    {
        static readonly ILog _log = Logger.Get<PrepareQueueClientFilter>();
        readonly SubscriptionSettings _settings;

        public PrepareSubscriptionClientFilter(SubscriptionSettings settings)
        {
            _settings = settings;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        async Task IFilter<NamespaceContext>.Send(NamespaceContext context, IPipe<NamespaceContext> next)
        {
            var inputAddress = _settings.GetInputAddress(context.ServiceAddress, _settings.Path);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Creating subscription client for {0}", inputAddress);

            SubscriptionClient subscriptionClient = null;

            try
            {
                var messagingFactory = _settings.RequiresSession
                    ? await context.SessionMessagingFactory.ConfigureAwait(false)
                    : await context.MessagingFactory.ConfigureAwait(false);

                subscriptionClient = messagingFactory.CreateSubscriptionClient(_settings.TopicDescription.Path, _settings.SubscriptionDescription.Name);

                subscriptionClient.PrefetchCount = _settings.PrefetchCount;

                ClientContext clientContext = new SubscriptionClientContext(subscriptionClient, inputAddress);

                context.GetOrAddPayload(() => clientContext);

                await next.Send(context).ConfigureAwait(false);
            }
            finally
            {
                if (subscriptionClient != null && !subscriptionClient.IsClosed)
                    await subscriptionClient.CloseAsync().ConfigureAwait(false);
            }
        }
    }
}