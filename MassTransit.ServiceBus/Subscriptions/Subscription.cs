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
	public class Subscription :
		IEquatable<Subscription>
	{
		protected Uri _endpointUri;
		private string _messageName;

		protected Subscription()
		{
		}

		public Subscription(string messageName, Uri endpointUri)
		{
			_endpointUri = endpointUri;
			_messageName = messageName.Trim();
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

		#region IEquatable<Subscription> Members

		public bool Equals(Subscription other)
		{
			if (other == null)
				return false;

			if (!other.EndpointUri.Equals(_endpointUri))
				return false;

			if (!other.MessageName.Equals(_messageName))
				return false;

			return true;
		}

		#endregion

		public override bool Equals(object obj)
		{
			Subscription other = obj as Subscription;

			return Equals(other);
		}

		public override int GetHashCode()
		{
			return _endpointUri.GetHashCode() + 29*_messageName.GetHashCode();
		}
	}
}