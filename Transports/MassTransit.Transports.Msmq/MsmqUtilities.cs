// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Transports.Msmq
{
    using System;
    using System.Diagnostics;
    using System.Messaging;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Transactions;
    using Serialization;

    public class MsmqUtilities
    {
        public int NumberOfMessages(Uri uri)
        {
            int i = 0;

			MessageQueue q = new MessageQueue(new MsmqEndpoint(uri, new BinaryMessageSerializer()).QueuePath);
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
			MessageQueue q = new MessageQueue(new MsmqEndpoint(uri, new BinaryMessageSerializer()).QueuePath);
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