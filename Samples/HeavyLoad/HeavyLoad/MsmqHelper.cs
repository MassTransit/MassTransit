namespace HeavyLoad
{
	using System;
	using System.Messaging;
	using System.Runtime.Serialization.Formatters.Binary;

	public static class MsmqHelper
	{
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
}