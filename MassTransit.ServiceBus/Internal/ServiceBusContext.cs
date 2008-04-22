namespace MassTransit.ServiceBus.Internal
{
	using System;

	public static class ServiceBusContext
	{
		[ThreadStatic]
		private static IServiceBus _bus;
		[ThreadStatic]
		private static IEnvelope _envelope;
		[ThreadStatic]
		private static IMessage _message;

		public static IServiceBus Bus
		{
			get { return _bus; }
			set { _bus = value; }
		}

		public static IEnvelope Envelope
		{
			get { return _envelope; }
			set { _envelope = value; }
		}

		public static IMessage Message
		{
			get { return _message; }
			set { _message = value; }
		}
	}
}