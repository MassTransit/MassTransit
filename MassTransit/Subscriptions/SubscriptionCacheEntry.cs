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
namespace MassTransit.Subscriptions
{
	using System;

	public class SubscriptionCacheEntry :
		IEquatable<SubscriptionCacheEntry>
	{
		private readonly Subscription _subscription;

		public SubscriptionCacheEntry(Subscription subscription)
		{
			_subscription = subscription;
		}

		public Subscription Subscription
		{
			get { return _subscription; }
		}

		#region IEquatable<SubscriptionCacheEntry> Members

		public bool Equals(SubscriptionCacheEntry other)
		{
			if (other == null)
				return false;

			return other.Subscription.Equals(_subscription);
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
			return _subscription.GetHashCode();
		}
	}
}