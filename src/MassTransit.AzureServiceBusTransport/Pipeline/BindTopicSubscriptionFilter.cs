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
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration;
    using Contexts;
    using Logging;
    using MassTransit.Pipeline;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;


    /// <summary>
    /// Binds the topic subscriptions
    /// </summary>
    public class BindTopicSubscriptionFilter :
        IFilter<ConnectionContext>
    {
        readonly ILog _log = Logger.Get<BindTopicSubscriptionFilter>();
        readonly TopicSubscriptionSettings _settings;

        public BindTopicSubscriptionFilter(TopicSubscriptionSettings settings)
        {
            _settings = settings;
        }

        public async Task Send(ConnectionContext context, IPipe<ConnectionContext> next)
        {
            var receiveSettings = context.GetPayload<ReceiveSettings>();

            SubscriptionDescription subscriptionDescription = null;

            NamespaceManager rootNamespaceManager = await context.RootNamespaceManager;

            var namespaceManager = await context.NamespaceManager;

            await GetOrAddTopic(rootNamespaceManager);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Creating subscription {0}", _settings.Topic.Path);

            string name = string.Format("{0}-{1}", (uint)receiveSettings.QueueDescription.Path.GetHashCode(), (uint)_settings.Topic.Path.GetHashCode());

            try
            {
                string queuePath = Path.Combine(namespaceManager.Address.AbsolutePath.TrimStart(new[]{'/'}), receiveSettings.QueueDescription.Path);
                var description = new SubscriptionDescription(_settings.Topic.Path, name)
                {
                    ForwardTo = queuePath,
                };

                subscriptionDescription = await rootNamespaceManager.CreateSubscriptionAsync(description);
            }
            catch (MessagingEntityAlreadyExistsException)
            {
            }
            if (subscriptionDescription == null)
                subscriptionDescription = await rootNamespaceManager.GetSubscriptionAsync(_settings.Topic.Path, name);

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Subscription: {0} ({1} -> {2})", subscriptionDescription.Name, subscriptionDescription.TopicPath,
                    subscriptionDescription.ForwardTo);
            }

            await next.Send(context);
        }

        public bool Visit(IPipeVisitor visitor)
        {
            return visitor.Visit(this);
        }

        async Task GetOrAddTopic(NamespaceManager namespaceManager)
        {
            TopicDescription topicDescription = null;

            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Creating topic {0}", _settings.Topic.Path);

                topicDescription = await namespaceManager.CreateTopicAsync(_settings.Topic);
            }
            catch (MessagingEntityAlreadyExistsException)
            {
            }
            if (topicDescription == null)
                topicDescription = await namespaceManager.GetTopicAsync(_settings.Topic.Path);

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Topic: {0} ({1})", topicDescription.Path,
                    string.Join(", ", new[]
                    {
                        topicDescription.EnableExpress ? "express" : "",
                        topicDescription.RequiresDuplicateDetection ? "dupe detect" : "",
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));
            }
        }
    }
}