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
namespace MassTransit.WindowsServiceBusTransport.Configuration
{
    using System.Collections.Generic;
    using MassTransit.Pipeline;
    using Transports;


    public class ServiceBusReceiveEndpointBuilder :
        IReceiveEndpointBuilder
    {
        readonly IConsumePipe _consumePipe;
        readonly IMessageNameFormatter _messageNameFormatter;
        readonly bool _subscribeMessageTopics;
        readonly List<TopicSubscriptionSettings> _topicSubscriptions;

        public ServiceBusReceiveEndpointBuilder(IConsumePipe consumePipe, IMessageNameFormatter messageNameFormatter, bool subscribeMessageTopics)
        {
            _consumePipe = consumePipe;
            _messageNameFormatter = messageNameFormatter;
            _subscribeMessageTopics = subscribeMessageTopics;
            _topicSubscriptions = new List<TopicSubscriptionSettings>();
        }

        ConnectHandle IConsumePipeConnector.ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            if (_subscribeMessageTopics)
                _topicSubscriptions.AddRange(_messageNameFormatter.GetTopicSubscription(typeof(T)));

            return _consumePipe.ConnectConsumePipe(pipe);
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return _consumePipe.ConnectConsumeMessageObserver(observer);
        }

        public IEnumerable<TopicSubscriptionSettings> GetTopicSubscriptions()
        {
            return _topicSubscriptions;
        }
    }
}