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
namespace MassTransit.AzureServiceBusTransport.Topology.Configuration.Configurators
{
    using System;
    using Microsoft.ServiceBus.Messaging;


    public class TopicConfigurator :
        MessageEntityConfigurator,
        ITopicConfigurator
    {
        public TopicConfigurator(string topicPath, bool temporary)
            : base(topicPath)
        {
            if (temporary)
            {
                AutoDeleteOnIdle = TimeSpan.FromMinutes(5);
                EnableExpress = true;
            }
        }

        public bool? EnableFilteringMessagesBeforePublishing { get; set; }

        public TopicDescription GetTopicDescription()
        {
            var topicDescription = new TopicDescription(Path);

            if (AutoDeleteOnIdle.HasValue)
                topicDescription.AutoDeleteOnIdle = AutoDeleteOnIdle.Value;

            if (DefaultMessageTimeToLive.HasValue)
                topicDescription.DefaultMessageTimeToLive = DefaultMessageTimeToLive.Value;

            if (DuplicateDetectionHistoryTimeWindow.HasValue)
                topicDescription.DuplicateDetectionHistoryTimeWindow = DuplicateDetectionHistoryTimeWindow.Value;

            if (EnableBatchedOperations.HasValue)
                topicDescription.EnableBatchedOperations = EnableBatchedOperations.Value;

            if (EnableExpress.HasValue)
                topicDescription.EnableExpress = EnableExpress.Value;

            if (EnableFilteringMessagesBeforePublishing.HasValue)
                topicDescription.EnableFilteringMessagesBeforePublishing = EnableFilteringMessagesBeforePublishing.Value;

            if (EnablePartitioning.HasValue)
                topicDescription.EnablePartitioning = EnablePartitioning.Value;

            if (IsAnonymousAccessible.HasValue)
                topicDescription.IsAnonymousAccessible = IsAnonymousAccessible.Value;

            if (MaxSizeInMegabytes.HasValue)
                topicDescription.MaxSizeInMegabytes = MaxSizeInMegabytes.Value;

            if (RequiresDuplicateDetection.HasValue)
                topicDescription.RequiresDuplicateDetection = RequiresDuplicateDetection.Value;

            if (SupportOrdering.HasValue)
                topicDescription.SupportOrdering = SupportOrdering.Value;

            if (!string.IsNullOrWhiteSpace(UserMetadata))
                topicDescription.UserMetadata = UserMetadata;

            return topicDescription;
        }
    }
}