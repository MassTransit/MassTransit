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
	using System.Diagnostics;
	using System.Threading;
	using Castle.Core;
	using NUnit.Framework;
	using Services.Timeout;
	using Services.Timeout.Messages;
	using Util;

	[TestFixture]
	public class When_scheduling_a_timeout_for_a_new_id :
		LocalAndRemoteTestContext
	{
		private ITimeoutRepository _repository;
		private TimeoutService _timeoutService;
		private Guid _correlationId;
		private DateTime _dateTime;

		protected override void Before_each()
		{
			_correlationId = CombGuid.NewCombGuid();
			_dateTime = DateTime.UtcNow + TimeSpan.FromSeconds(1);

			Container.AddComponentLifeStyle<ITimeoutRepository, InMemoryTimeoutRepository>(LifestyleType.Singleton);

			_repository = Container.Resolve<ITimeoutRepository>();

			Container.AddComponent<ScheduleTimeoutConsumer>();
			Container.AddComponent<CancelTimeoutConsumer>();

			_timeoutService = new TimeoutService(LocalBus, _repository);
			_timeoutService.Start();
		}

		protected override void After_each()
		{
			_timeoutService.Stop();
			_timeoutService.Dispose();
		}

		[Test]
		public void The_timeout_should_be_added_to_the_storage()
		{
			ManualResetEvent _timedOut = new ManualResetEvent(false);


			LocalBus.Subscribe<TimeoutExpired>(x => _timedOut.Set());

			LocalBus.Publish(new ScheduleTimeout(_correlationId, _dateTime));

			Stopwatch watch = Stopwatch.StartNew();

			Assert.IsTrue(_timedOut.WaitOne(TimeSpan.FromSeconds(5), true));

			watch.Stop();

			Debug.WriteLine(string.Format("Timeout took {0}ms", watch.ElapsedMilliseconds));
		}
	}
}