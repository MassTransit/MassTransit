namespace Starbucks.Barista
{
	using System;
	using System.Threading;
	using Magnum.StateMachine;
	using MassTransit;
	using MassTransit.Saga;
	using Messages;

	public class DrinkPreparationSaga :
		SagaStateMachine<DrinkPreparationSaga>,
		ISaga
	{
		static DrinkPreparationSaga()
		{
			Define(() =>
				{
					Initially(
						When(NewOrder)
							.Then((saga, message) => saga.ProcessNewOrder(message))
							.TransitionTo(WaitingForPayment)
						);

					During(WaitingForPayment,
					       When(PaymentComplete)
					       	.Then((saga,message) =>
					       		{
					       			Console.WriteLine("Payment Complete for '{0}' got it!", saga.Name);
					       			saga.ServeDrink();
					       		})
					       	.Complete()
						);
				});
		}

		public DrinkPreparationSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}


		public string Drink { get; set; }
		public string Name { get; set; }

		public static State Initial { get; set; }
		public static State Completed { get; set; }
		public static State PreparingDrink { get; set; }
		public static State WaitingForPayment { get; set; }


		public static Event<NewOrderMessage> NewOrder { get; set; }
		public static Event<PaymentCompleteMessage> PaymentComplete { get; set; }

		public Guid CorrelationId { get; private set; }

		public IServiceBus Bus { get; set; }


		public void ProcessNewOrder(NewOrderMessage message)
		{
			Name = message.Name;
			Drink = string.Format("{0} {1}", message.Size, message.Item);

			Console.WriteLine(string.Format("{0} for {1}, got it!", Drink, Name));

			for (int i = 0; i < 10; i++)
			{
				Thread.Sleep(i*200);
				Console.WriteLine("[wwhhrrrr....psssss...chrhrhrhrrr]");
			}
		}

		private void ServeDrink()
		{
			Console.WriteLine(string.Format("I've got a {0} ready for {1}!", Drink, Name));

			var message = new DrinkReadyMessage
				{
					CorrelationId = CorrelationId,
					Drink = Drink,
					Name = Name,
				};

			Bus.Publish(message);
		}
	}
}