namespace BusDriver.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Magnum.CommandLineParser;
    using MassTransit.Transports.Msmq;

    public class CountEndpointCommand
    {
        public CountEndpointCommand(IEnumerable<ICommandLineElement> commandLineElements)
        {
            Uri uri = commandLineElements
                .Where(x => typeof (IDefinitionElement).IsAssignableFrom(x.GetType()))
                .Select(x => x as IDefinitionElement)
                .Where(x => x.Key == "uri")
                .Select(x => new Uri(x.Value))
                .Single();

            var management = MsmqEndpointManagement.New(uri);

            long count = management.Count();

            Console.WriteLine(string.Format("{0} message{1} in {2}", count, count != 1 ? "s" : "", uri));
        }
    }
}