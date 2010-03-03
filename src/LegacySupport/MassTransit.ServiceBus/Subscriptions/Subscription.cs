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
namespace MassTransit.LegacySupport.Subscriptions
{
    using System;

    [Serializable]
    public class Subscription : IEquatable<Subscription>
    {
        string _correlationId;
        protected Uri _endpointUri;
        string _messageName;

        protected Subscription()
        {
        }

        public Subscription(string messageName, Uri endpointUri)
        {
            _endpointUri = endpointUri;
            _messageName = messageName.Trim();
            _correlationId = string.Empty;
        }

        public Subscription(string messageName, string correlationId, Uri endpointUri)
        {
            _endpointUri = endpointUri;
            _messageName = messageName.Trim();
            _correlationId = correlationId;
        }

        public Subscription(Subscription subscription)
        {
            _endpointUri = subscription.EndpointUri;
            _messageName = subscription.MessageName.Trim();
        }

        public Uri EndpointUri
        {
            get { return _endpointUri; }
            set { _endpointUri = value; }
        }

        public string MessageName
        {
            get { return _messageName; }
            set { _messageName = value; }
        }

        public string CorrelationId
        {
            get { return _correlationId; }
            set { _correlationId = value;}
        }

        #region IEquatable<Subscription> Members

        public bool Equals(Subscription subscription)
        {
            if (subscription == null) return false;
            if (!_endpointUri.Equals(subscription._endpointUri)) return false;
            if (!string.Equals(_messageName, subscription._messageName)) return false;
            if (!string.Equals(_correlationId, subscription._correlationId)) return false;
            return true;
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            return Equals(obj as Subscription);
        }

        public override int GetHashCode()
        {
            int result = _endpointUri.GetHashCode();
            result = 29*result + _messageName.GetHashCode();
            result = 29*result + _correlationId.GetHashCode();
            return result;
        }
    }
}