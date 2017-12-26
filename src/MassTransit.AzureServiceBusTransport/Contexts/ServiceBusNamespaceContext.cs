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
namespace MassTransit.AzureServiceBusTransport.Contexts
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Util;


    public class ServiceBusNamespaceContext :
        BasePipeContext,
        NamespaceContext
    {
        readonly IServiceBusHost _host;
        readonly IReceiveTransportObserver _receiveTransportObserver;
        readonly IReceiveObserver _receiveObserver;
        readonly ITaskSupervisor _supervisor;

        public ServiceBusNamespaceContext(IServiceBusHost host, IReceiveObserver receiveObserver, IReceiveTransportObserver receiveTransportObserver,
            TaskSupervisor supervisor)
            : base(new PayloadCache(), supervisor.StoppingToken)
        {
            _host = host;
            _receiveObserver = receiveObserver;
            _receiveTransportObserver = receiveTransportObserver;
            _supervisor = supervisor;
        }

        public Task<MessagingFactory> MessagingFactory => _host.MessagingFactory;

        public Task<MessagingFactory> SessionMessagingFactory => _host.SessionMessagingFactory;

        public NamespaceManager NamespaceManager => _host.NamespaceManager;

        public Uri ServiceAddress => _host.Settings.ServiceUri;

        public Task<QueueDescription> CreateQueue(QueueDescription queueDescription)
        {
            return _host.CreateQueue(queueDescription);
        }

        public Task<TopicDescription> CreateTopic(TopicDescription topicDescription)
        {
            return _host.CreateTopic(topicDescription);
        }

        public Task<SubscriptionDescription> CreateTopicSubscription(SubscriptionDescription subscriptionDescription)
        {
            return _host.CreateTopicSubscription(subscriptionDescription);
        }

        public Task<SubscriptionDescription> CreateTopicSubscription(string subscriptionName, string topicPath, string queuePath,
            QueueDescription queueDescription)
        {
            return _host.CreateTopicSubscription(Defaults.CreateSubscriptionDescription(topicPath, subscriptionName, queueDescription, queuePath));
        }

        public Task DeleteTopicSubscription(SubscriptionDescription description)
        {
            return _host.DeleteTopicSubscription(description);
        }

        ITaskScope NamespaceContext.CreateScope(string tag)
        {
            return _supervisor.CreateScope(tag);
        }

        public string GetQueuePath(QueueDescription queueDescription)
        {
            return _host.GetQueuePath(queueDescription);
        }

        Task IReceiveObserver.PreReceive(ReceiveContext context)
        {
            return _receiveObserver.PreReceive(context);
        }

        Task IReceiveObserver.PostReceive(ReceiveContext context)
        {
            return _receiveObserver.PostReceive(context);
        }

        Task IReceiveObserver.PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            return _receiveObserver.PostConsume(context, duration, consumerType);
        }

        Task IReceiveObserver.ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        {
            return _receiveObserver.ConsumeFault(context, duration, consumerType, exception);
        }

        Task IReceiveObserver.ReceiveFault(ReceiveContext context, Exception exception)
        {
            return _receiveObserver.ReceiveFault(context, exception);
        }

        Task IReceiveTransportObserver.Ready(ReceiveTransportReady ready)
        {
            return _receiveTransportObserver.Ready(ready);
        }

        Task IReceiveTransportObserver.Completed(ReceiveTransportCompleted completed)
        {
            return _receiveTransportObserver.Completed(completed);
        }

        Task IReceiveTransportObserver.Faulted(ReceiveTransportFaulted faulted)
        {
            return _receiveTransportObserver.Faulted(faulted);
        }
    }
}