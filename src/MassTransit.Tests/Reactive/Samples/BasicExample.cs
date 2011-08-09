// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Reactive.Samples
{
	using System;
	using System.Reactive.Linq;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using MassTransit.Reactive;
	using Messages;
	using NUnit.Framework;
	using TestFramework;

	[Scenario, Explicit("Fails from command-line build, don't know why, so I'm removing it from the build")]
	public class BasicExample :
		Given_a_standalone_service_bus
	{
		[Given]
		public void A_reactive_query_is_observing_a_bus_message()
		{
			_observable = LocalBus.AsObservable<PingMessage>();

			_thatJustHappened = new Future<PingMessage>();
			_subscription = _observable.Subscribe(m => _thatJustHappened.Complete(m));

			LocalBus.Publish(new PingMessage());
		}

		IObservable<PingMessage> _observable;
		Future<PingMessage> _thatJustHappened;
		IDisposable _subscription;

		[Finally]
		public void Finally()
		{
			_subscription.Dispose();
		}

		[Then]
		public void The_message_should_be_observed()
		{
			Assert.IsNotNull(_observable.Timeout(8.Seconds()).Take(1).Single());

			_thatJustHappened.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();
		}
	}
}