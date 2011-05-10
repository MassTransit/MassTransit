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
namespace MassTransit.TestFramework.Fixtures
{
	using System;
	using Magnum;
	using Magnum.Reflection;
	using MassTransit.Transports;
	using MassTransit.Transports.Loopback;
	using NUnit.Framework;
	using Saga;

	[TestFixture]
	public class SagaTestFixture<TSaga> :
		LocalTestFixture<LoopbackTransportFactory>
		where TSaga : class, ISaga
	{
		private InMemorySagaRepository<TSaga> _repository;

		public InMemorySagaRepository<TSaga> Repository
		{
			get { return _repository; }
		}

		public SagaTestFixture()
		{
			LocalUri = new Uri("loopback://localhost/mt_client");
			SagaId = CombGuid.Generate();
		}

		[TestFixtureSetUp]
		public void SagaTestFixtureSetup()
		{
			_repository = SetupSagaRepository<TSaga>();
		}

		protected Guid SagaId { get; private set; }

		protected TSaga Saga
		{
			get { return _repository.ShouldContainSaga(SagaId); }
		}

		protected TSaga AddExistingSaga(Guid sagaId, Action<TSaga> initializer)
		{
			TSaga saga = FastActivator<TSaga>.Create(sagaId);

			initializer(saga);

			_repository.Add(saga);

			return saga;
		}
	}
}