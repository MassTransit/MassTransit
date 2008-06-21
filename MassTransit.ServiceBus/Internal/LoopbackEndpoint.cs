namespace MassTransit.ServiceBus.Internal
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Threading;
	using log4net;

	public class LoopbackEndpoint : IEndpoint
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (LoopbackEndpoint));
		private static readonly ILog _messageLog = LogManager.GetLogger("MassTransit.Messages");

		private readonly BinaryFormatter _formatter = new BinaryFormatter();
		private readonly Semaphore _messageReady = new Semaphore(0, int.MaxValue);
		private readonly Queue<byte[]> _messages = new Queue<byte[]>();
		private readonly Uri _uri;

		public LoopbackEndpoint(Uri uri)
		{
			_uri = uri;
		}

		public static string Scheme
		{
			get { return "loopback"; }
		}

		public Uri Uri
		{
			get { return _uri; }
		}

		public void Send<T>(T message) where T : class
		{
			Enqueue(message);
		}

		public void Send<T>(T message, TimeSpan timeToLive) where T : class
		{
			Enqueue(message);
		}

		public object Receive()
		{
			return Receive(TimeSpan.MaxValue);
		}

		public object Receive(TimeSpan timeout)
		{
			if (_messageReady.WaitOne(timeout, true))
			{
				object obj =  Dequeue();

				if (_messageLog.IsInfoEnabled)
					_messageLog.InfoFormat("RECV:{0}:{1}", _uri, obj.GetType().Name);

				return obj;
			}

			return null;
		}

		public object Receive(Predicate<object> accept)
		{
			return Receive(TimeSpan.MaxValue, accept);
		}

		public object Receive(TimeSpan timeout, Predicate<object> accept)
		{
			if (_messageReady.WaitOne(timeout, true))
			{
				object obj = Dequeue();

				if (accept(obj))
				{
					if (_messageLog.IsInfoEnabled)
						_messageLog.InfoFormat("RECV:{0}:{1}", _uri, obj.GetType().Name);

					return obj;
				}
			}

			return null;
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
			_messages.Clear();
		}

		private void Enqueue<T>(T message)
		{
			using (MemoryStream mstream = new MemoryStream())
			{
				_formatter.Serialize(mstream, message);
				_messages.Enqueue(mstream.ToArray());
			}

			_messageReady.Release();
		}

		private object Dequeue()
		{
			byte[] buffer = _messages.Dequeue();

			using (MemoryStream mstream = new MemoryStream(buffer))
			{
				object obj = _formatter.Deserialize(mstream);

				return obj;
			}
		}
	}
}