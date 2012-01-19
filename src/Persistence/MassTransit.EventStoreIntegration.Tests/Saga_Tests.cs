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
namespace MassTransit.EventStoreIntegration.Tests
{
	using System;
	using System.Threading;
	using EventStore;
	using EventStore.Dispatcher;
	using Magnum;
	using Magnum.Concurrency;
	using Magnum.Extensions;
	using MassTransit.Tests;
	using MassTransit.Tests.TextFixtures;
	using NUnit.Framework;
	//using Pipeline.Inspectors;
	using Saga;
	using log4net.Config;
	using CS = Saga.SagaStateMachine<Cashier>;
	using SharpTestsEx;
	using System.Linq;

	public class Saga_Tests
		: LoopbackTestFixture
	{
		Guid conversationId = CombGuid.Generate();
		Atomic<FutureMessage<Commit>> commitDispatched = Atomic.Create(new FutureMessage<Commit>());
		ISagaRepository<Cashier> _sagaRepository;
		IStoreEvents es;

		protected override void EstablishContext()
		{
			BasicConfigurator.Configure();

			base.EstablishContext();

			es = Wireup.Init()
				.UsingInMemoryPersistence()
				.LogToConsoleWindow()
				.UsingSynchronousDispatchScheduler(new TestDispatcher(commitDispatched))
				.Build();

			_sagaRepository = new SagaEventStoreRepository<Cashier>(es);
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			es.Dispose();
		}

		[Test]
		public void can_consume_es_repo_cashier_saga()
		{
			var finally_ran = new ManualResetEventSlim(false);
			var unsub = LocalBus.SubscribeSaga(_sagaRepository);
			//PipelineViewer.Trace(LocalBus.InboundPipeline);
			
			try
			{
				LocalBus.Publish(new NewOrder { CorrelationId = conversationId, Item = "Latte", Name = "Latte Promiscio", Size = "grande" });

				var futureMessage = commitDispatched.Value;
				Assert.IsTrue(futureMessage.IsAvailable(5.Seconds()), "no commits were done");
				Assert.That(futureMessage.Message.CommitId, Is.Not.EqualTo(conversationId), 
					"because this wouldn't be unique accross many messages to a saga");
			}
			finally
			{
				unsub();
				finally_ran.Set();
			}
			finally_ran.Wait();
		}

		[Test]
		public void can_rehydrate_the_saga()
		{
			// given we have consumed new order
			can_consume_es_repo_cashier_saga();

			// set up
			var unsub = LocalBus.SubscribeSaga(_sagaRepository);

			try
			{
				// we expect a commit to happend
				var nextMessage = new FutureMessage<Commit>();
				commitDispatched.Set(_ => nextMessage);

				// when we consume a message:
				LocalBus.Publish(new SubmitPayment{ Amount = 30.0m, CorrelationId = conversationId, PaymentType = PaymentType.Cash});

				Assert.IsTrue(nextMessage.IsAvailable(5.Seconds()), "no commits were done");

				nextMessage.Message.Events.Select(x => x.Body)
					.Satisfy(events =>
						events.Count(evt => evt is SagaStateDelta) != 0 // has state change
						&& (from e in events 
						    where e is SagaStateDelta
						    let state = e as SagaStateDelta
							select state).First().StateName == "Completed");
			}
			finally
			{
				unsub();
			}
		}
	}

	public class TestDispatcher : IDispatchCommits
	{
		readonly Atomic<FutureMessage<Commit>> _waitForDispatch;

		public TestDispatcher(Atomic<FutureMessage<Commit>> waitForDispatch)
		{
			_waitForDispatch = waitForDispatch;
		}

		public void Dispatch(Commit commit)
		{
			_waitForDispatch.Value.Set(commit);
		}

		public void Dispose()
		{
		}
	}
}