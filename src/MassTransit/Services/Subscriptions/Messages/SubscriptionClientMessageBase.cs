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
namespace MassTransit.Services.Subscriptions.Messages
{
	using System;

	[Serializable]
	public class SubscriptionClientMessageBase :
		CorrelatedBy<Guid>
	{
		protected SubscriptionClientMessageBase(Guid clientId, Uri controlUri, Uri dataUri)
		{
			ControlUri = controlUri;
			DataUri = dataUri;
			CorrelationId = clientId;
		}

		protected SubscriptionClientMessageBase()
		{
		}

		public Guid CorrelationId { get; set; }
		public Uri ControlUri { get; set; }
		public Uri DataUri { get; set; }

	    public bool Equals(SubscriptionClientMessageBase other)
	    {
	        if (ReferenceEquals(null, other)) return false;
	        if (ReferenceEquals(this, other)) return true;
	        return other.CorrelationId.Equals(CorrelationId) && Equals(other.ControlUri, ControlUri) && Equals(other.DataUri, DataUri);
	    }

	    public override bool Equals(object obj)
	    {
	        if (ReferenceEquals(null, obj)) return false;
	        if (ReferenceEquals(this, obj)) return true;
	        if (obj.GetType() != typeof(SubscriptionClientMessageBase)) return false;
	        return Equals((SubscriptionClientMessageBase) obj);
	    }

	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            int result = CorrelationId.GetHashCode();
	            result = (result*397) ^ (ControlUri != null ? ControlUri.GetHashCode() : 0);
	            result = (result*397) ^ (DataUri != null ? DataUri.GetHashCode() : 0);
	            return result;
	        }
	    }
	}
}