// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Testing
{
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using Shouldly;


    [Explicit]
    public class When_a_consumer_with_multiple_message_consumers_is_tested
    {
        IConsumerTest<IBusTestScenario, TwoMessageConsumer> _test;

        [SetUp]
        public void A_consumer_is_being_tested()
        {
            _test = TestFactory.ForConsumer<TwoMessageConsumer>()
                .New(x =>
                {
                    x.UseConsumerFactory(() => new TwoMessageConsumer());

                    x.Send(new A(), (scenario, context) => context.ResponseAddress = scenario.Bus.Address);
                    x.Send(new B(), (scenario, context) => context.ResponseAddress = scenario.Bus.Address);
                });

            _test.Execute();
        }

        [TearDown]
        public void Teardown()
        {
            _test.Dispose();
            _test = null;
        }

        [Test]
        public void Should_have_sent_the_aa_response_from_the_consumer()
        {
            _test.Sent.Select<Aa>().Any().ShouldBe(true);
        }

        [Test]
        public void Should_have_sent_the_bb_response_from_the_consumer()
        {
            _test.Sent.Select<Bb>().Any().ShouldBe(true);
        }

        [Test]
        public void Should_have_called_the_consumer_a_method()
        {
            _test.Consumer.Received.Select<A>().Any().ShouldBe(true);
        }

        [Test]
        public void Should_have_called_the_consumer_b_method()
        {
            _test.Consumer.Received.Select<B>().Any().ShouldBe(true);
        }


        class A
        {
        }


        class Aa
        {
        }


        class B
        {
        }


        class Bb
        {
        }


        class TwoMessageConsumer :
            IConsumer<A>,
            IConsumer<B>
        {
            public Task Consume(ConsumeContext<A> context)
            {
                return context.RespondAsync(new Aa());
            }

            public Task Consume(ConsumeContext<B> context)
            {
                return context.RespondAsync(new Bb());
            }
        }
    }
}