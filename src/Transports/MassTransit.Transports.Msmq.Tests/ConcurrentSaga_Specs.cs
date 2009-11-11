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
namespace MassTransit.Transports.Msmq.Tests
{
	using System;
	using System.Linq;
	using System.Threading;
	using Infrastructure.Saga;
	using Infrastructure.Tests.Sagas;
	using log4net;
	using Magnum;
	using NUnit.Framework;
	using Rhino.Mocks;
	using Saga;
	using TestFixtures;

	[TestFixture, Category("Integration")]
	public class Sending_multiple_initiating_messages_should_not_fail_badly :
		MsmqConcurrentSagaTestFixtureBase
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Sending_multiple_initiating_messages_should_not_fail_badly));

		private ISagaRepository<ConcurrentLegacySaga> _sagaRepository;

		protected override void EstablishContext()
		{
			base.EstablishContext();

			_sagaRepository = new NHibernateSagaRepositoryForContainers<ConcurrentLegacySaga>(SessionFactory);
			ObjectBuilder.Stub(x => x.GetInstance<ISagaRepository<ConcurrentLegacySaga>>()).Return(_sagaRepository);
		}

		[Test]
		public void Should_process_the_messages_in_order_and_not_at_the_same_time()
		{
			Guid transactionId = CombGuid.Generate();

			_log.Info("Creating transaction for " + transactionId);

			const int startValue = 1;

			var startConcurrentSaga = new StartConcurrentSaga {CorrelationId = transactionId, Name = "Chris", Value = startValue};

			LocalBus.Endpoint.Send(startConcurrentSaga);
			LocalBus.Endpoint.Send(startConcurrentSaga);

			_log.Info("Just published the start message");

			UnsubscribeAction unsubscribeAction = LocalBus.Subscribe<ConcurrentLegacySaga>();

			Thread.Sleep(1500);

			const int nextValue = 2;
			var continueConcurrentSaga = new ContinueConcurrentSaga {CorrelationId = transactionId, Value = nextValue};

			LocalBus.Publish(continueConcurrentSaga);
			_log.Info("Just published the continue message");
			Thread.Sleep(8000);

			unsubscribeAction();
			foreach (ConcurrentLegacySaga saga in _sagaRepository.Where(x => true))
			{
				_log.Info("Found saga: " + saga.CorrelationId);
			}

			int currentValue = _sagaRepository.Where(x => x.CorrelationId == transactionId).First().Value;

			Assert.AreEqual(nextValue, currentValue);
		}
	}
}