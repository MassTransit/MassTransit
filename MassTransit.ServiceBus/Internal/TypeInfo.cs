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
namespace MassTransit.ServiceBus.Internal
{
    public class TypeInfo : ITypeInfo
    {
        private readonly SubscriptionTypeInfo _subscriptionTypeInfo = new SubscriptionTypeInfo();
        private IPublicationTypeInfo _publicationTypeInfo;

        internal IPublicationTypeInfo PublicationTypeInfo
        {
            set { _publicationTypeInfo = value; }
        }

        public SubscriptionTypeInfo SubscriptionTypeInfo
        {
            get { return _subscriptionTypeInfo; }
        }

        public ISubscriptionTypeInfo GetSubscriptionTypeInfo()
        {
            return _subscriptionTypeInfo;
        }

        public IPublicationTypeInfo GetPublicationTypeInfo()
        {
            return _publicationTypeInfo;
        }

        public void Dispose()
        {
            _subscriptionTypeInfo.Dispose();
        }
    }
}