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
namespace MassTransit.AmazonSqsTransport.Topology.Configuration.Configurators
{
    using System.Collections.Generic;
    using AmazonSqsTransport.Configuration;
    using Entities;


    public class QueueConfigurator :
        EntityConfigurator,
        IQueueConfigurator,
        Queue
    {
        protected QueueConfigurator(string queueName, bool durable = true, bool autoDelete = false, IDictionary<string, object> queueAttributes = null, IDictionary<string, object> queueSubscriptionAttributes = null, IDictionary<string, string> queueTags = null)
            : base(queueName, durable, autoDelete)
        {
            QueueAttributes = queueAttributes ?? new Dictionary<string, object>();
            QueueSubscriptionAttributes = queueSubscriptionAttributes ?? new Dictionary<string, object>();
            QueueTags = queueTags ?? new Dictionary<string, string>();
        }

        public QueueConfigurator(Queue source)
            : base(source.EntityName, source.Durable, source.AutoDelete)
        {
            QueueAttributes = source.QueueAttributes;
            QueueSubscriptionAttributes = source.QueueSubscriptionAttributes;
            QueueTags = source.QueueTags;
        }

        public IDictionary<string, object> QueueAttributes { get; set; }
        public IDictionary<string, object> QueueSubscriptionAttributes { get; set; }
        public IDictionary<string, string> QueueTags { get; set; }

        public IDictionary<string, string> Tags => QueueTags;
    }
}
