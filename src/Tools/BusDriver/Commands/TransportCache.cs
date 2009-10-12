namespace BusDriver.Commands
{
	using System;
	using System.Collections.Generic;
	using MassTransit;
	using MassTransit.Transports.Msmq;

	public class TransportCache
	{
		private readonly Dictionary<Uri, ITransport> _transports = new Dictionary<Uri, ITransport>();
		private static readonly TransportCache _instance;

		static TransportCache()
		{
			_instance = new TransportCache();
		}

		private TransportCache()
		{
		}

		public static ITransport GetTransport(Uri uri)
		{
			ITransport transport;
			if (_instance._transports.TryGetValue(uri, out transport))
				return transport;

			transport = MsmqTransportFactory.New(From.Uri(uri));
			_instance._transports.Add(uri, transport);

			return transport;
		}

		public static void Clear()
		{
			_instance._transports.Values.Each(x => x.Dispose());
			_instance._transports.Clear();
		}
	}
}