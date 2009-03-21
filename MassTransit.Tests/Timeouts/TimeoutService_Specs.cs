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
namespace MassTransit.Tests.Timeouts
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Magnum;
    using Magnum.DateTimeExtensions;
    using MassTransit.Services.Timeout;
    using MassTransit.Services.Timeout.Messages;
    using NUnit.Framework;
    using Rhino.Mocks;
    using TextFixtures;

	[TestFixture]
    public class When_scheduling_a_timeout_for_a_new_id :
        LoopbackLocalAndRemoteTestFixture
    {
        private ITimeoutRepository _repository;
        private TimeoutService _timeoutService;
        private Guid _correlationId;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            _correlationId = CombGuid.Generate();

            _repository = new InMemoryTimeoutRepository();
            ObjectBuilder.Stub(x => x.GetInstance<ITimeoutRepository>()).Return(_repository);

            ObjectBuilder.Stub(x => x.GetInstance<ScheduleTimeoutConsumer>()).Return(new ScheduleTimeoutConsumer(_repository));
            ObjectBuilder.Stub(x => x.GetInstance<CancelTimeoutConsumer>()).Return(new CancelTimeoutConsumer(_repository));

            _timeoutService = new TimeoutService(LocalBus, _repository);
            _timeoutService.Start();
        }

        protected override void TeardownContext()
        {
            base.TeardownContext();

            _timeoutService.Stop();
            _timeoutService.Dispose();
        }

        [Test]
        public void The_timeout_should_be_added_to_the_storage()
        {
            var _timedOut = new ManualResetEvent(false);


            LocalBus.Subscribe<TimeoutExpired>(x => _timedOut.Set());

            LocalBus.Publish(new ScheduleTimeout(_correlationId, 1.Seconds()));

            Stopwatch watch = Stopwatch.StartNew();

            Assert.IsTrue(_timedOut.WaitOne(TimeSpan.FromSeconds(5), true));

            watch.Stop();

            Debug.WriteLine(string.Format("Timeout took {0}ms", watch.ElapsedMilliseconds));
        }
    }
}