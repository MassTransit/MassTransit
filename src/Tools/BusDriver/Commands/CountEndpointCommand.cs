namespace BusDriver.Commands
{
	using System;
	using System.Collections.Generic;
	using Magnum.CommandLineParser;
	using MassTransit.Transports.Msmq;

	public class CountEndpointCommand
	{
		public CountEndpointCommand(IEnumerable<ICommandLineElement> commandLineElements)
		{
			Uri uri = commandLineElements.GetDefinition("uri", x => new Uri(x));

			var management = MsmqEndpointManagement.New(uri);

			long count = management.Count();

			Console.WriteLine(string.Format("{0} message{1} in {2}", count, count != 1 ? "s" : "", uri));
		}
	}
}