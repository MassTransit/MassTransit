/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.

using System;

namespace MassTransit.ServiceBus.Subscriptions.Messages
{
    [Serializable]
    public class SubscriptionChange : IMessage
    {
        #region SubscriptionChangeType enum

        #endregion

        private Subscription _subscription;
        private SubscriptionChangeType _changeType;

        public SubscriptionChange(string messageName, Uri address, SubscriptionChangeType changeType): this(new Subscription(address, messageName), changeType)
        {
        }

        public SubscriptionChange(Subscription subscription, SubscriptionChangeType changeType)
        {
            _changeType = changeType;
            _subscription = subscription;
        }

        public SubscriptionChangeType ChangeType
        {
            get { return _changeType; }
        }


        public Subscription Subscription
        {
            get { return _subscription; }
        }

    }
}