namespace BusDriver.Commands
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Magnum.CommandLineParser;

	public class CreateCommand
	{
		public CreateCommand(IEnumerable<ICommandLineElement> commandLineElements)
		{
			var target = commandLineElements.FirstOrDefault();

			var type = target as IArgumentElement;
			if (type == null)
			{
				Console.WriteLine("Usage fault: " + target);
				return;
			}

			if (type.Id == "endpoint")
			{
				var command = new CreateEndpointCommand(commandLineElements.Skip(1));
			}
		}
	}
}