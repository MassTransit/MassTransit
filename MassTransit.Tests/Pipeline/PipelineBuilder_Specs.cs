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
namespace MassTransit.Tests.Pipeline
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
	using Magnum.Common.DateTimeExtensions;
	using MassTransit.Pipeline;
	using Messages;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class When_building_a_pipeline
	{
		[SetUp]
		public void Setup()
		{
			_builder = MockRepository.GenerateMock<IObjectBuilder>();
		}

		private IObjectBuilder _builder;

		[Test]
		public void A_indiscriminate_consumer_should_get_added()
		{
			IndiscriminantConsumer<PingMessage> consumer = new IndiscriminantConsumer<PingMessage>();

			ISubscribeContext context = MockRepository.GenerateMock<ISubscribeContext>();
			context.Expect(x => x.HasMessageTypeBeenDefined(typeof (PingMessage))).Return(false);

			SubscribePipeline model = new SubscribePipeline(_builder);

			model.Subscribe(consumer);

			context.AssertWasCalled(x => x.HasMessageTypeBeenDefined(typeof (PingMessage)));
		}

		[Test]
		public void Batch_composition_should_work()
		{
			TestBatchConsumer<IndividualBatchMessage, Guid> batchConsumer = new TestBatchConsumer<IndividualBatchMessage, Guid>();

			SubscribePipeline pipeline = new SubscribePipeline(MockRepository.GenerateMock<IObjectBuilder>());

			pipeline.Subscribe(batchConsumer);

			Guid batchId = Guid.NewGuid();
			const int _batchSize = 1;
			for (int i = 0; i < _batchSize; i++)
			{
				IndividualBatchMessage message = new IndividualBatchMessage(batchId, _batchSize);

				pipeline.Dispatch(message);
			}

			TimeSpan _timeout = 5.Seconds();

			batchConsumer.ShouldHaveReceivedBatch(_timeout);
		}

		[Test]
		public void Reflection_test()
		{
			Type t = typeof (ParticularConsumer);

			foreach (Type interfaceType in t.GetInterfaces())
			{
				Trace.WriteLine(interfaceType.FullName);
			}
		}

		[Test]
		public void The_pipeline_should_be_happy()
		{
			IndiscriminantConsumer<PingMessage> consumer = new IndiscriminantConsumer<PingMessage>();

			SubscribePipeline pipeline = new SubscribePipeline(MockRepository.GenerateMock<IObjectBuilder>());

			pipeline.Subscribe(consumer);

			pipeline.Dispatch(new PingMessage());

			Assert.IsNotNull(consumer.Consumed);
		}

		[Test]
		public void When_subscription_using_the_builder_intercepters_should_be_called()
		{
			IndiscriminantConsumer<PingMessage> consumer = new IndiscriminantConsumer<PingMessage>();

			ISubscribeContext context = MockRepository.GenerateMock<ISubscribeContext>();
			ISubscribeInterceptor interceptor = MockRepository.GenerateMock<ISubscribeInterceptor>();
			interceptor.Expect(x => x.Subscribe(context, consumer)).Return(new List<Func<bool>>());


			interceptor.AssertWasCalled(x => x.Subscribe(context, consumer));
		}
	}

	public class MessagePipelineBuilder
	{
		private readonly MessagePipeline _pipeline;
		private readonly MessageRouter<object> _router;
		private readonly Dictionary<Type, IMessageTypeRouterBuilder> _typeBuilders = new Dictionary<Type, IMessageTypeRouterBuilder>();

		public MessagePipeline Pipeline
		{
			get { return _pipeline; }
		}

		public Func<bool> Subscribe<TMessage>(Consumes<TMessage>.All consumer)
			where TMessage : class
		{
			IMessageTypeRouterBuilder builder;
			if (_typeBuilders.TryGetValue(typeof (TMessage), out builder) == false)
			{
				builder = new MessageTypeRouterBuilder<TMessage>(_router);

				_typeBuilders.Add(typeof (TMessage), builder);
			}

			return builder.Subscribe(consumer);
		}
	}

	public class MessageTypeRouterBuilder<TMessage> :
		IMessageTypeRouterBuilder
		where TMessage : class
	{
		private readonly MessageRouter<TMessage> _router;
		private readonly MessageTranslator<object, TMessage> _translator;

		public MessageTypeRouterBuilder(MessageRouter<object> objectRouter)
		{
			_router = new MessageRouter<TMessage>();

			_translator = new MessageTranslator<object, TMessage>(_router);

			objectRouter.Connect(_translator);
		}

		public Func<bool> Subscribe<T>(Consumes<T>.All consumer) where T : class
		{
			Func<bool> token = _router.Connect(new MessageSink<TMessage>(message => (Consumes<TMessage>.All) consumer));

			return token;
		}
	}

	public interface IMessageTypeRouterBuilder
	{
		Func<bool> Subscribe<TMessage>(Consumes<TMessage>.All consumer) where TMessage : class;
	}
}