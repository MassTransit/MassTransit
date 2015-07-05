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
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Filters.Latest;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    [TestFixture]
    public class Using_the_latest_filter_on_the_pipe
    {
        [Test, Explicit]
        public async void Should_keep_track_of_only_the_last_value()
        {
            ILatestFilter<ConsumeContext<A>> latestFilter = null;

            IPipe<ConsumeContext<A>> pipe = Pipe.New<ConsumeContext<A>>(x =>
            {
                x.UseLatest(l => l.Created += filter => latestFilter = filter);
                x.UseExecute(payload =>
                {
                });
            });

            latestFilter.ShouldNotBe(null);

            for (int i = 0; i <= 100; i++)
            {
                var context = new TestConsumeContext<A>(new A {Index = i});
                await pipe.Send(context);
            }

            ConsumeContext<A> latest = await latestFilter.Latest;

            latest.Message.Index.ShouldBe(100);
        }


        class A
        {
            public int Index { get; set; }
        }
    }
}