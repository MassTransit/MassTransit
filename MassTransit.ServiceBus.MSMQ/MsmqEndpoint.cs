/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.

namespace MassTransit.ServiceBus.MSMQ
{
	using System;
	using System.Messaging;
	using Exceptions;
	using Internal;

	/// <summary>
	/// A MessageQueueEndpoint is an implementation of an endpoint using the Microsoft Message Queue service.
	/// </summary>
	public class MsmqEndpoint :
		IMsmqEndpoint
	{
		private readonly string _queuePath;
		private readonly Uri _uri;
		private IMessageReceiver _receiver;
		private IMessageSender _sender;

		/// <summary>
		/// Initializes a <c ref="MessageQueueEndpoint" /> instance with the specified URI string.
		/// </summary>
		/// <param name="uriString">The URI for the endpoint</param>
		public MsmqEndpoint(string uriString)
			: this(new Uri(uriString))
		{
		}

		/// <summary>
		/// Initializes a <c ref="MessageQueueEndpoint" /> instance with the specified URI.
		/// </summary>
		/// <param name="uri">The URI for the endpoint</param>
		public MsmqEndpoint(Uri uri)
		{
			_uri = uri;

			if (_uri.AbsolutePath.IndexOf("/", 1) >= 0)
			{
				throw new EndpointException(this, "Queue Endpoints can't have a child folder unless it is 'public'. Good: 'msmq://machinename/queue_name' or 'msmq://machinename/public/queue_name' - Bad: msmq://machinename/queue_name/bad_form");
			}

			string hostName = _uri.Host;
			if (string.Compare(hostName, ".") == 0 || string.Compare(hostName, "localhost", true) == 0)
			{
				hostName = Environment.MachineName.ToLowerInvariant();
			}

			if (string.Compare(_uri.Host, "localhost", true) == 0)
			{
				_uri = new Uri("msmq://" + Environment.MachineName.ToLowerInvariant() + _uri.AbsolutePath);
			}

			_queuePath = string.Format(@"FormatName:DIRECT=OS:{0}\private$\{1}", hostName, _uri.AbsolutePath.Substring(1));
		}

		/// <summary>
		/// Creates an instance of the <c ref="MessageQueueEndpoint" /> class using the specified queue
		/// </summary>
		/// <param name="queue">A Microsoft Message Queue</param>
		public MsmqEndpoint(MessageQueue queue)
		{
			string path = queue.Path;
			const string prefix = "FormatName:DIRECT=OS:";

			if (path.Length > prefix.Length && path.Substring(0, prefix.Length).ToUpperInvariant() == prefix.ToUpperInvariant())
				path = path.Substring(prefix.Length);

			string[] parts = path.Split('\\');

			if (parts.Length != 3)
				throw new ArgumentException("Invalid Queue Path Specified");

			//Validate parts[1] = private$
			if (string.Compare(parts[1], "private$", true) != 0)
				throw new ArgumentException("Invalid Queue Path Specified");

			if (parts[0] == ".")
				parts[0] = Environment.MachineName;

			parts[0] = parts[0].ToLowerInvariant();

			_queuePath = string.Format("{0}{1}\\{2}\\{3}", prefix, parts[0], parts[1], parts[2]);
			_uri = new Uri(string.Format("msmq://{0}/{1}", parts[0], parts[2]));
		}

		#region IMessageQueueEndpoint Members

		/// <summary>
		/// The path of the message queue for the endpoint. Suitable for use with <c ref="MessageQueue" />.Open
		/// to access a message queue.
		/// </summary>
		public string QueuePath
		{
			get { return _queuePath; }
		}

		public MessageQueue Open(QueueAccessMode mode)
		{
			MessageQueue queue = new MessageQueue(QueuePath, mode);

			MessagePropertyFilter mpf = new MessagePropertyFilter();
			mpf.SetAll();

			queue.MessageReadPropertyFilter = mpf;

			return queue;
		}

		/// <summary>
		/// The address of the endpoint, in URI format
		/// </summary>
		public Uri Uri
		{
			get { return _uri; }
		}

		public IMessageSender Sender
		{
			get
			{
				lock (this)
				{
					if (_sender == null)
						_sender = new MsmqMessageSender(this);
				}
				return _sender;
			}
		}

		public IMessageReceiver Receiver
		{
			get
			{
				lock (this)
				{
					if (_receiver == null)
						_receiver = new MsmqMessageReceiver(this);
				}

				return _receiver;
			}
		}

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose()
		{
			if (_receiver != null)
				_receiver.Dispose();

			if (_sender != null)
				_sender.Dispose();
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
		public static implicit operator MsmqEndpoint(string queueUri)
		{
			return new MsmqEndpoint(queueUri);
		}

		/// <summary>
		/// Returns the URI string for the message queue endpoint.
		/// </summary>
		/// <param name="endpoint">The endpoint to use to generate the URI string</param>
		/// <returns>A URI string that identifies the message queue endpoint</returns>
		public static implicit operator string(MsmqEndpoint endpoint)
		{
			return endpoint.Uri.AbsoluteUri;
		}
	}
}