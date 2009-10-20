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
	using Magnum.StateMachine;
	using MassTransit.Actors;

	public class RequestResponseActor :
		StateDrivenActor<RequestResponseActor>
	{
		static RequestResponseActor()
		{
			Define(() =>
				{
					Correlate(InitiateRequest).By((actor, message) => actor.CorrelationId == message.Id);
					Correlate(ResponseReceived).By((actor, message) => actor.CorrelationId == message.RequestId);

					Initially(
						When(InitiateRequest)
							.Then((actor, message) =>
								{
									actor.Name = message.Name;

									actor.Bus.Publish(new ActorRequest {Id = message.Id, Name = message.Name},
										x => x.SendResponseTo(actor.Bus.Endpoint));
								})
							.TransitionTo(Active));

					During(Active,
						When(ResponseReceived)
							.Then((actor, message) =>
								{
									actor.Age = message.Age;

									actor.SetAsCompleted();
								})
							.TransitionTo(Completed));

					Anytime(When(InitiateRequest));
				});
		}

		public RequestResponseActor(Guid correlationId)
			: base(correlationId)
		{
			CorrelationId = correlationId;
		}

		public RequestResponseActor(Guid correlationId, AsyncCallback callback, object state)
			: base(correlationId, callback, state)
		{
		}

		public static Event<InitiateActorRequest> InitiateRequest { get; set; }
		public static Event<ActorResponse> ResponseReceived { get; set; }

		public static State Initial { get; set; }
		public static State Completed { get; set; }
		public static State Active { get; set; }

		public virtual string Name { get; set; }

		public virtual int Age { get; set; }
	}
}