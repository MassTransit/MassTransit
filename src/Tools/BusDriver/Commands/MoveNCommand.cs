using System;
using System.Collections.Generic;
using log4net;
using Magnum.CommandLineParser;
using MassTransit;

namespace BusDriver.Commands
{
    public class MoveNCommand
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (ReceiveCommand));

        public MoveNCommand(IEnumerable<ICommandLineElement> commandLineElements)
        {
            Uri from = commandLineElements.GetDefinition("from", x => new Uri(x));
            Uri to = commandLineElements.GetDefinition("to", x => new Uri(x));

            int countOfElementsToMove = commandLineElements.GetDefinition("count", x => Convert.ToInt32(x));

            ITransport fromQueue = TransportCache.GetTransport(from);
            ITransport toQueue = TransportCache.GetTransport(to);

            IList<byte[]> rawMessageBodies = new List<byte[]>();

            _log.InfoFormat("Attempting to move {0} messages from {1} to {2}.", countOfElementsToMove, from, to);

            for(int i=1; i <= countOfElementsToMove; i++)
            {
                bool foundMessage = false;
                int receiveAttempt = i;
                fromQueue.Receive(message => msg =>
                {  
                    var data = new byte[message.Length];
                    message.Read(data, 0, data.Length);
                    rawMessageBodies.Add(data);
                    foundMessage = true;
                    _log.DebugFormat("Receive Attempt {0}: Received message of length {1}.", receiveAttempt, message.Length);
                });

                if (!foundMessage)
                {
                    _log.InfoFormat("After {0} receives. No more messages were found.", i);
                    break;
                }
            }

            _log.InfoFormat("Sending {0} messages to {1}.", rawMessageBodies.Count, to);

            foreach (var messageBody in rawMessageBodies)
            {
                var localMessageBody = messageBody;
                toQueue.Send(x => x.Write(localMessageBody, 0, localMessageBody.Length));
            }
        }
    }
}
