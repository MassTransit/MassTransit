namespace MassTransit.AzureServiceBusTransport.Contexts
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Util;


    public class ServiceBusNamespaceContext :
        BasePipeContext,
        NamespaceContext,
        IAsyncDisposable
    {
        readonly NamespaceManager _namespaceManager;

        public ServiceBusNamespaceContext(NamespaceManager namespaceManager, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _namespaceManager = namespaceManager;
        }

        public Task DisposeAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            LogContext.Debug?.Log("Closed namespace manager: {Host}", _namespaceManager.Address);

            return TaskUtil.Completed;
        }

        public Uri ServiceAddress => _namespaceManager.Address;

        public async Task<QueueDescription> CreateQueue(QueueDescription queueDescription)
        {
            var create = true;
            try
            {
                queueDescription = await _namespaceManager.GetQueueAsync(queueDescription.Path).ConfigureAwait(false);

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
                    LogContext.Debug?.Log("Creating queue {Queue}", queueDescription.Path);

                    queueDescription = await _namespaceManager.CreateQueueAsync(queueDescription).ConfigureAwait(false);

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
                    {
                        throw;
                    }
                }

                if (!created)
                    queueDescription = await _namespaceManager.GetQueueAsync(queueDescription.Path).ConfigureAwait(false);
            }

            LogContext.Debug?.Log("Queue: {Queue} ({Attributes})", queueDescription.Path,
                string.Join(", ",
                    new[]
                    {
                        queueDescription.EnableExpress ? "express" : "",
                        queueDescription.RequiresDuplicateDetection ? "dupe detect" : "",
                        queueDescription.EnableDeadLetteringOnMessageExpiration ? "dead letter" : "",
                        queueDescription.RequiresSession ? "session" : ""
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));

            return queueDescription;
        }

        public async Task<TopicDescription> CreateTopic(TopicDescription topicDescription)
        {
            var create = true;
            try
            {
                topicDescription = await _namespaceManager.GetTopicAsync(topicDescription.Path).ConfigureAwait(false);

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
                    LogContext.Debug?.Log("Creating topic {Topic}", topicDescription.Path);

                    topicDescription = await _namespaceManager.CreateTopicAsync(topicDescription).ConfigureAwait(false);

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
                    {
                        throw;
                    }
                }

                if (!created)
                    topicDescription = await _namespaceManager.GetTopicAsync(topicDescription.Path).ConfigureAwait(false);
            }

            LogContext.Debug?.Log("Topic: {Topic} ({Attributes})", topicDescription.Path,
                string.Join(", ",
                    new[] {topicDescription.EnableExpress ? "express" : "", topicDescription.RequiresDuplicateDetection ? "dupe detect" : ""}.Where(x =>
                        !string.IsNullOrWhiteSpace(x))));

            return topicDescription;
        }

        public async Task<SubscriptionDescription> CreateTopicSubscription(SubscriptionDescription description, RuleDescription rule, Filter filter)
        {
            var create = true;
            SubscriptionDescription subscriptionDescription = null;
            try
            {
                subscriptionDescription = await _namespaceManager.GetSubscriptionAsync(description.TopicPath, description.Name)
                    .ConfigureAwait(false);

                string NormalizeForwardTo(string forwardTo)
                {
                    if (string.IsNullOrEmpty(forwardTo))
                        return string.Empty;

                    var address = _namespaceManager.Address.ToString();
                    return forwardTo.Replace(address, string.Empty).Trim('/');
                }

                var targetForwardTo = NormalizeForwardTo(description.ForwardTo);
                var currentForwardTo = NormalizeForwardTo(subscriptionDescription.ForwardTo);

                if (!targetForwardTo.Equals(currentForwardTo))
                {
                    LogContext.Debug?.Log("Updating subscription: {Subscription} ({Topic} -> {ForwardTo})", subscriptionDescription.Name,
                        subscriptionDescription.TopicPath, subscriptionDescription.ForwardTo);

                    await _namespaceManager.UpdateSubscriptionAsync(description).ConfigureAwait(false);
                }

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
                    LogContext.Debug?.Log("Creating subscription {Subscription} {Topic} -> {ForwardTo}", description.Name, description.TopicPath,
                        description.ForwardTo);

                    subscriptionDescription = rule != null
                        ? await _namespaceManager.CreateSubscriptionAsync(description, rule).ConfigureAwait(false)
                        : filter != null
                            ? await _namespaceManager.CreateSubscriptionAsync(description, filter).ConfigureAwait(false)
                            : await _namespaceManager.CreateSubscriptionAsync(description).ConfigureAwait(false);

                    created = true;
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                }

                if (!created)
                    subscriptionDescription = await _namespaceManager.GetSubscriptionAsync(description.TopicPath, description.Name)
                        .ConfigureAwait(false);
            }


            LogContext.Debug?.Log("Subscription {Subscription} ({Topic} -> {ForwardTo})", subscriptionDescription.Name,
                subscriptionDescription.TopicPath, subscriptionDescription.ForwardTo);

            return subscriptionDescription;
        }

        public async Task DeleteTopicSubscription(SubscriptionDescription description)
        {
            try
            {
                await _namespaceManager.DeleteSubscriptionAsync(description.TopicPath, description.Name).ConfigureAwait(false);
            }
            catch (MessagingEntityNotFoundException)
            {
            }

            LogContext.Debug?.Log("Subscription Deleted: {Subscription} ({Topic} -> {ForwardTo})", description.Name, description.TopicPath,
                description.ForwardTo);
        }
    }
}
