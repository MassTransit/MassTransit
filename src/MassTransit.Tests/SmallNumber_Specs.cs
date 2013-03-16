// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using MassTransit.Testing;
    using NUnit.Framework;


    [TestFixture]
    public class When_a_small_decimal_is_serialized
    {
        [Test]
        public void Should_properly_deserialize()
        {
            ConsumerTest<BusTestScenario, SmallNumberConsumer> testConsumer = TestFactory
                .ForConsumer<SmallNumberConsumer>()
                .New(x =>
                    {
                        x.ConstructUsing(() => new SmallNumberConsumer());
                        x.Send(new SmallNumberMessage {SmallNumber = _smallNumber});
                    });

            testConsumer.Execute();

            Assert.IsTrue(
                testConsumer.Sent.Any<SmallNumberMessage>(m => m.Context.Message.SmallNumber == _smallNumber),
                "SmallNumberMessage not sent");
            Assert.IsTrue(
                testConsumer.Consumer.Received.Any<SmallNumberMessage>(
                    (c,m) => m.SmallNumber == _smallNumber), "SmallNumberMessage not received");
        }

        const decimal _smallNumber = 0.000001M;


        public class SmallNumberConsumer : Consumes<SmallNumberMessage>.All
        {
            public void Consume(SmallNumberMessage message)
            {
            }
        }


        public class SmallNumberMessage
        {
            public decimal SmallNumber { get; set; }
        }
    }
}