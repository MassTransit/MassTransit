using System;
using GatewayService.Interfaces;
using GatewayService.Messages;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;

namespace GatewayService
{
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
			Correlate(RequestReceived).By((saga,message) => saga.CustomerId == message.CustomerId && saga.OrderId == message.OrderId);
			Correlate(ResponseReceived).By((saga,message) => saga.CustomerId == message.CustomerId && saga.OrderId == message.OrderId);

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
								                 		
								                 	});
			       	      	})
			       	.TransitionTo(Completed));
			
		}

		protected OrderStatus OrderStatus { get; set; }

		protected DateTime? OrderCreated { get; set; }

		protected OrderDetailsRequestSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public virtual string OrderId { get; set; }
		public virtual string CustomerId { get; set; }

		public static State Initial { get; set; }
		public static State WaitingForResponse { get; set; }
		public static State Completed { get; set; }

		public static Event<RequestOrderDetails> RequestReceived { get; set; }
		public static Event<OrderDetailsResponse> ResponseReceived { get; set; }
		public static Event<OrderDetailsRequestFailed> RequestFailed { get; set; }

		public virtual Guid CorrelationId { get;  set; }
		public virtual IServiceBus Bus { get; set; }
	}
}