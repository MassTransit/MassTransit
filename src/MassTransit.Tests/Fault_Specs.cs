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
namespace MassTransit.Tests
{
	using System;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using NUnit.Framework;
	using TextFixtures;

	[TestFixture]
	public class When_a_message_fault_occurs :
		LoopbackTestFixture
	{
		public class SmartConsumer :
			Consumes<Fault<Hello>>.All
		{
			readonly FutureMessage<Fault<Hello>> _fault = new FutureMessage<Fault<Hello>>();

			public FutureMessage<Fault<Hello>> Fault
			{
				get { return _fault; }
			}

			public void Consume(Fault<Hello> message)
			{
				_fault.Set(message);
			}
		}

		public class Hello
		{
		}

		[Test]
		public void Should_receive_a_fault_message()
		{
			var consumer = new SmartConsumer();

			LocalBus.SubscribeHandler((ConsumeContext<Hello> context) =>  { throw new AccessViolationException("Crap!"); });

			LocalBus.SubscribeInstance(consumer);

		    LocalBus.Publish(new Hello(), x => x.SendFaultTo(LocalBus));

			consumer.Fault.IsAvailable(300.Seconds()).ShouldBeTrue();
		}
	}

	[TestFixture]
	public class When_a_correlated_message_fault_is_received :
		LoopbackTestFixture
	{
		public class SmartConsumer :
			Consumes<Fault<Hello, Guid>>.All
		{
			readonly Guid _id = Guid.NewGuid();

			readonly FutureMessage<Fault<Hello, Guid>> _fault = new FutureMessage<Fault<Hello, Guid>>();

			public FutureMessage<Fault<Hello, Guid>> Fault
			{
				get { return _fault; }
			}

			public void Consume(Fault<Hello, Guid> message)
			{
                if(message.CorrelationId == _id)
    				_fault.Set(message);
			}

			public Guid CorrelationId
			{
				get { return _id; }
			}
		}

		public class Hello :
			CorrelatedBy<Guid>
		{
			protected Hello()
			{
			}

			public Hello(Guid id)
			{
				CorrelationId = id;
			}

			public Guid CorrelationId { get; set; }
		}


		[Test]
		public void Should_receive_a_fault_message()
		{
			var consumer = new SmartConsumer();

            LocalBus.SubscribeHandler((ConsumeContext<Hello> context) => { throw new AccessViolationException("Crap!"); });

			LocalBus.SubscribeInstance(consumer);

			LocalBus.Publish(new Hello(consumer.CorrelationId));

			consumer.Fault.IsAvailable(8.Seconds()).ShouldBeTrue();
		}
	}
}