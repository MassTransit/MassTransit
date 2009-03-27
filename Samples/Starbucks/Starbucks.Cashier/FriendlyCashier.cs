namespace Starbucks.Cashier
{
	using System;
	using System.Threading;
	using MassTransit;
	using Messages;
	using Microsoft.Practices.ServiceLocation;

	public class FriendlyCashier :
		Consumes<NewOrderMessage>.All,
		Consumes<SubmitPaymentMessage>.All
	{
		private IServiceBus _bus;

		private IServiceBus Bus
		{
			get
			{
				if (_bus == null)
					_bus = ServiceLocator.Current.GetInstance<IServiceBus>();
				return _bus;
			}
		}

		public void Consume(NewOrderMessage message)
		{
			Console.WriteLine(string.Format("I've received an order for a {0} {1} for {2}.",
			                                message.Size, message.Item, message.Name));

			decimal price = GetPriceForSize(message.Size);
			Bus.Publish(new PaymentDueMessage(message.CorrelationId, message.Name, price));
		}

		public void Consume(SubmitPaymentMessage message)
		{
			var paymentType = message.PaymentType;
			Console.WriteLine("Received a payment of " + paymentType);
			if (paymentType == PaymentType.CreditCard)
			{
				Console.Write("Authorizing Card...");
				Thread.Sleep(4000);
				Console.WriteLine("done!");
			}

			Bus.Publish(new PaymentCompleteMessage(message.CorrelationId, message.Name, message.Amount));
		}

		private decimal GetPriceForSize(string size)
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