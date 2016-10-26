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
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.ServiceBus;
    using NewIdFormatters;
    using Settings;
    using Transport;


    /// <summary>
    /// Prepares a queue for receiving messages using the ReceiveSettings specified.
    /// </summary>
    public class PrepareReceiveEndpointFilter :
        IFilter<NamespaceContext>
    {
        static readonly INewIdFormatter _formatter = new ZBase32Formatter();
        readonly ReceiveSettings _settings;
        readonly TopicSubscriptionSettings[] _subscriptionSettings;

        public PrepareReceiveEndpointFilter(ReceiveSettings settings, params TopicSubscriptionSettings[] subscriptionSettings)
        {
            _settings = settings;
            _subscriptionSettings = subscriptionSettings;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        public async Task Send(NamespaceContext context, IPipe<NamespaceContext> next)
        {
            await context.CreateQueue(_settings.QueueDescription).ConfigureAwait(false);

            if (_subscriptionSettings.Length > 0)
            {
                await Task.WhenAll(_subscriptionSettings.Select(subscription => CreateSubscription(context, context.NamespaceManager, subscription)))
                    .ConfigureAwait(false);
            }

            context.GetOrAddPayload(() => _settings);

            await next.Send(context).ConfigureAwait(false);
        }

        async Task CreateSubscription(NamespaceContext context, NamespaceManager namespaceManager, TopicSubscriptionSettings settings)
        {
            var topicDescription = await context.CreateTopic(settings.Topic).ConfigureAwait(false);

            var queuePath = Path.Combine(namespaceManager.Address.AbsoluteUri.TrimStart('/'), _settings.QueueDescription.Path)
                .Replace('\\', '/');

            var subscriptionName = GetSubscriptionName(namespaceManager, _settings.QueueDescription.Path);

            await context.CreateTopicSubscription(subscriptionName, topicDescription.Path, queuePath, _settings.QueueDescription).ConfigureAwait(false);
        }

        static string GetSubscriptionName(NamespaceManager namespaceManager, string queuePath)
        {
            var subscriptionPath = queuePath;

            var suffix = namespaceManager.Address.AbsolutePath.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            if (!string.IsNullOrEmpty(suffix))
                subscriptionPath = $"{queuePath}-{suffix}";

            string name;
            if (subscriptionPath.Length > 50)
            {
                string hashed;
                using (var hasher = new SHA1Managed())
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(subscriptionPath);
                    byte[] hash = hasher.ComputeHash(buffer);
                    hashed = _formatter.Format(hash).Substring(0, 6);
                }

                name = $"{subscriptionPath.Substring(0, 43)}-{hashed}";
            }
            else
                name = subscriptionPath;

            return name;
        }
    }
}