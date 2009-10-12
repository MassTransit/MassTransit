namespace BusDriver.Commands
{
	using System;
	using System.Collections.Generic;
	using Magnum.CommandLineParser;
	using MassTransit.Transports.Msmq;

	public class CreateEndpointCommand
	{
		public CreateEndpointCommand(IEnumerable<ICommandLineElement> commandLineElements)
		{
			Uri uri = commandLineElements.GetDefinition("uri", x => new Uri(x));

			bool transactional = commandLineElements.GetSwitch('t');


			Console.WriteLine("Creating queue: " + uri + (transactional ? " (transactional)" : ""));

			var management = MsmqEndpointManagement.New(uri);

			management.Create(transactional);
		}
	}
}