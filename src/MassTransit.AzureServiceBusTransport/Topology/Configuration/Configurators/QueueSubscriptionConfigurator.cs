// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Topology.Configurators
{
    public class QueueSubscriptionConfigurator :
        IQueueSubscriptionConfigurator
    {
        SubscriptionConfigurator _subscriptionConfigurator;
        TopicConfigurator _topicConfigurator;

        public QueueSubscriptionConfigurator(string topicPath, string subscriptionName)
        {
            _topicConfigurator = new TopicConfigurator(topicPath);
            _subscriptionConfigurator = new SubscriptionConfigurator(topicPath, subscriptionName);
        }
    }
}