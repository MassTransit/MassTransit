// Copyright 2007-2010 The Apache Software Foundation.
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
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using MassTransit.Pipeline.Inspectors;
    using NUnit.Framework;
    using TestFramework;
    using TextFixtures;

    [TestFixture]
    public class Sending_a_message_that_implements_an_interface :
        LoopbackLocalAndRemoteTestFixture
    {
        [Test]
        public void Should_deliver_the_message_to_an_interested_consumer()
        {
            var first = new Future<FirstMessageContract>();

            RemoteBus.SubscribeHandler<FirstMessageContract>(first.Complete);

            LocalBus.ShouldHaveSubscriptionFor<FirstMessageContract>();
            
            PipelineViewer.Trace(RemoteBus.InboundPipeline);

            var message = new SomeMessageContract("Joe", 27);

            LocalBus.Publish(message);

            first.WaitUntilCompleted(1.Seconds()).ShouldBeTrue();
        }

        [Test]
        public void Should_deliver_the_message_to_an_both_interested_consumers()
        {
            var first = new Future<FirstMessageContract>();
            var second = new Future<SecondMessageContract>();

            // These can't be on the same bus, because we only send a message to an endpoint once
            // maybe we can do something here by changing the outbound context to keep track of tmessage/endpoint uri
            RemoteBus.SubscribeHandler<FirstMessageContract>(first.Complete);
            LocalBus.SubscribeHandler<SecondMessageContract>(second.Complete);

            LocalBus.ShouldHaveSubscriptionFor<FirstMessageContract>();

            var message = new SomeMessageContract("Joe", 27);

            LocalBus.Publish(message);

            first.WaitUntilCompleted(1.Seconds()).ShouldBeTrue();
            second.WaitUntilCompleted(1.Seconds()).ShouldBeTrue();
        }


        public interface FirstMessageContract
        {
            string Name { get; }
        }

        public interface SecondMessageContract
        {
            string Name { get; }
            int Age { get; }
        }

        public class SomeMessageContract :
            FirstMessageContract,
            SecondMessageContract
        {
            public SomeMessageContract(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public string Name { get; private set; }
            public int Age { get; private set; }
        }
    }
}