namespace MassTransit.ServiceBus.SQS
{
	using System;
	using System.IO;
	using System.Net;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Threading;
	using Amazon.SQS;
	using Amazon.SQS.Model;
	using Exceptions;
	using log4net;

	public class AmazonEndpoint :
		IEndpoint
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (AmazonEndpoint));
		private static readonly ILog _messageLog = LogManager.GetLogger("MassTransit.Messages");

		private readonly BinaryFormatter _formatter = new BinaryFormatter();
		private readonly Uri _uri;
		private AmazonSQS _service;
		private string _queueName;

		public AmazonEndpoint(Uri uri)
		{
			_uri = uri;
			_queueName = Uri.AbsolutePath.Substring(1);

			string userInfo = _uri.UserInfo;

			string[] segments = userInfo.Split(':');

			string accessKeyId = segments[0];
			string secretAccessKey = segments[1];

			_service = new AmazonSQSClient(accessKeyId, secretAccessKey);
		}

		public static string Scheme
		{
			get { return "amazon"; }
		}

		public Uri Uri
		{
			get { return _uri; }
		}

		public void Send<T>(T message) where T : class
		{
			Send(message, TimeSpan.MaxValue);
		}

		public void Send<T>(T message, TimeSpan timeToLive) where T : class
		{
			try
			{
				string body;
				using(MemoryStream mstream = new MemoryStream())
				{
					_formatter.Serialize(mstream, message);

					body = Convert.ToBase64String(mstream.ToArray());
				}

				SendMessage request = new SendMessage()
					.WithQueueName(_queueName)
					.WithMessageBody(body);

				SendMessageResponse response = _service.SendMessage(request);

				_messageLog.DebugFormat("SENT: {0} - {1}", _queueName, response.SendMessageResult.MessageId);
				
			}
			catch (Exception ex)
			{
				throw new EndpointException(this, "Error sending message to " + _queueName, ex);
			}
		}

		public object Receive()
		{
			return Receive(TimeSpan.MaxValue);
		}

		public object Receive(TimeSpan timeout)
		{
			return Receive(timeout, delegate { return true; });
		}

		public object Receive(Predicate<object> accept)
		{
			return Receive(TimeSpan.FromSeconds(1), accept);
		}

		public object Receive(TimeSpan timeout, Predicate<object> accept)
		{
			try
			{
				ReceiveMessage request = new ReceiveMessage()
					.WithQueueName(_queueName)
					.WithVisibilityTimeout((int) timeout.TotalSeconds)
					.WithMaxNumberOfMessages(1m);

				ReceiveMessageResponse response = _service.ReceiveMessage(request);

				foreach (Message message in response.ReceiveMessageResult.Message)
				{
					byte[] body = Convert.FromBase64String(message.Body);
					using(MemoryStream mstream = new MemoryStream(body, false))
					{
						object msg = _formatter.Deserialize(mstream);

						if(accept(msg))
						{
							DeleteMessage deleteRequest = new DeleteMessage()
								.WithQueueName(_queueName)
								.WithReceiptHandle(message.ReceiptHandle);

							DeleteMessageResponse deleteResponse = _service.DeleteMessage(deleteRequest);

							_messageLog.DebugFormat("RECV: {0} - {1}", _queueName, message.MessageId);

							return msg;
						}
					}
				}

				return null;
			}
			catch (Exception ex)
			{
				throw new EndpointException(this, "Error receiving message from " + _queueName, ex);
			}
		}

		public T Receive<T>() where T : class
		{
			return Receive<T>(TimeSpan.MaxValue);
		}

		public T Receive<T>(TimeSpan timeout) where T : class
		{
			try
			{
				return (T) Receive(timeout, delegate(object obj)
				                            	{
				                            		Type messageType = obj.GetType();

				                            		if (messageType != typeof (T))
				                            			return false;

				                            		return true;
				                            	});
			}
			catch (Exception ex)
			{
				string message = string.Format("Error on receive with Receive<{0}> accept", typeof (T).Name);
				_log.Error(message, ex);
			}

			throw new Exception("Receive<T>(TimeSpan timeout) didn't error");
		}

		public T Receive<T>(Predicate<T> accept) where T : class
		{
			return Receive<T>(TimeSpan.MaxValue, accept);
		}

		public T Receive<T>(TimeSpan timeout, Predicate<T> accept) where T : class
		{
			try
			{
				return (T) Receive(timeout, delegate(object obj)
				                            	{
				                            		Type messageType = obj.GetType();

				                            		if (messageType != typeof (T))
				                            			return false;

				                            		T message = obj as T;
				                            		if (message == null)
				                            			return false;

				                            		return accept(message);
				                            	});
			}
			catch (Exception ex)
			{
				string message = string.Format("Error on receive with Predicate<{0}> accept", typeof (T).Name);
				_log.Error(message, ex);
			}

			throw new Exception("Receive<T>(TimeSpan timeout, Predicate<T> accept) had a weird error");
		}

		public void Dispose()
		{
			_service = null;
		}
	}
}