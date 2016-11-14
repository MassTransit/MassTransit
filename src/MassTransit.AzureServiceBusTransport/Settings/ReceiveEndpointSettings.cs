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
namespace MassTransit.AzureServiceBusTransport.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using GreenPipes.Internals.Reflection;
    using Microsoft.ServiceBus.Messaging;
    using Transport;


    public class ReceiveEndpointSettings :
        BaseClientSettings,
        ReceiveSettings
    {
        static readonly ReadWriteProperty<QueueDescription, TimeSpan?> _autoDeleteOnIdle;
        static readonly ReadWriteProperty<QueueDescription, bool?> _enableExpress;

        static ReceiveEndpointSettings()
        {
            var propertyInfo = typeof(QueueDescription).GetProperty("InternalAutoDeleteOnIdle", BindingFlags.Instance | BindingFlags.NonPublic);
            _autoDeleteOnIdle = new ReadWriteProperty<QueueDescription, TimeSpan?>(propertyInfo);

             propertyInfo = typeof(QueueDescription).GetProperty("InternalEnableExpress", BindingFlags.Instance | BindingFlags.NonPublic);
            _enableExpress = new ReadWriteProperty<QueueDescription, bool?>(propertyInfo);
        }

        public ReceiveEndpointSettings(QueueDescription description)
        {
            QueueDescription = description;
        }

        public override void SelectBasicTier()
        {
            _autoDeleteOnIdle.Set(QueueDescription, default(TimeSpan?));
            _enableExpress.Set(QueueDescription, default(bool?));

            QueueDescription.DefaultMessageTimeToLive = TimeSpan.FromDays(14);
        }

        public override TimeSpan AutoDeleteOnIdle
        {
            get { return QueueDescription.AutoDeleteOnIdle; }
            set { QueueDescription.AutoDeleteOnIdle = value; }
        }

        public override TimeSpan DefaultMessageTimeToLive
        {
            get { return QueueDescription.DefaultMessageTimeToLive; }
            set { QueueDescription.DefaultMessageTimeToLive = value; }
        }

        public TimeSpan DuplicateDetectionHistoryTimeWindow
        {
            set { QueueDescription.DuplicateDetectionHistoryTimeWindow = value; }
        }

        public override bool EnableBatchedOperations
        {
            set { QueueDescription.EnableBatchedOperations = value; }
        }

        public override bool EnableDeadLetteringOnMessageExpiration
        {
            set { QueueDescription.EnableDeadLetteringOnMessageExpiration = value; }
        }

        public bool RequiresDuplicateDetection
        {
            set { QueueDescription.RequiresDuplicateDetection = value; }
        }

        public bool EnablePartitioning
        {
            set { QueueDescription.EnablePartitioning = value; }
        }

        public override string ForwardDeadLetteredMessagesTo
        {
            set { QueueDescription.ForwardDeadLetteredMessagesTo = value; }
        }

        public bool IsAnonymousAccessible
        {
            set { QueueDescription.IsAnonymousAccessible = value; }
        }

        public override int MaxDeliveryCount
        {
            get { return QueueDescription.MaxDeliveryCount; }
            set { QueueDescription.MaxDeliveryCount = value; }
        }

        public long MaxSizeInMegabytes
        {
            set { QueueDescription.MaxSizeInMegabytes = value; }
        }

        public override bool RequiresSession
        {
            get { return QueueDescription.RequiresSession; }
            set { QueueDescription.RequiresSession = value; }
        }

        public bool SupportOrdering
        {
            set { QueueDescription.SupportOrdering = value; }
        }

        public override string UserMetadata
        {
            set { QueueDescription.UserMetadata = value; }
        }

        public QueueDescription QueueDescription { get; }

        public bool RemoveSubscriptions { get; set; }

        public override TimeSpan LockDuration
        {
            get { return QueueDescription.LockDuration; }
            set { QueueDescription.LockDuration = value; }
        }

        public override string Path => QueueDescription.Path;

        protected override IEnumerable<string> GetQueryStringOptions()
        {
            if (QueueDescription.EnableExpress)
                yield return "express=true";

            if (QueueDescription.AutoDeleteOnIdle > TimeSpan.Zero && QueueDescription.AutoDeleteOnIdle != Defaults.AutoDeleteOnIdle)
                yield return $"autodelete={QueueDescription.AutoDeleteOnIdle.TotalSeconds}";
        }
    }
}