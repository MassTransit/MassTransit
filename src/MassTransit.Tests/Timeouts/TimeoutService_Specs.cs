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
namespace MassTransit.Tests.Timeouts
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using MassTransit.Saga;
    using MassTransit.Services.Timeout;
    using MassTransit.Services.Timeout.Messages;
    using MassTransit.Services.Timeout.Server;
    using NUnit.Framework;
    using TextFixtures;
    using MassTransit.Testing;

    [TestFixture]
    public class When_scheduling_a_timeout_for_a_new_id :
        LoopbackTestFixture
    {
        TimeoutService _timeoutService;
        Guid _correlationId;
        ISagaRepository<TimeoutSaga> _timeoutSagaRepository;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            _correlationId = NewId.NextGuid();

            _timeoutSagaRepository = SetupSagaRepository<TimeoutSaga>();

            _timeoutService = new TimeoutService(LocalBus, _timeoutSagaRepository);
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
            var timedOut = new FutureMessage<TimeoutExpired>();

            LocalBus.SubscribeHandler<TimeoutExpired>(timedOut.Set);

            LocalBus.HasSubscription<TimeoutExpired>(8.Seconds()).Any()
                .ShouldBeTrue("No subscription");

            LocalBus.Publish(new ScheduleTimeout(_correlationId, 2.Seconds()));

            Stopwatch watch = Stopwatch.StartNew();

            timedOut.IsAvailable(8.Seconds()).ShouldBeTrue("Did not get timeout message");

            watch.Stop();

            Debug.WriteLine(string.Format("Timeout took {0}ms", watch.ElapsedMilliseconds));
        }
    }
}