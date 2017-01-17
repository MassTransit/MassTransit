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
namespace MassTransit.AzureServiceBusTransport.Topology.Entities
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.ServiceBus.Messaging;


    public class TopicEntity :
        Topic,
        TopicHandle
    {
        public TopicEntity(long id, TopicDescription topicDescription)
        {
            Id = id;

            TopicDescription = topicDescription;
        }

        public static IEqualityComparer<TopicEntity> NameComparer { get; } = new NameEqualityComparer();
        public static IEqualityComparer<TopicEntity> EntityComparer { get; } = new TopicEntityEqualityComparer();

        public TopicDescription TopicDescription { get; }
        public long Id { get; }
        public Topic Topic => this;

        public override string ToString()
        {
            return string.Join(", ", new[]
            {
                $"path: {TopicDescription.Path}"
            }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        sealed class TopicEntityEqualityComparer :
            IEqualityComparer<TopicEntity>
        {
            public bool Equals(TopicEntity x, TopicEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return string.Equals(x.TopicDescription.Path, y.TopicDescription.Path)
                    && x.TopicDescription.AutoDeleteOnIdle == y.TopicDescription.AutoDeleteOnIdle
                    && x.TopicDescription.DefaultMessageTimeToLive == y.TopicDescription.DefaultMessageTimeToLive
                    && x.TopicDescription.DuplicateDetectionHistoryTimeWindow == y.TopicDescription.DuplicateDetectionHistoryTimeWindow
                    && x.TopicDescription.EnableBatchedOperations == y.TopicDescription.EnableBatchedOperations
                    && x.TopicDescription.EnableExpress == y.TopicDescription.EnableExpress
                    && x.TopicDescription.EnableFilteringMessagesBeforePublishing == y.TopicDescription.EnableFilteringMessagesBeforePublishing
                    && x.TopicDescription.EnablePartitioning == y.TopicDescription.EnablePartitioning
                    && x.TopicDescription.IsAnonymousAccessible == y.TopicDescription.IsAnonymousAccessible
                    && x.TopicDescription.MaxSizeInMegabytes == y.TopicDescription.MaxSizeInMegabytes
                    && x.TopicDescription.RequiresDuplicateDetection == y.TopicDescription.RequiresDuplicateDetection
                    && x.TopicDescription.SupportOrdering == y.TopicDescription.SupportOrdering
                    && string.Equals(x.TopicDescription.UserMetadata, y.TopicDescription.UserMetadata);
            }

            public int GetHashCode(TopicEntity obj)
            {
                unchecked
                {
                    var hashCode = obj.TopicDescription.Path.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TopicDescription.AutoDeleteOnIdle.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TopicDescription.DefaultMessageTimeToLive.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TopicDescription.DuplicateDetectionHistoryTimeWindow.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TopicDescription.EnableBatchedOperations.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TopicDescription.EnableExpress.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TopicDescription.EnableFilteringMessagesBeforePublishing.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TopicDescription.EnablePartitioning.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TopicDescription.IsAnonymousAccessible.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TopicDescription.MaxSizeInMegabytes.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TopicDescription.RequiresDuplicateDetection.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TopicDescription.SupportOrdering.GetHashCode();
                    if (!string.IsNullOrWhiteSpace(obj.TopicDescription.UserMetadata))
                        hashCode = (hashCode * 397) ^ obj.TopicDescription.UserMetadata.GetHashCode();

                    return hashCode;
                }
            }
        }


        sealed class NameEqualityComparer :
            IEqualityComparer<TopicEntity>
        {
            public bool Equals(TopicEntity x, TopicEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return string.Equals(x.TopicDescription.Path, y.TopicDescription.Path);
            }

            public int GetHashCode(TopicEntity obj)
            {
                return obj.TopicDescription.Path.GetHashCode();
            }
        }
    }
}