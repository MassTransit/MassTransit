namespace BusDriver.Commands
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using log4net;
	using Magnum.CommandLineParser;
	using MassTransit;

	public class SendTextCommand
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (SendTextCommand));

		public SendTextCommand(IEnumerable<ICommandLineElement> commandLineElements)
		{
			Uri uri = commandLineElements.GetDefinition("uri", x => new Uri(x));

			string message = commandLineElements.GetDefinition("m");

			_log.Info("Sending message " + message + " to " + uri);

			ITransport transport = TransportCache.GetTransport(uri);
			transport.Send(s =>
				{
					var bytes = Encoding.UTF8.GetBytes(message);

					s.Write(bytes, 0, bytes.Length);
				});
		}
	}
}