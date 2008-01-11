using System;
using MassTransit.ServiceBus.Exceptions;

namespace MassTransit.ServiceBus
{
	/// <summary>
	/// A MessageQueueEndpoint is an implementation of an endpoint using the Microsoft Message Queue service.
	/// </summary>
	public class MessageQueueEndpoint :
		IMessageQueueEndpoint
	{
		private readonly string _queuePath;
		private readonly Uri _uri;

		//private MessageQueue _queue;

		/// <summary>
		/// Initializes a <c ref="MessageQueueEndpoint /> instance with the specified URI string.
		/// </summary>
		/// <param name="uriString">The URI for the endpoint</param>
		public MessageQueueEndpoint(string uriString)
			: this(new Uri(uriString))
		{
			string hostName = _uri.Host;
			if (string.Compare(hostName, ".") == 0 || string.Compare(hostName, "localhost", true) == 0)
			{
				hostName = Environment.MachineName;
			}

			_queuePath = string.Format(@"{0}\private$\{1}", hostName, _uri.AbsolutePath.Substring(1));
		}

		/// <summary>
		/// Initializes a <c ref="MessageQueueEndpoint /> instance with the specified URI.
		/// </summary>
		/// <param name="uri">The URI for the endpoint</param>
		public MessageQueueEndpoint(Uri uri)
		{
			_uri = uri;

			if (_uri.AbsolutePath.IndexOf("/", 1) >= 0)
			{
				throw new EndpointException(this, "Queue Endpoints can't have a child folder. Good: msmq://machinename/queue_name | Bad: msmq://machinename/queue_name/bad_form");
			}
            string hostName = _uri.Host;
            if (string.Compare(hostName, ".") == 0 || string.Compare(hostName, "localhost", true) == 0)
            {
                hostName = Environment.MachineName;
            }
            _queuePath = string.Format(@"{0}\private$\{1}", hostName, _uri.AbsolutePath.Substring(1));
		}

		#region IMessageQueueEndpoint Members

		public string QueueName
		{
			get { return _queuePath; }
		}

		public Uri Uri
		{
			get { return _uri; }
		}

		public void Dispose()
		{
		}

		#endregion

		//private MessageQueueTransactionType GetTransactionType()
		//{
		//    MessageQueueTransactionType tt = MessageQueueTransactionType.None;
		//    if (_queue.Transactional)
		//    {
		//        Check.RequireTransaction(
		//            string.Format(
		//                "The current queue {0} is transactional and this MessageQueueEndpoint is not running in a transaction.",
		//                _uri));

		//        tt = MessageQueueTransactionType.Automatic;
		//    }
		//    return tt;
		//}

		/// <summary>
		/// Implicitly creates a <c ref="MessageQueueEndpoint" />.
		/// </summary>
		/// <param name="queueUri">A string identifying the URI of the message queue (ex. msmq://localhost/my_queue)</param>
		/// <returns>An instance of the MessageQueueEndpoint class</returns>
		public static implicit operator MessageQueueEndpoint(string queueUri)
		{
			return new MessageQueueEndpoint(queueUri);
		}

		/// <summary>
		/// Returns the URI string for the message queue endpoint.
		/// </summary>
		/// <param name="endpoint">The endpoint to use to generate the URI string</param>
		/// <returns>A URI string that identifies the message queue endpoint</returns>
		public static implicit operator string(MessageQueueEndpoint endpoint)
		{
			return endpoint.Uri.AbsoluteUri;
		}

		/// <summary>
		/// Creates an instance of the <c ref="MessageQueueEndpoint" /> class using the specified queue path
		/// </summary>
		/// <param name="path">A path to a Microsoft Message Queue</param>
		/// <returns>An instance of the <c ref="MessageQueueEndpoint" /> class for the specified queue</returns>
		public static IMessageQueueEndpoint FromQueuePath(string path)
		{
			const string prefix = "FORMATNAME:DIRECT=OS:";

			if (path.Length > prefix.Length && path.Substring(0, prefix.Length).ToUpperInvariant() == prefix)
				path = path.Substring(prefix.Length);

			string[] parts = path.Split('\\');

			if (parts.Length != 3)
				throw new ArgumentException("Invalid Queue Path Specified");

			if (string.Compare(parts[1], "private$", true) != 0)
				throw new ArgumentException("Invalid Queue Path Specified");

			return new MessageQueueEndpoint(string.Format("msmq://{0}/{1}", parts[0], parts[2]));
		}
	}
}