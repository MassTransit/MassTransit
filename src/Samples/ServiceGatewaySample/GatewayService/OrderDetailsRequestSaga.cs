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
namespace GatewayService
{
	using System;
	using Interfaces;
	using Magnum.StateMachine;
	using MassTransit;
	using MassTransit.Saga;
	using Messages;

	public class OrderDetailsRequestSaga :
		SagaStateMachine<OrderDetailsRequestSaga>,
		ISaga
	{
		static OrderDetailsRequestSaga()
		{
			Define(Saga);
		}

		private static void Saga()
		{
			Correlate(RequestReceived).By((saga, message) => saga.CustomerId == message.CustomerId && saga.OrderId == message.OrderId);
			Correlate(ResponseReceived).By((saga, message) => saga.CustomerId == message.CustomerId && saga.OrderId == message.OrderId);

			Initially(
				When(RequestReceived)
					.Then((saga, request) =>
						{
							saga.OrderId = request.OrderId;
							saga.CustomerId = request.CustomerId;

							saga.Bus.Publish(new SendOrderDetailsRequest
								{
									RequestId = saga.CorrelationId,
									CustomerId = request.CustomerId,
									OrderId = request.OrderId,
								});
						})
					.TransitionTo(WaitingForResponse));

			During(WaitingForResponse,
				When(ResponseReceived)
					.Then((saga, response) =>
						{
							saga.OrderCreated = response.Created;
							saga.OrderStatus = response.Status;

							saga.Bus.Publish(new OrderDetails
								{
                                   CustomerId = saga.CustomerId,
								   OrderId = saga.OrderId,
								   Created = saga.OrderCreated.Value,
								   Status = saga.OrderStatus,
								});
						})
					.TransitionTo(Completed));
		}

		protected OrderStatus OrderStatus { get; set; }

		protected DateTime? OrderCreated { get; set; }

		public OrderDetailsRequestSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		protected OrderDetailsRequestSaga()
		{
		}

		public virtual string OrderId { get; set; }
		public virtual string CustomerId { get; set; }

		public static State Initial { get; set; }
		public static State WaitingForResponse { get; set; }
		public static State Completed { get; set; }

		public static Event<RetrieveOrderDetails> RequestReceived { get; set; }
		public static Event<OrderDetailsResponse> ResponseReceived { get; set; }
		public static Event<OrderDetailsRequestFailed> RequestFailed { get; set; }

		public virtual Guid CorrelationId { get; set; }
		public virtual IServiceBus Bus { get; set; }
	}
}