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


    public class QueueEntity :
        Queue,
        QueueHandle
    {
        public QueueEntity(long id, QueueDescription queueDescription)
        {
            Id = id;
            QueueDescription = queueDescription;
        }

        public static IEqualityComparer<QueueEntity> NameComparer { get; } = new NameEqualityComparer();
        public static IEqualityComparer<QueueEntity> EntityComparer { get; } = new QueueEntityEqualityComparer();

        public QueueDescription QueueDescription { get; }
        public long Id { get; }
        public Queue Queue => this;

        public override string ToString()
        {
            return string.Join(", ", new[]
            {
                $"path: {QueueDescription.Path}"
            }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        sealed class QueueEntityEqualityComparer :
            IEqualityComparer<QueueEntity>
        {
            public bool Equals(QueueEntity x, QueueEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return string.Equals(x.QueueDescription.Path, y.QueueDescription.Path)
                    && x.QueueDescription.AutoDeleteOnIdle == y.QueueDescription.AutoDeleteOnIdle
                    && x.QueueDescription.DefaultMessageTimeToLive == y.QueueDescription.DefaultMessageTimeToLive
                    && x.QueueDescription.DuplicateDetectionHistoryTimeWindow == y.QueueDescription.DuplicateDetectionHistoryTimeWindow
                    && x.QueueDescription.EnableBatchedOperations == y.QueueDescription.EnableBatchedOperations
                    && x.QueueDescription.EnableDeadLetteringOnMessageExpiration == y.QueueDescription.EnableDeadLetteringOnMessageExpiration
                    && x.QueueDescription.EnableExpress == y.QueueDescription.EnableExpress
                    && x.QueueDescription.EnablePartitioning == y.QueueDescription.EnablePartitioning
                    && string.Equals(x.QueueDescription.ForwardDeadLetteredMessagesTo, y.QueueDescription.ForwardDeadLetteredMessagesTo)
                    && string.Equals(x.QueueDescription.ForwardTo, y.QueueDescription.ForwardTo)
                    && x.QueueDescription.IsAnonymousAccessible == y.QueueDescription.IsAnonymousAccessible
                    && x.QueueDescription.LockDuration == y.QueueDescription.LockDuration
                    && x.QueueDescription.MaxDeliveryCount == y.QueueDescription.MaxDeliveryCount
                    && x.QueueDescription.MaxSizeInMegabytes == y.QueueDescription.MaxSizeInMegabytes
                    && x.QueueDescription.RequiresDuplicateDetection == y.QueueDescription.RequiresDuplicateDetection
                    && x.QueueDescription.RequiresSession == y.QueueDescription.RequiresSession
                    && x.QueueDescription.SupportOrdering == y.QueueDescription.SupportOrdering
                    && string.Equals(x.QueueDescription.UserMetadata, y.QueueDescription.UserMetadata);
            }

            public int GetHashCode(QueueEntity obj)
            {
                unchecked
                {
                    var hashCode = obj.QueueDescription.Path.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.AutoDeleteOnIdle.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.DefaultMessageTimeToLive.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.DuplicateDetectionHistoryTimeWindow.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.EnableBatchedOperations.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.EnableDeadLetteringOnMessageExpiration.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.EnableExpress.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.EnablePartitioning.GetHashCode();
                    if (!string.IsNullOrWhiteSpace(obj.QueueDescription.ForwardDeadLetteredMessagesTo))
                        hashCode = (hashCode * 397) ^ obj.QueueDescription.ForwardDeadLetteredMessagesTo.GetHashCode();
                    if (!string.IsNullOrWhiteSpace(obj.QueueDescription.ForwardTo))
                        hashCode = (hashCode * 397) ^ obj.QueueDescription.ForwardTo.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.IsAnonymousAccessible.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.LockDuration.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.MaxDeliveryCount.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.MaxSizeInMegabytes.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.RequiresDuplicateDetection.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.RequiresSession.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.SupportOrdering.GetHashCode();
                    if (!string.IsNullOrWhiteSpace(obj.QueueDescription.UserMetadata))
                        hashCode = (hashCode * 397) ^ obj.QueueDescription.UserMetadata.GetHashCode();

                    return hashCode;
                }
            }
        }


        sealed class NameEqualityComparer :
            IEqualityComparer<QueueEntity>
        {
            public bool Equals(QueueEntity x, QueueEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return string.Equals(x.QueueDescription.Path, y.QueueDescription.Path);
            }

            public int GetHashCode(QueueEntity obj)
            {
                return obj.QueueDescription.Path.GetHashCode();
            }
        }
    }
}