using System;

namespace ClientB
{
	using MassTransit;

	class Program
	{
		static void Main(string[] args)
		{
			var queueName = Guid.NewGuid();
			Console.WriteLine("G'day mate! The bloody queue name to kick off the bus is: {0}", queueName);

			try
			{
				Bus.Initialize(sbc =>
				{
					sbc.UseRabbitMq();
					sbc.ReceiveFrom(string.Format("rabbitmq://localhost/{0}", queueName));
					sbc.Subscribe(subs => subs.Handler<Common.ClientMessage>(msg => Console.WriteLine(msg.Text)));
				});

				Console.WriteLine("Aight mate! We're now BALLS DEEP in the bus.");
			}
			catch (Exception ex)
			{
				Console.WriteLine("Arrrrgh! You fkd up.");
			}

			Console.Read();
		}
	}
}
