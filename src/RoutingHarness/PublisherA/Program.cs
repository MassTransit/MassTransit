using System.Threading;

namespace PublisherA
{
	using System;
	using Common;
	using MassTransit;

	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("This is the Publisher A");
			Bus.Initialize(sbc =>
			{
				sbc.UseRabbitMq();
				sbc.ReceiveFrom("rabbitmq://localhost/publisher_a");
			});

			var thread = new Thread(new ThreadStart(
				delegate
				{
					while (true)
					{
						Bus.Instance.Publish(new ClientMessage {Text = "ClientA." + Guid.NewGuid()});
					}
				}));

			thread.Start();
			
			Console.ReadKey();

			thread.Abort();

			Bus.Shutdown();
		}
	}
}
