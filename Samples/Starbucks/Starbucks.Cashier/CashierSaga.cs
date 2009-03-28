namespace Starbucks.Cashier
{
	using System;
	using System.Threading;
	using Magnum.StateMachine;
	using MassTransit;
	using MassTransit.Saga;
	using Messages;

	public class CashierSaga :
		SagaStateMachine<CashierSaga>,
		ISaga
	{
		private decimal _amount;
		private string _item;
		private string _name;
		private string _size;

		static CashierSaga()
		{
			Define(() =>
				{
					Initially(
						When(NewOrder)
							.Then((saga, message) => saga.ProcessNewOrder(message))
							.TransitionTo(WaitingForPayment)
						);

					During(WaitingForPayment,
					       When(PaymentSubmitted)
					       	.Then((saga, message) => saga.ProcessPayment(message))
					       	.Complete()
						);
				});
		}

		public CashierSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public static State Initial { get; set; }
		public static State Completed { get; set; }
		public static State WaitingForPayment { get; set; }

		public static Event<NewOrderMessage> NewOrder { get; set; }
		public static Event<SubmitPaymentMessage> PaymentSubmitted { get; set; }

		public Guid CorrelationId { get; set; }

		public IServiceBus Bus { get; set; }

		private void ProcessNewOrder(NewOrderMessage message)
		{
			_name = message.Name;
			_item = message.Item;
			_size = message.Size;
			_amount = GetPriceForSize(_size);

			Console.WriteLine(string.Format("I've received an order for a {0} {1} for {2}.", _size, _item, _name));

			var paymentDueMessage = new PaymentDueMessage
				{
					CorrelationId = message.CorrelationId,
					Amount = _amount,
				};

			CurrentMessage.Respond(paymentDueMessage);
		}

		public void ProcessPayment(SubmitPaymentMessage message)
		{
			if (message.Amount > _amount)
			{
				Console.WriteLine("Thanks for the tip!");
			}
			else if (message.Amount < _amount)
			{
				Console.WriteLine("What are you, some kind of charity case?");
			}

			var paymentType = message.PaymentType;

			Console.WriteLine("Received a payment of {0} for {1} ({2})", paymentType, _item, _size);

			if (paymentType == PaymentType.CreditCard)
			{
				Console.Write("Authorizing Card...");
				Thread.Sleep(4000);
				Console.WriteLine("done!");
			}

			var completeMessage = new PaymentCompleteMessage
				{
					CorrelationId = message.CorrelationId,
				};

			Bus.Publish(completeMessage);
		}


		private static decimal GetPriceForSize(string size)
		{
			switch (size.ToLower())
			{
				case "tall":
					return 3.25m;
				case "grande":
					return 4.00m;
				case "venti":
					return 4.75m;
				default:
					throw new Exception(string.Format("We don't have that size ({0})", size));
			}
		}
	}
}