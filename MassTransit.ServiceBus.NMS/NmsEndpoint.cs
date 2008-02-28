using System;
using MassTransit.ServiceBus.Internal;

namespace MassTransit.ServiceBus.NMS
{
	public class NmsEndpoint :
		INmsEndpoint
	{
		private IMessageReceiver _receiver;
		private IMessageSender _sender;
		private Uri _uri;

		public NmsEndpoint(Uri uri)
		{
			_uri = uri;
		}

		public NmsEndpoint(string uriString)
		{
			_uri = new Uri(uriString);
		}

		#region INmsEndpoint Members

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
						_sender = new NmsMessageSender(this);
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
						_receiver = new NmsMessageReceiver(this);
				}

				return _receiver;
			}
		}

		public void Dispose()
		{
			if (_receiver != null)
				_receiver.Dispose();

			if (_sender != null)
				_sender.Dispose();
		}

		#endregion
	}
}