namespace MassTransit.ServiceBus.NMS
{
	using System;
	using System.IO;
	using Apache.NMS;
	using Formatters;

	public class NmsMessageBody :
		IFormattedBody
	{
		private readonly ISession _session;
		private IMessage _message;
		private MemoryStream _stream;


		public NmsMessageBody(ISession sess)
		{
			_session = sess;
		}

		#region IFormattedBody Members

		public object Body
		{
			get { throw new NotImplementedException(); }
			set
			{
				if (value is string)
				{
					_message = _session.CreateTextMessage((string) value);
				}
				else if ( value is byte[])
				{
					_message = _session.CreateBytesMessage((byte[]) value);
				}
				else
				{
					throw new NotImplementedException();
				}
			}
		}

		public Stream BodyStream
		{
			get
			{
				if(_stream == null)
				{
					_stream = new MemoryStream();
				}

				return _stream;
			}
			set { throw new NotImplementedException(); }
		}

		#endregion

		public IMessage Message
		{
			get { return _message; }
		}
	}
}