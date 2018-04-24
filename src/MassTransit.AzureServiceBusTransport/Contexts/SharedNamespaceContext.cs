// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Microsoft.ServiceBus.Messaging;


    public class SharedNamespaceContext :
        BasePipeContext,
        NamespaceContext
    {
        readonly NamespaceContext _context;

        public SharedNamespaceContext(NamespaceContext context, CancellationToken cancellationToken)
            : base(new PayloadCacheScope(context), cancellationToken)
        {
            _context = context;
        }

        Uri NamespaceContext.ServiceAddress => _context.ServiceAddress;

        Task<QueueDescription> NamespaceContext.CreateQueue(QueueDescription queueDescription)
        {
            return _context.CreateQueue(queueDescription);
        }

        Task<TopicDescription> NamespaceContext.CreateTopic(TopicDescription topicDescription)
        {
            return _context.CreateTopic(topicDescription);
        }

        Task<SubscriptionDescription> NamespaceContext.CreateTopicSubscription(SubscriptionDescription subscriptionDescription, RuleDescription rule, Filter filter)
        {
            return _context.CreateTopicSubscription(subscriptionDescription, rule, filter);
        }

        Task NamespaceContext.DeleteTopicSubscription(SubscriptionDescription description)
        {
            return _context.DeleteTopicSubscription(description);
        }
    }
}