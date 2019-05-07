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
namespace MassTransit.AmazonSqsTransport.Topology.Builders
{
    using Logging;
    using Microsoft.Extensions.Logging;


    public static class TopologyLayoutExtensions
    {
        static readonly ILogger _logger = Logger.Get<BrokerTopology>();

        public static void LogResult(this BrokerTopology layout)
        {
            foreach (var topic in layout.Topics)
            {
                _logger.LogInformation("Topic: {0}, type: {1}, durable: {2}, auto-delete: {3}", topic.EntityName, topic.Durable, topic.AutoDelete);
            }

            foreach (var consumer in layout.QueueSubscriptions)
            {
                _logger.LogInformation("TopicSubscription: source {0}, destination: {1}", consumer.Source.EntityName, consumer.Destination.EntityName);
            }
        }
    }
}
