using System;
using MassTransit;
using MassTransit.Transports.RabbitMq;
using MassTransit.Transports.RabbitMq.Configuration.Configurators;

namespace ClientC
{
	class Program
	{
		static void Main(string[] args)
		{
			var queueName = "ClientC." + Guid.NewGuid();
			Console.WriteLine("G'day mate! The bloody queue name to kick off the bus is: {0}", queueName);

			try
			{
				Bus.Initialize(
					sbc =>
					{
						sbc.UseRabbitMq();
						sbc.ReceiveFrom(string.Format("rabbitmq://localhost/{0}", queueName));
						sbc.Subscribe(subs => subs.Handler<Common.ClientMessage>(msg => Console.WriteLine(msg.Text)));

					});

				Console.WriteLine("Aight mate! We're now BALLS DEEP in the bus.");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				Console.WriteLine("Arrrrgh! You fkd up.");
			}

			Console.Read();

			Bus.Shutdown();
		}
	}
}
