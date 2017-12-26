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
namespace MassTransit.AzureServiceBusTransport
{
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration;
    using Logging;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;


    public static class NamespaceManagerExtensions
    {
        static readonly ILog _log = Logger.Get<ServiceBusHost>();

        public static async Task<QueueDescription> CreateQueueSafeAsync(this NamespaceManager namespaceManager, QueueDescription queueDescription)
        {
            var create = true;
            try
            {
                queueDescription = await namespaceManager.GetQueueAsync(queueDescription.Path).ConfigureAwait(false);

                create = false;
            }
            catch (MessagingEntityNotFoundException)
            {
            }

            if (create)
            {
                var created = false;
                try
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Creating queue {0}", queueDescription.Path);

                    queueDescription = await namespaceManager.CreateQueueAsync(queueDescription).ConfigureAwait(false);

                    created = true;
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                }
                catch (MessagingException mex)
                {
                    if (mex.Message.Contains("(409)"))
                    {
                    }
                    else
                        throw;
                }

                if (!created)
                    queueDescription = await namespaceManager.GetQueueAsync(queueDescription.Path).ConfigureAwait(false);
            }

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Queue: {0} ({1})", queueDescription.Path,
                    string.Join(", ", new[]
                    {
                        queueDescription.EnableExpress ? "express" : "",
                        queueDescription.RequiresDuplicateDetection ? "dupe detect" : "",
                        queueDescription.EnableDeadLetteringOnMessageExpiration ? "dead letter" : "",
                        queueDescription.RequiresSession ? "session" : ""
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));
            }

            return queueDescription;
        }

        public static async Task<TopicDescription> CreateTopicSafeAsync(this NamespaceManager namespaceManager, TopicDescription topicDescription)
        {
            var create = true;
            try
            {
                topicDescription = await namespaceManager.GetTopicAsync(topicDescription.Path).ConfigureAwait(false);

                create = false;
            }
            catch (MessagingEntityNotFoundException)
            {
            }

            if (create)
            {
                var created = false;
                try
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Creating topic {0}", topicDescription.Path);

                    topicDescription = await namespaceManager.CreateTopicAsync(topicDescription).ConfigureAwait(false);

                    created = true;
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                }
                catch (MessagingException mex)
                {
                    if (mex.Message.Contains("(409)"))
                    {
                    }
                    else
                        throw;
                }

                if (!created)
                    topicDescription = await namespaceManager.GetTopicAsync(topicDescription.Path).ConfigureAwait(false);
            }

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Topic: {0} ({1})", topicDescription.Path,
                    string.Join(", ", new[]
                    {
                        topicDescription.EnableExpress ? "express" : "",
                        topicDescription.RequiresDuplicateDetection ? "dupe detect" : ""
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));
            }

            return topicDescription;
        }

        public static async Task<SubscriptionDescription> CreateTopicSubscriptionSafeAsync(this NamespaceManager namespaceManager, string subscriptionName,
            string topicPath, string queuePath, QueueDescription queueDescription)
        {
            var description = Defaults.CreateSubscriptionDescription(topicPath, subscriptionName, queueDescription, queuePath);

            var create = true;
            SubscriptionDescription subscriptionDescription = null;
            try
            {
                subscriptionDescription = await namespaceManager.GetSubscriptionAsync(topicPath, subscriptionName).ConfigureAwait(false);
                if (queuePath.Equals(subscriptionDescription.ForwardTo))
                {
                    if (_log.IsDebugEnabled)
                    {
                        _log.DebugFormat("Updating subscription: {0} ({1} -> {2})", subscriptionDescription.Name, subscriptionDescription.TopicPath,
                            subscriptionDescription.ForwardTo);
                    }

                    await namespaceManager.UpdateSubscriptionAsync(description).ConfigureAwait(false);

                    create = false;
                }
                else
                {
                    if (_log.IsWarnEnabled)
                    {
                        _log.WarnFormat("Removing invalid subscription: {0} ({1} -> {2})", subscriptionDescription.Name, subscriptionDescription.TopicPath,
                            subscriptionDescription.ForwardTo);
                    }

                    await namespaceManager.DeleteSubscriptionAsync(topicPath, subscriptionName).ConfigureAwait(false);
                }
            }
            catch (MessagingEntityNotFoundException)
            {
            }

            if (create)
            {
                var created = false;
                try
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Creating subscription {0} -> {1}", topicPath, queuePath);


                    subscriptionDescription = await namespaceManager.CreateSubscriptionAsync(description).ConfigureAwait(false);

                    created = true;
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                }
                catch (MessagingException mex)
                {
                    if (mex.Message.Contains("(409)"))
                    {
                    }
                    else
                        throw;
                }

                if (!created)
                    subscriptionDescription = await namespaceManager.GetSubscriptionAsync(topicPath, subscriptionName).ConfigureAwait(false);
            }

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Subscription: {0} ({1} -> {2})", subscriptionDescription.Name, subscriptionDescription.TopicPath,
                    subscriptionDescription.ForwardTo);
            }

            return subscriptionDescription;
        }

        public static async Task<SubscriptionDescription> CreateTopicSubscriptionSafeAsync(this NamespaceManager namespaceManager,
            SubscriptionDescription description)
        {
            var create = true;
            SubscriptionDescription subscriptionDescription = null;
            try
            {
                subscriptionDescription = await namespaceManager.GetSubscriptionAsync(description.TopicPath, description.Name).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(subscriptionDescription.ForwardTo))
                {
                    if (_log.IsWarnEnabled)
                    {
                        _log.WarnFormat("Removing invalid subscription: {0} ({1} -> {2})", subscriptionDescription.Name, subscriptionDescription.TopicPath,
                            subscriptionDescription.ForwardTo);
                    }

                    await namespaceManager.DeleteSubscriptionAsync(description.TopicPath, description.Name).ConfigureAwait(false);
                }
                else
                {
                    create = false;
                }
            }
            catch (MessagingEntityNotFoundException)
            {
            }

            if (create)
            {
                var created = false;
                try
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Creating subscription {0} -> {1}", description.TopicPath, description.Name);


                    subscriptionDescription = await namespaceManager.CreateSubscriptionAsync(description).ConfigureAwait(false);

                    created = true;
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                }
                catch (MessagingException mex)
                {
                    if (mex.Message.Contains("(409)"))
                    {
                    }
                    else
                        throw;
                }

                if (!created)
                    subscriptionDescription = await namespaceManager.GetSubscriptionAsync(description.TopicPath, description.Name).ConfigureAwait(false);
            }

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Subscription: {0} ({1} -> {2})", subscriptionDescription.Name, subscriptionDescription.TopicPath,
                    subscriptionDescription.ForwardTo);
            }

            return subscriptionDescription;
        }
    }
}