using System.Security.Policy;

namespace BusDriver.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using log4net;
    using Magnum.CommandLineParser;
    using MassTransit.Transports.Msmq;

    public class ReceiveCommand
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (ReceiveCommand));

        public ReceiveCommand(IEnumerable<ICommandLineElement> commandLineElements)
        {
            Uri uri = commandLineElements.GetDefinition("uri", x => new Uri(x));

            var transport = MsmqTransportFactory.New(From.Uri(uri));

            transport.Receive(message => msg =>
                                             {
                                                 var data = new byte[message.Length];
                                                 message.Read(data, 0, data.Length);

                                                 string text = Encoding.UTF8.GetString(data);

                                                 Console.WriteLine("Message: " + text);
                                             });
        }
    }

    public static class Extens
    {
        public static string GetDefinition(this IEnumerable<ICommandLineElement> elements, string key)
        {
            return elements
                .Where(x => typeof (IDefinitionElement).IsAssignableFrom(x.GetType()))
                .Select(x => x as IDefinitionElement)
                .Where(x => x.Key == key)
                .Select(x => x.Value)
                .Single();
        }

        public static T GetDefinition<T>(this IEnumerable<ICommandLineElement> elements, string key,
                                         Func<string, T> converter)
        {
            return elements
                .Where(x => typeof (IDefinitionElement).IsAssignableFrom(x.GetType()))
                .Select(x => x as IDefinitionElement)
                .Where(x => x.Key == key)
                .Select(x => converter(x.Value))
                .Single();
        }
    }
}