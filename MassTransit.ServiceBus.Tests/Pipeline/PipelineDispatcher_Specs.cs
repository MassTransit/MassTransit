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
namespace MassTransit.ServiceBus.Tests
{
	using System;
	using Messages;
	using NUnit.Framework;
	using Pipeline;

	[TestFixture]
	public class When_subscription_a_component_to_the_pipeline
	{
		[Test]
		public void The_appropriate_handler_should_be_added()
		{
			IndiscriminantConsumer<PingMessage> consumer = new IndiscriminantConsumer<PingMessage>();

			MessageRouter<PingMessage> router = new MessageRouter<PingMessage>();
			MessagePipeline<PingMessage> pipeline = new MessagePipeline<PingMessage>(router);

			consumer.SubscribeTo(router);

			PingMessage message = new PingMessage();

			pipeline.Dispatch(message, x => true);

			Assert.AreEqual(message, consumer.Consumed);
		}

		[Test]
		public void The_pipeline_should_have_insertable_items()
		{
			// two consumers, one for each type of message

			IndiscriminantConsumer<PingMessage> pingConsumer = new IndiscriminantConsumer<PingMessage>();
			IndiscriminantConsumer<PongMessage> pongConsumer = new IndiscriminantConsumer<PongMessage>();

			MessageRouter<PingMessage> pingRouter = new MessageRouter<PingMessage>();
			MessageRouter<PongMessage> pongRouter = new MessageRouter<PongMessage>();

			Func<bool> pingToken = pingConsumer.SubscribeTo(pingRouter);
			Func<bool> pongToken = pongConsumer.SubscribeTo(pongRouter);

			MessageTranslator<object, PingMessage> pingTranslator = new MessageTranslator<object, PingMessage>(pingRouter);
			MessageTranslator<object, PongMessage> pongTranslator = new MessageTranslator<object, PongMessage>(pongRouter);

			MessageRouter<object> objectRouter = new MessageRouter<object>();
			objectRouter.Connect(pingTranslator);
			objectRouter.Connect(pongTranslator);

			MessagePipeline<object> pipeline = new MessagePipeline<object>(objectRouter);

			PingMessage pingMessage = new PingMessage();
			PongMessage pongMessage = new PongMessage();

			pipeline.Dispatch(pingMessage, accept => true);
			pipeline.Dispatch(pongMessage, accept => true);

			Assert.AreEqual(pingMessage, pingConsumer.Consumed);
			Assert.AreEqual(pongMessage, pongConsumer.Consumed);

			pingToken();
			pongToken();
		}

		[Test]
		public void The_subscriptions_should_be_a_separate_concern_from_the_pipeline()
		{
			IndiscriminantConsumer<PingMessage> consumer = new IndiscriminantConsumer<PingMessage>();

			MessageRouter<PingMessage> router = new MessageRouter<PingMessage>();

			MessagePipeline<PingMessage> pipeline = new MessagePipeline<PingMessage>(router);

			consumer.SubscribeTo(router);

			PingMessage message = new PingMessage();

			pipeline.Dispatch(message, x => true);

			Assert.AreEqual(message, consumer.Consumed);
		}

		[Test]
		public void When_nobody_wants_the_message_it_should_not_be_accepted()
		{
			MessageRouter<PingMessage> router = new MessageRouter<PingMessage>();

			PingMessage message = new PingMessage();

			bool accepted = false;

			MessagePipeline<PingMessage> pipeline = new MessagePipeline<PingMessage>(router);

			pipeline.Dispatch(message, x => accepted = true);

			Assert.IsFalse(accepted);
		}

		[Test]
		public void When_somebody_gets_the_message_it_should_be_accepted()
		{
			MessageRouter<PingMessage> router = new MessageRouter<PingMessage>();

			IndiscriminantConsumer<PingMessage> consumer = new IndiscriminantConsumer<PingMessage>();

			consumer.SubscribeTo(router);

			PingMessage message = new PingMessage();

			bool accepted = false;

			MessagePipeline<PingMessage> pipeline = new MessagePipeline<PingMessage>(router);

			pipeline.Dispatch(message, x => accepted = true);

			Assert.IsTrue(accepted);
		}
	}
}