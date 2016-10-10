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
    using GreenPipes;


    /// <summary>
    /// Prepares a queue for receiving messages using the ReceiveSettings specified.
    /// </summary>
    public class PrepareSubscriptionEndpointFilter :
        IFilter<NamespaceContext>
    {
        readonly SubscriptionSettings _settings;

        public PrepareSubscriptionEndpointFilter(SubscriptionSettings settings)
        {
            _settings = settings;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        public async Task Send(NamespaceContext context, IPipe<NamespaceContext> next)
        {
            await CreateSubscription(context).ConfigureAwait(false);

            context.GetOrAddPayload(() => _settings);

            await next.Send(context).ConfigureAwait(false);
        }

        async Task CreateSubscription(NamespaceContext context)
        {
            await context.CreateTopic(_settings.TopicDescription).ConfigureAwait(false);

            await context.CreateTopicSubscription(_settings.SubscriptionDescription).ConfigureAwait(false);
        }
    }
}