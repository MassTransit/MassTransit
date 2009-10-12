namespace BusDriver.Commands
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using log4net;
	using Magnum.CommandLineParser;

	public class ReceiveCommand
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (ReceiveCommand));

		public ReceiveCommand(IEnumerable<ICommandLineElement> commandLineElements)
		{
			Uri uri = commandLineElements.GetDefinition("uri", x => new Uri(x));

			var transport = TransportCache.GetTransport(uri);

			transport.Receive(message => msg =>
				{
					var data = new byte[message.Length];
					message.Read(data, 0, data.Length);

					string text = Encoding.UTF8.GetString(data);

					Console.WriteLine("Message: " + text);
				});
		}
	}
}