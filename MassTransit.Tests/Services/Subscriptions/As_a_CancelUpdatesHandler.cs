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
	using MassTransit.Subscriptions;
	using MassTransit.Subscriptions.Messages;
	using MassTransit.Subscriptions.ServerHandlers;
	using NUnit.Framework;

	[TestFixture]
	public class As_a_CancelUpdatesHandler
		: Specification
	{
		private CancelUpdatesHandler handle;
		private IEndpointFactory _mockResolver;
		private CancelSubscriptionUpdates msgCancel;
		private Uri uri = new Uri("queue:\\bob");

		protected override void Before_each()
		{
			msgCancel = new CancelSubscriptionUpdates(uri);
			_mockResolver = StrictMock<IEndpointFactory>();
			handle = new CancelUpdatesHandler(new FollowerRepository(_mockResolver));
		}


		[Test]
		public void respond_to_update_cancel()
		{
			using (Record())
			{
			}
			using (Playback())
			{
				handle.Consume(msgCancel);
			}
		}
	}
}