namespace BusDriver.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Magnum.CommandLineParser;
    using MassTransit.Transports.Msmq;

    public class CreateEndpointCommand
    {
        public CreateEndpointCommand(IEnumerable<ICommandLineElement> commandLineElements)
        {
            Uri uri = commandLineElements
                .Where(x => typeof (IDefinitionElement).IsAssignableFrom(x.GetType()))
                .Select(x => x as IDefinitionElement)
                .Where(x => x.Key == "uri")
                .Select(x => new Uri(x.Value))
                .Single();

            bool transactional = commandLineElements
                .Where(x => typeof (ISwitchElement).IsAssignableFrom(x.GetType()))
                .Select(x => x as ISwitchElement)
                .Where(x => x.Key == 't')
                .Select(x => true)
                .SingleOrDefault();

            Console.WriteLine("Creating queue: " + uri + (transactional ? " (transactional)" : ""));

            var management = MsmqEndpointManagement.New(uri);

            management.Create(transactional);
        }
    }
}