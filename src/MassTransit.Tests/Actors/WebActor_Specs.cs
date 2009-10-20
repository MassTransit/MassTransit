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
namespace MassTransit.Tests.Actors
{
	using System;
	using System.Threading;
	using Magnum;
	using Magnum.DateTimeExtensions;
	using Magnum.StateMachine;
	using MassTransit.Actors;
	using MassTransit.Pipeline.Inspectors;
	using MassTransit.Saga;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TextFixtures;

	[TestFixture]
	public class A_web_actor_should_get_wired :
		LoopbackTestFixture
	{
		private IActorRepository<RequestResponseActor> _repository;

		protected override void EstablishContext()
		{
			base.EstablishContext();

			_repository = new InMemoryActorRepository<RequestResponseActor>();

			ObjectBuilder.Stub(x => x.GetInstance<ISagaRepository<RequestResponseActor>>())
				.Return(_repository);
		}

		[Test]
		public void Trace_Visitor()
		{
			var actor = new RequestResponseActor(Guid.NewGuid());

			StateMachineInspector.Trace(actor);
		}

		[Test]
		public void FirstTestName()
		{
			LocalBus.Subscribe<ActorRequest>(request => { CurrentMessage.Respond(new ActorResponse {RequestId = request.Id, Age = 27}); });

			LocalBus.Subscribe<RequestResponseActor>();

			PipelineViewer.Trace(LocalBus.InboundPipeline);

			var called = new ManualResetEvent(false);

			Guid id = CombGuid.Generate();

			var actor = new RequestResponseActor(id);
				
				actor.BeginAction(asyncResult => { called.Set(); }, null);

			_repository.Add(actor);

			LocalBus.Endpoint.Send(new InitiateActorRequest {Id = id, Name = "Chris"});

			called.WaitOne(555.Seconds()).ShouldBeTrue("Well shit, why didn't it work");
		}
	}

	public class ActorRequest
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}

	public class InitiateActorRequest
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}

	public class ActorResponse
	{
		public Guid RequestId { get; set; }
		public int Age { get; set; }
	}
}