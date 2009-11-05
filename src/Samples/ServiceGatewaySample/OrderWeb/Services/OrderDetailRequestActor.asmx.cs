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
namespace OrderWeb.Services
{
	using System;
	using System.Diagnostics;
	using GatewayService.Interfaces;
	using Magnum.StateMachine;
	using MassTransit;
	using MassTransit.Actors;

	public class OrderDetailRequestActor :
		StateDrivenActor<OrderDetailRequestActor>
	{
		static OrderDetailRequestActor()
		{
			Define(Saga);
		}

		private static void Saga()
		{
			Correlate(InitiateRequest)
				.By((actor, message) => actor.CorrelationId == message.RequestId);
			Correlate(DetailsReceived)
				.By((actor, message) => actor.OrderId == message.OrderId &&
				                        actor.CustomerId == message.CustomerId);

			Initially(
				When(InitiateRequest)
					.Then((actor, message) =>
						{
							actor.Stopwatch = Stopwatch.StartNew();
							actor.CustomerId = message.CustomerId;
							actor.OrderId = message.OrderId;

							actor.Bus.Publish(new RetrieveOrderDetailsRequest
								{
									OrderId = message.OrderId,
									CustomerId = message.CustomerId,
								}, x => x.SendResponseTo(actor.Bus.Endpoint));
						})
					.TransitionTo(WaitingForDetails),
				When(DetailsReceived)
					.Then((actor, message) => { throw new InvalidOperationException("We have not yet added the actor to the system"); })
				);

			During(WaitingForDetails,
				When(DetailsReceived)
					.Then((actor, message) =>
						{
							actor.OrderCreated = message.Created;
							actor.OrderStatus = message.Status;

							actor.Stopwatch.Stop();
						})
					.TransitionTo(Completed));

			Anytime(When(Completed.Enter).Then(actor => actor.Complete()));

			Anytime(When(InitiateRequest).Then(x => { }));
		}

		protected Stopwatch Stopwatch { get; set; }

		public string CustomerId { get; set; }
		public string OrderId { get; set; }
		public DateTime? OrderCreated { get; set; }
		public OrderStatus OrderStatus { get; set; }

		public static State Initial { get; set; }
		public static State Completed { get; set; }
		public static State WaitingForDetails { get; set; }

		public static Event<InitiateOrderDetailsRequest> InitiateRequest { get; set; }
		public static Event<OrderDetailsReceived> DetailsReceived { get; set; }

		public OrderDetailRequestActor(Guid correlationId)
			: base(correlationId)
		{
			CorrelationId = correlationId;
		}

		protected OrderDetailRequestActor()
		{
		}

		public IAsyncResult BeginAction(AsyncCallback callback, object state)
		{
			SetAsyncResult(callback, state);

			return this;
		}

		private void Complete()
		{
			SetAsCompleted();
		}
	}
}