namespace BusDriver.Commands
{
    using System;
    using System.Collections.Generic;
    using log4net;
    using Magnum.CommandLineParser;
    using MassTransit;
    using MassTransit.Transports.Msmq;

    public class MoveCommand
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (ReceiveCommand));

        public MoveCommand(IEnumerable<ICommandLineElement> commandLineElements)
        {
            Uri from = commandLineElements.GetDefinition("from", x => new Uri(x));
            Uri to = commandLineElements.GetDefinition("to", x => new Uri(x));


            ITransport fromQueue = TransportCache.GetTransport(from);
			ITransport toQueue = TransportCache.GetTransport(to);

            fromQueue.Receive(message => msg =>
                                             {
                                                 var data = new byte[message.Length];
                                                 message.Read(data, 0, data.Length);

                                                 toQueue.Send(x => x.Write(data, 0, data.Length));
                                             });
        }
    }
}