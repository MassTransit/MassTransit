// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Internals.Extensions;
    using NUnit.Framework;
    using Shouldly;
    using Util;


    [TestFixture]
    public class Supervisor_Specs
    {
        [Test]
        public async Task Should_complete_immediately_if_no_participants()
        {
            var supervisor = new TaskSupervisor("test");

            await supervisor.Completed;
        }

        [Test]
        public async Task Should_complete_if_participant_is_completed()
        {
            var supervisor = new TaskSupervisor("test");

            var stopwatch = Stopwatch.StartNew();
            var participant = supervisor.CreateParticipant("testA");
            Task.Delay(100)
                .ContinueWith(x => participant.SetComplete());

            await supervisor.Completed.WithTimeout(1000);

            stopwatch.Stop();

            stopwatch.Elapsed.ShouldBeGreaterThan(TimeSpan.FromMilliseconds(75));
        }

        [Test]
        public async Task Should_complete_if_scoped_participant_is_completed()
        {
            var supervisor = new TaskSupervisor("test");

            var stopwatch = Stopwatch.StartNew();

            var scope = supervisor.CreateScope("scope");

            var participant = supervisor.CreateParticipant("testA");
            scope.Completed.ContinueWith(x => scope.SetComplete());

            Task.Delay(100)
                .ContinueWith(x => participant.SetComplete());

            await supervisor.Completed;

            stopwatch.Stop();

            stopwatch.Elapsed.ShouldBeGreaterThan(TimeSpan.FromMilliseconds(75));
        }

        [Test]
        public async Task Should_fault_if_not_completed()
        {
            var supervisor = new TaskSupervisor("test");

            var stopwatch = Stopwatch.StartNew();
            var participant = supervisor.CreateParticipant("testA");

            Assert.Throws<OperationCanceledException>(async () => await supervisor.Completed.WithTimeout(1000));

            stopwatch.Stop();

            stopwatch.Elapsed.ShouldBeGreaterThan(TimeSpan.FromMilliseconds(750));
        }

    }
}