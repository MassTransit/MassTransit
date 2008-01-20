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
using MassTransit.ServiceBus.Util;

namespace MassTransit.ServiceBus.Subscriptions
{
    public class SubscriptionCacheEntry :
        IEquatable<SubscriptionCacheEntry>
    {
    	private MessageId _messageId = Util.MessageId.Empty;
        private Uri _endpoint;


        public SubscriptionCacheEntry(Uri endpoint)
        {
            _endpoint = endpoint;
        }

        public Uri Endpoint
        {
            get { return _endpoint; }
            set { _endpoint = value; }
        }

		public MessageId MessageId
        {
            get { return _messageId; }
            set { _messageId = value; }
        }

        #region IEquatable<SubscriptionCacheEntry> Members

        public bool Equals(SubscriptionCacheEntry other)
        {
            if (other == null)
                return false;

			if (_endpoint != other.Endpoint)
				return false;

			if (_messageId != other.MessageId)
				return false;

        	return true;
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            return Equals(obj as SubscriptionCacheEntry);
        }

        public override int GetHashCode()
        {
            return _endpoint.GetHashCode() + 29*_messageId.GetHashCode();
        }

    }
}