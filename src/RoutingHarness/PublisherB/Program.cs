using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			String read;
			while (!String.IsNullOrEmpty(read = Console.ReadLine()))
			{
				Bus.Instance.Publish(new ClientMessage { Text = read });
			}

			Console.ReadKey();
		}
	}
}
