// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Infrastructure
{
    using System;
    using Subscriptions;
    using Subscriptions;

    public class PersistentSubscription :
        Subscription
    {
        private int _id;
        private bool _isActive;

        protected PersistentSubscription()
        {
        }

        public PersistentSubscription(Subscription subscription)
            : base(subscription)
        {
            _isActive = true;
        }

        public int Id
        {
            get { return _id; }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        public string Address
        {
            get { return EndpointUri.ToString(); }
            set { _endpointUri = new Uri(value); }
        }
    }
}