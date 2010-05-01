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
namespace MassTransit.Tests.Reactive
{
	using System;
	using MassTransit.Reactive;
	using Messages;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class ServiceBusSubscriptionSpecs
	{
		[Test]
		public void WhenUnsubscribedIsCalledShouldCallAction()
		{
			var bus = MockRepository.GenerateStub<IServiceBus>();
			var obs = MockRepository.GenerateStub<IObserver<PingMessage>>();
			bool disposed = false;
			bus.Stub(b => b.Subscribe<PingMessage>(obs.OnNext)).Return(() => disposed = true);

			var sbs = new ServiceBusSubscription<PingMessage>(bus, obs, null);
			sbs.Dispose();

			Assert.IsTrue(disposed);
		}
	}
}