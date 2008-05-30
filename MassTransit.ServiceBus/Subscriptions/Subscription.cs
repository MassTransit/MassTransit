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
namespace MassTransit.ServiceBus.Subscriptions
{
	using System;

	[Serializable]
	public class Subscription : IEquatable<Subscription>
	{
		private string _correlationId;
		protected Uri _endpointUri;
		private string _messageName;

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
		}

		public string MessageName
		{
			get { return _messageName; }
		}

		public string CorrelationId
		{
			get { return _correlationId; }
		}

		public bool Equals(Subscription subscription)
		{
			if (subscription == null) return false;
			if (!Equals(_endpointUri, subscription._endpointUri)) return false;
			if (!Equals(_messageName, subscription._messageName)) return false;
			if (!Equals(_correlationId, subscription._correlationId)) return false;
			return true;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
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