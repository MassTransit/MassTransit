namespace OpenAllNight
{
	using System;
	using System.IO;
	using System.Threading;
	using Castle.Core;
	using Castle.Windsor;
	using log4net.Config;
	using MassTransit;
	using MassTransit.Services.Subscriptions.Messages;
	using MassTransit.WindsorIntegration;

	internal class Program
	{
		private static readonly DateTime _startedAt = DateTime.Now;
		private static Func<bool> _unsubscribeToken = () => false;
		private static DateTime lastPrint = DateTime.Now;

		private static void Main()
		{
			XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.xml"));

			WindsorContainer c = new DefaultMassTransitContainer("castle.xml");
			c.AddComponentLifeStyle("counter", typeof (Counter), LifestyleType.Singleton);
			c.AddComponentLifeStyle("rvaoeuaoe", typeof (CacheUpdateResponseHandler), LifestyleType.Transient);

			IServiceBus bus = c.Resolve<IServiceBus>();
			bus.Subscribe<CacheUpdateResponseHandler>();

			SimpleMessageHandler handler = new SimpleMessageHandler();


			IEndpoint ep = c.Resolve<IEndpointFactory>().GetEndpoint(new Uri("msmq://localhost/mt_pubsub"));
			Counter counter = c.Resolve<Counter>();
			Console.WriteLine("Please enter the number of hours you would like this test to run for?");
			string input = Console.ReadLine();
			double hours = double.Parse(input);
			DateTime stopTime = DateTime.Now.AddHours(hours);

			Console.WriteLine("Test Started");

			Random rand = new Random();

			while (DateTime.Now < stopTime)
			{
				ep.Send(new CacheUpdateRequest(new Uri("msmq://localhost/test_servicebus")));
				counter.IncrementMessagesSent();

				if (rand.Next(0, 10) == 0)
				{
					_unsubscribeToken();
					_unsubscribeToken = bus.Subscribe(handler);
					counter.Subscribed = true;
				}
				else if (rand.Next(0, 10) == 0)
				{
					_unsubscribeToken();
					_unsubscribeToken = () => false;
					counter.Subscribed = false;
				}

				if (rand.Next(0, 10) < 4)
				{
					bus.Publish(new SimpleMessage());
					counter.IncrementPublishCount();
				}

				Thread.Sleep(rand.Next(1, 20));
				PrintTime(bus, counter);
			}

			Console.WriteLine("Messages Sent: {0}", counter.MessagesSent);
			Console.WriteLine("Messages Received: {0}", counter.MessagesReceived);
			Console.WriteLine("Done");
			Console.ReadLine();
		}

		private static void PrintTime(IServiceBus bus, Counter counter)
		{
			TimeSpan ts = DateTime.Now.Subtract(lastPrint);
			if (ts.Minutes >= 1)
			{
				Console.WriteLine("Elapsed Time: {0} mins, So far I have - Sent: {1}, Received: {2}, Published: {3}, Received: {4}",
				                  (int) ((DateTime.Now - _startedAt).TotalMinutes), counter.MessagesSent, counter.MessagesReceived, counter.PublishCount, SimpleMessageHandler.MessageCount);

				lastPrint = DateTime.Now;
			}
		}
	}


	public class SimpleMessageHandler :
		Consumes<SimpleMessage>.All
	{
		private static long _messageCount;

		public static long MessageCount
		{
			get { return _messageCount; }
		}

		public void Consume(SimpleMessage message)
		{
			Interlocked.Increment(ref _messageCount);
		}
	}

	[Serializable]
	public class SimpleMessage
	{
		private readonly string _data = new string('*', 1024);
	}
}