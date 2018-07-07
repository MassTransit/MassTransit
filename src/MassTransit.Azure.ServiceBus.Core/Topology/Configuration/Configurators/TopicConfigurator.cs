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
namespace MassTransit.Azure.ServiceBus.Core.Topology.Configuration.Configurators
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using Microsoft.Azure.ServiceBus.Management;


    public class TopicConfigurator :
        MessageEntityConfigurator,
        ITopicConfigurator
    {
        public TopicConfigurator(string topicPath, bool temporary)
            : base(topicPath)
        {
            if (temporary)
            {
                AutoDeleteOnIdle = Defaults.TemporaryAutoDeleteOnIdle;
            }
        }

        public bool? EnableFilteringMessagesBeforePublishing { get; set; }

        public IEnumerable<ValidationResult> Validate()
        {
            if (!ServiceBusEntityNameValidator.Validator.IsValidEntityName(Path))
                yield return this.Failure("Path", $"must be a valid topic path: {Path}");

            if (AutoDeleteOnIdle.HasValue && AutoDeleteOnIdle != TimeSpan.Zero && AutoDeleteOnIdle < TimeSpan.FromMinutes(5))
                yield return this.Failure("AutoDeleteOnIdle", "must be zero, or >= 5:00");
        }

        public TopicDescription GetTopicDescription()
        {
            var topicDescription = new TopicDescription(FullPath);

            if (AutoDeleteOnIdle.HasValue)
                topicDescription.AutoDeleteOnIdle = AutoDeleteOnIdle.Value;

            if (DefaultMessageTimeToLive.HasValue)
                topicDescription.DefaultMessageTimeToLive = DefaultMessageTimeToLive.Value;

            if (DuplicateDetectionHistoryTimeWindow.HasValue)
                topicDescription.DuplicateDetectionHistoryTimeWindow = DuplicateDetectionHistoryTimeWindow.Value;

            if (EnableBatchedOperations.HasValue)
                topicDescription.EnableBatchedOperations = EnableBatchedOperations.Value;

            if (EnablePartitioning.HasValue)
                topicDescription.EnablePartitioning = EnablePartitioning.Value;

            if (MaxSizeInMB.HasValue)
                topicDescription.MaxSizeInMB = MaxSizeInMB.Value;

            if (RequiresDuplicateDetection.HasValue)
                topicDescription.RequiresDuplicateDetection = RequiresDuplicateDetection.Value;

            if (!string.IsNullOrWhiteSpace(UserMetadata))
                topicDescription.UserMetadata = UserMetadata;

            return topicDescription;
        }
    }
}