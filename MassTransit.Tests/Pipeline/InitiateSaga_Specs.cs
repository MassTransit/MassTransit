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
	using System.Collections;
	using System.Collections.Generic;
	using MassTransit.Internal;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Configuration;
	using MassTransit.Pipeline.Inspectors;
	using MassTransit.Pipeline.Interceptors;
	using MassTransit.Saga;
	using MassTransit.Saga.Pipeline;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class When_an_initiating_message_for_a_saga_arrives
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			_sagaId = Guid.NewGuid();

			_builder = MockRepository.GenerateMock<IObjectBuilder>();

			_endpoint = MockRepository.GenerateMock<IEndpoint>();
			_endpoint.Stub(x => x.Uri).Return(_uri);

			_repository = MockRepository.GenerateMock<ISagaRepository<SimpleSaga>>();

			_bus = MockRepository.GenerateMock<IServiceBus>();
			_bus.Stub(x => x.Endpoint).Return(_endpoint);

			_context = MockRepository.GenerateMock<IInterceptorContext>();
			_context.Stub(x => x.Builder).Return(_builder);

			_initiateSimpleSagaUnsubscribe = MockRepository.GenerateMock<UnsubscribeAction>();
			_completeSimpleSagaUnsubscribe = MockRepository.GenerateMock<UnsubscribeAction>();

			Hashtable empty = new Hashtable();

			_completeSink = new OrchestrateSagaMessageSink<SimpleSaga, CompleteSimpleSaga>(_context, _bus, _repository);
			_builder.Stub(x => x.GetInstance<OrchestrateSagaMessageSink<SimpleSaga, CompleteSimpleSaga>>(empty)).Return(_completeSink).IgnoreArguments();

			_initiateSink = new InitiateSagaMessageSink<SimpleSaga, InitiateSimpleSaga>(_context, _bus, _repository);
			_builder.Stub(x => x.GetInstance<InitiateSagaMessageSink<SimpleSaga, InitiateSimpleSaga>>(empty)).Return(_initiateSink).IgnoreArguments();

			_subscriptionEvent = MockRepository.GenerateMock<ISubscriptionEvent>();
			_subscriptionEvent.Expect(x => x.SubscribedTo<InitiateSimpleSaga>()).Repeat.Any().Return(() =>
				{
					_initiateSimpleSagaUnsubscribe();
					return true;
				});
			_subscriptionEvent.Expect(x => x.SubscribedTo<CompleteSimpleSaga>()).Repeat.Any().Return(() =>
				{
					_completeSimpleSagaUnsubscribe();
					return true;
				});

			_pipeline = MessagePipelineConfigurator.CreateDefault(_builder);
			_pipeline.Configure(x => x.Register(_subscriptionEvent));

			_remove = _pipeline.Subscribe<SimpleSaga>();

			PipelineViewer.Trace(_pipeline);
		}

		#endregion

		private readonly Uri _uri = new Uri("msmq://localhost/mt_client");
		private IEndpoint _endpoint;

		private IObjectBuilder _builder;
		private OrchestrateSagaMessageSink<SimpleSaga, CompleteSimpleSaga> _completeSink;
		private ISagaRepository<SimpleSaga> _repository;
		private IServiceBus _bus;
		private IInterceptorContext _context;
		private InitiateSagaMessageSink<SimpleSaga, InitiateSimpleSaga> _initiateSink;

		private Guid _sagaId;
		private SimpleSaga _saga;
		private MessagePipeline _pipeline;
		private UnsubscribeAction _remove;
		private ISubscriptionEvent _subscriptionEvent;
		private UnsubscribeAction _initiateSimpleSagaUnsubscribe;
		private UnsubscribeAction _completeSimpleSagaUnsubscribe;

		[Test]
		public void Should_publish_subscriptions_for_saga_subscriptions()
		{
			_subscriptionEvent.VerifyAllExpectations();
		}

		[Test]
		public void Should_remove_subscriptions_for_saga_subscriptions()
		{
			_remove();

			_subscriptionEvent.VerifyAllExpectations();

			_initiateSimpleSagaUnsubscribe.AssertWasCalled(x => x());
			_completeSimpleSagaUnsubscribe.AssertWasCalled(x => x());
		}

		[Test]
		public void The_saga_should_be_created_when_an_initiating_message_is_received()
		{
			_saga = MockRepository.GenerateMock<SimpleSaga>();

			_repository.Stub(x => x.InitiateNewSaga(_sagaId)).Return(new List<SimpleSaga> {_saga}.GetEnumerator());

			InitiateSimpleSaga initiateSimpleSaga = new InitiateSimpleSaga(_sagaId);

			_pipeline.Dispatch(initiateSimpleSaga);

			Assert.IsTrue(_saga.Initiated);
		}

		[Test]
		public void The_saga_should_be_loaded_when_an_orchestrated_message_is_received()
		{
			_saga = MockRepository.GenerateMock<SimpleSaga>();

			_repository.Stub(x => x.OrchestrateExistingSaga(_sagaId)).Return(new List<SimpleSaga> { _saga }.GetEnumerator());

			CompleteSimpleSaga completeMessage = new CompleteSimpleSaga(_sagaId);

			_pipeline.Dispatch(completeMessage);

			Assert.IsTrue(_saga.Completed);
		}
	}

	public class SimpleSaga :
		InitiatedBy<InitiateSimpleSaga>,
		Orchestrates<CompleteSimpleSaga>,
		ISaga
	{
		private Guid _correlationId;

		public SimpleSaga()
		{
		}

		public SimpleSaga(Guid correlationId)
		{
			_correlationId = correlationId;
		}

		public bool Completed { get; private set; }
		public bool Initiated { get; private set; }

		public void Consume(InitiateSimpleSaga message)
		{
			Initiated = true;
		}

		public Guid CorrelationId
		{
			get { return _correlationId; }
			set { _correlationId = value; }
		}

		public IServiceBus Bus { get; set; }

		public void Consume(CompleteSimpleSaga message)
		{
			Completed = true;
		}
	}

	[Serializable]
	public class InitiateSimpleSaga :
		SimpleSagaMessageBase
	{
		public InitiateSimpleSaga()
		{
		}

		public InitiateSimpleSaga(Guid correlationId) :
			base(correlationId)
		{
		}
	}

	[Serializable]
	public class CompleteSimpleSaga :
		SimpleSagaMessageBase
	{
		public CompleteSimpleSaga()
		{
		}

		public CompleteSimpleSaga(Guid correlationId) :
			base(correlationId)
		{
		}
	}

	public class SimpleSagaMessageBase : CorrelatedBy<Guid>
	{
		private Guid _correlationId;

		public SimpleSagaMessageBase()
		{
		}

		public SimpleSagaMessageBase(Guid correlationId)
		{
			_correlationId = correlationId;
		}

		public Guid CorrelationId
		{
			get { return _correlationId; }
			set { _correlationId = value; }
		}
	}
}