// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Pipeline
{
    using System;
    using MassTransit.Pipeline;
    using NUnit.Framework;
    using Policies;
    using Shouldly;
    using TestFramework;


    [TestFixture]
    public class Using_the_retry_filter
    {
        class A
        {
        }


        [Test]
        public void Should_retry_the_specified_times_and_fail()
        {
            int count = 0;
            IPipe<ConsumeContext<A>> pipe = Pipe.New<ConsumeContext<A>>(x =>
            {
                x.Retry(Retry.Interval(4, TimeSpan.FromMilliseconds(2)));
                x.Execute(payload =>
                {
                    count++;
                    throw new IntentionalTestException("Kaboom!");
                });
            });

            var context = new TestConsumeContext<A>(new A());

            var exception = Assert.Throws<IntentionalTestException>(async () => await pipe.Send(context));

            count.ShouldBe(5);
        }

        [Test]
        public void Should_support_overloading_downstream()
        {
            int count = 0;
            IPipe<ConsumeContext<A>> pipe = Pipe.New<ConsumeContext<A>>(x =>
            {
                x.Retry(Retry.Interval(4, TimeSpan.FromMilliseconds(2)));
                x.Retry(Retry.None);
                x.Execute(payload =>
                {
                    count++;
                    throw new IntentionalTestException("Kaboom!");
                });
            });

            var context = new TestConsumeContext<A>(new A());

            var exception = Assert.Throws<IntentionalTestException>(async () => await pipe.Send(context));

            count.ShouldBe(1);
        }

        [Test]
        public void Should_support_overloading_downstream_either_way()
        {
            int count = 0;
            IPipe<ConsumeContext<A>> pipe = Pipe.New<ConsumeContext<A>>(x =>
            {
                x.Retry(Retry.None);
                x.Retry(Retry.Interval(4, TimeSpan.FromMilliseconds(2)));
                x.Execute(payload =>
                {
                    count++;
                    throw new IntentionalTestException("Kaboom!");
                });
            });

            var context = new TestConsumeContext<A>(new A());

            var exception = Assert.Throws<IntentionalTestException>(async () => await pipe.Send(context));

            count.ShouldBe(5);
        }
    }
}