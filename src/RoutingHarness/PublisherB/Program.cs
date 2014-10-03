using System;
using System.Threading;

namespace PublisherB
{
	using Common;
	using MassTransit;

	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("This is the Publisher B");
			Bus.Initialize(sbc =>
			{
				sbc.UseRabbitMq();
				sbc.ReceiveFrom("rabbitmq://localhost/publisher_b");
			});

			var thread = new Thread(new ThreadStart(
				delegate
				{
					while (true)
					{
						Bus.Instance.Publish(new ClientMessage { Text = "ClientB." + Guid.NewGuid() });
					}
				}));

			thread.Start();

			Console.ReadKey();

			thread.Abort();

			Bus.Shutdown();
		}
	}
}
