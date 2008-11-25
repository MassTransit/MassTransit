namespace MassTransit.MSMQ
{
    using System;
    using System.Diagnostics;
    using System.Messaging;
    using System.Runtime.Serialization.Formatters.Binary;

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

        public static void VerifyMessageInQueue<T>(string queuePath, T messageCheck)
        {
            using (MessageQueue mq = new MessageQueue(GetQueueName(queuePath), QueueAccessMode.Receive))
            {
                Message msg = mq.Receive(TimeSpan.FromSeconds(3));

                object message = new BinaryFormatter().Deserialize(msg.BodyStream);
            }
        }

        public static void ValidateAndPurgeQueue(string queuePath)
        {
            ValidateAndPurgeQueue(queuePath, false);
        }

        public static void ValidateAndPurgeQueue(string queuePath, bool isTransactional)
        {
            try
            {
                MessageQueue queue = new MessageQueue(GetQueueName(queuePath), QueueAccessMode.ReceiveAndAdmin);
                queue.Purge();
            }
            catch (MessageQueueException ex)
            {
                if (ex.MessageQueueErrorCode == MessageQueueErrorCode.QueueNotFound)
                {
                    MessageQueue.Create(GetQueueName(queuePath), isTransactional);
                }
            }
        }

        public static string GetQueueName(string name)
        {
            string result = name;
            if (result.Contains("FormatName:DIRECT=OS:"))
                result = result.Replace("FormatName:DIRECT=OS:", "");
            if (result.Contains("localhost"))
                result = result.Replace("localhost", ".");
            if (result.Contains(Environment.MachineName.ToLowerInvariant()))
                result = result.Replace(Environment.MachineName.ToLowerInvariant(), ".");

            return result;
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