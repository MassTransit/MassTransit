namespace MassTransit.ServiceBus.MSMQ
{
    using System;
    using System.Diagnostics;
    using System.Messaging;

    public class MsmqUtilities
    {
        public int NumberOfMessages(Uri uri)
        {
            int i = 0;
            
            MessageQueue q = new MessageQueue(new MsmqEndpoint(uri).QueuePath);
            using (MessageEnumerator e = q.GetMessageEnumerator2())
            {
                while (e.MoveNext(new TimeSpan(0)))
                {
                    i++;
                }
            }

            return i;
        }

        public void PurgeQueue(Uri uri)
        {
            MessageQueue q = new MessageQueue(new MsmqEndpoint(uri).QueuePath);
            q.Purge();
        }
    }

    public class WindowsUtilities
    {
        public void StopService(string serverName, string serviceName)
        {
            // sc \\servername stop {servername}
            Process p = new Process();
            p.WaitForExit();
            p.StartInfo.FileName = "sc";
            p.StartInfo.Arguments = string.Format("{0} start {1}", serverName, serviceName);
            p.Start();
        }

        public void StartService(string serverName, string serviceName)
        {
            // sc \\servername start {servername}
            Process p = new Process();
            //p.WaitForExit();
            p.StartInfo.FileName = "sc";
            p.StartInfo.Arguments = string.Format("sc {0} start {1}", serverName, serviceName );
            p.Start();
        }

        public void RestartService(string serverName, string serviceName)
        {
            StopService(serverName, serviceName);
            StartService(serverName, serviceName);
        }

        public void Test()
        {
            StartService("\\pooh", "msvsmon80");
        }
    }
}