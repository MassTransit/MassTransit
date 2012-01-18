namespace MassTransit.EventStoreIntegration.Tests
{
	using System;
	using System.Threading;
	using Magnum.Extensions;
	using Magnum.StateMachine;
	using Saga;
	using Services.Timeout.Messages;

	public class Cashier :
		SagaStateMachine<Cashier>,
		ISagaEventSourced
	{
		static Cashier()
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
							.Complete(),
						When(ItTookTooLong)
							.Then((saga, message) => saga.CallInTheGuards())
							.Complete()
						);
				});
		}

		readonly DeltaManager<Cashier> _eventHandler;

		public Cashier(Guid correlationId)
		{
			_eventHandler = new DeltaManager<Cashier>(this, correlationId, null);
		}

		[Serializable]
		internal class RememberOrder
		{
			public string Name { get; set; }
			public string Item { get; set; }
			public string Size { get; set; }
			public decimal Amount { get; set; }
		}

		void ProcessNewOrder(NewOrderMessage message)
		{
			var delta = _eventHandler.RaiseStateDelta(new RememberOrder
				{
					Name = message.Name,
					Item = message.Item,
					Size = message.Size,
					Amount = GetPriceForSize(message.Size)
				});

			Console.WriteLine(string.Format("I've received an order for a {0} {1} for {2}.", 
				delta.Size,
				delta.Item,
				delta.Name));

			Bus.Context().Respond(new PaymentDueMessage
				{
					CorrelationId = message.CorrelationId,
					Amount = delta.Amount
				});

			Bus.Publish(new ScheduleTimeout(CorrelationId, 1.Minutes().FromNow()));
		}

		RememberOrder _p;
		void Apply(RememberOrder payment)
		{
			_p = payment;
		}

		void ProcessPayment(SubmitPayment message)
		{
			if (message.Amount > _p.Amount)
				Console.WriteLine("Thanks for the tip!");
			else if (message.Amount < _p.Amount)
				Console.WriteLine("What are you, some kind of charity case?");

			var paymentType = message.PaymentType;

			Console.WriteLine("Received a payment of {0} for {1} ({2}) for {3}", paymentType, _p.Item, _p.Size, _p.Name);

			if (paymentType == PaymentType.CreditCard)
			{
				Console.Write("Authorizing Card...");
				Thread.Sleep(4000);
				Console.WriteLine("done!");
			}

			var completeMessage = new PaymentCompleteMessage
				{
					CorrelationId = message.CorrelationId
				};

			Bus.Publish(completeMessage);
		}

		void CallInTheGuards()
		{
			Bus.Publish(new GuardsGetThisDudeOutOfTheStore());
		}

		internal static decimal GetPriceForSize(string size)
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

		public static State Initial { get; set; }
		public static State Completed { get; set; }
		public static State WaitingForPayment { get; set; }

		public static Event<TimeoutExpired> ItTookTooLong { get; set; }
		public static Event<NewOrderMessage> NewOrder { get; set; }
		public static Event<SubmitPayment> PaymentSubmitted { get; set; }

		public Guid CorrelationId { get { return _eventHandler.CorrelationId; } }
		public IServiceBus Bus { get; set; }

		ISagaDeltaManager ISagaEventSourced.DeltaManager
		{
			get { return _eventHandler; }
		}
	}
}