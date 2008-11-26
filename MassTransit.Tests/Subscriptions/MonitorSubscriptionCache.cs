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
namespace MassTransit.Tests.Subscriptions
{
	using System;
	using System.Threading;
	using MassTransit.Subscriptions;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	public class MonitorSubscriptionCache<TMessage>
	{
		private readonly ManualResetEvent _added = new ManualResetEvent(false);
		private readonly ISubscriptionCache _cache;
		private readonly ManualResetEvent _removed = new ManualResetEvent(false);

		public MonitorSubscriptionCache(ISubscriptionCache cache)
		{
			_cache = cache;

			_cache.OnAddSubscription += OnAddSubscription;
			_cache.OnRemoveSubscription += OnRemoveSubscription;
		}

		private void OnAddSubscription(object sender, SubscriptionEventArgs e)
		{
			if (e.Subscription.MessageName == typeof (TMessage).FullName)
				_added.Set();
		}

		private void OnRemoveSubscription(object sender, SubscriptionEventArgs e)
		{
			if (e.Subscription.MessageName == typeof (TMessage).FullName)
				_removed.Set();
		}

		public void ShouldHaveBeenAdded(TimeSpan timeout)
		{
			Assert.That(_added.WaitOne(timeout, true), Is.True, "The subscription should have been added");
		}

		public void ShouldHaveBeenRemoved(TimeSpan timeout)
		{
			Assert.That(_removed.WaitOne(timeout, true), Is.True, "The subscription should have been removed");
		}
	}
}