namespace MassTransit.ServiceBus.Tests
{
	using System;

	public static class MessageRouterExtensions
	{
		public static Func<bool> SubscribeTo<TMessage>(this Consumes<TMessage>.Selected consumer, MessageRouter<TMessage> router)
			where TMessage : class
		{
			return router.Add(new MessageSink<TMessage>(message => consumer.Accept(message) ? consumer : Consumes<TMessage>.Null));
		}

		public static Func<bool> SubscribeTo<TMessage>(this Consumes<TMessage>.All consumer, MessageRouter<TMessage> router)
			where TMessage : class
		{
			return router.Add(new MessageSink<TMessage>(message => consumer));
		}
	}
}