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
			String read;
			while (!String.IsNullOrEmpty(read = Console.ReadLine()))
			{
				Bus.Instance.Publish(new ClientMessage { Text = read });
			}

			Console.ReadKey();
		}
	}
}
