namespace MassTransit.ServiceBus.Util
{
    using System;
    using System.Messaging;

    public static class MsmqUtilities
    {
        public static string NormalizeQueueName(MessageQueue queue)
        {
            string machineName = queue.MachineName;
            if (machineName == "." || string.Compare(machineName, "localhost", true) == 0)
            {
                queue.MachineName = Environment.MachineName;
            }

            return queue.Path;
        }
    }
}