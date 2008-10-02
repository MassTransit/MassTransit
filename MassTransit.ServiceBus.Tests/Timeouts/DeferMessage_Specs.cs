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
namespace MassTransit.ServiceBus.Tests.Timeouts
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
	using Castle.Core;
	using MassTransit.ServiceBus.Timeout.Messages;
	using MassTransit.Services.Messages;
	using Messages;
	using NUnit.Framework;
	using Services;
	using Timeout;
	using Util;

	[TestFixture]
	public class When_a_message_is_deferred :
		LocalAndRemoteTestContext
	{
		private ITimeoutStorage _storage;
		private TimeoutService _timeoutService;
		private Guid _correlationId;
		private DateTime _dateTime;
		private IDeferredMessageRepository _repository;
		private MessageDeferralService _deferService;

		protected override void Before_each()
		{
			_correlationId = CombGuid.NewCombGuid();
			_dateTime = DateTime.UtcNow + TimeSpan.FromSeconds(1);

			Container.AddComponentLifeStyle<ITimeoutStorage, InMemoryTimeoutStorage>(LifestyleType.Singleton);

			_storage = Container.Resolve<ITimeoutStorage>();

			_timeoutService = new TimeoutService(LocalBus, _storage);
			_timeoutService.Start();

			Container.AddComponentLifeStyle<IDeferredMessageRepository, InMemoryDeferredMessageRepository>(LifestyleType.Singleton);

			_repository = Container.Resolve<IDeferredMessageRepository>();

			_deferService = new MessageDeferralService(LocalBus);
			_deferService.Start();
		}

		protected override void After_each()
		{
			_deferService.Stop();
			_deferService.Dispose();
			
			_timeoutService.Stop();
			_timeoutService.Dispose();
		}

		[Test]
		public void It_should_be_received_after_the_deferral_period_elapses()
		{
			ManualResetEvent _timedOut = new ManualResetEvent(false);

			Stopwatch watch = Stopwatch.StartNew();

			LocalBus.Subscribe<PingMessage>(x => _timedOut.Set());

			LocalBus.Publish(new DeferMessage(_correlationId, TimeSpan.FromSeconds(3), new PingMessage()));

			Assert.IsTrue(_timedOut.WaitOne(TimeSpan.FromSeconds(10), true));

			watch.Stop();

			Debug.WriteLine(string.Format("Timeout took {0}ms", watch.ElapsedMilliseconds));
		}
	}
}