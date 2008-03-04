namespace MassTransit.ServiceBus.NMS
{
	using System;
	using System.IO;
	using Apache.NMS;
	using Formatters;

	public class NmsFormattedBody :
		IFormattedBody
	{
		private readonly ISession _session;

		private IBytesMessage _bytesMessage;
		private Apache.NMS.IMessage _message;
		private MemoryStream _stream;


		public NmsFormattedBody(ISession sess)
		{
			_session = sess;
		}

		public Apache.NMS.IMessage Message
		{
			get
			{
				if (_stream != null)
				{
					byte[] buffer = new byte[_stream.Length];
					_stream.Position = 0;
					_stream.Read(buffer, 0, buffer.Length);

					_bytesMessage.Content = buffer;
				}
				return _message;
			}
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
				else if (value is byte[])
				{
					_message = _bytesMessage = _session.CreateBytesMessage((byte[]) value);
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
				if (_stream == null)
				{
					_stream = new MemoryStream();
					_message = _bytesMessage = _session.CreateBytesMessage();
				}

				return _stream;
			}
			set { throw new NotImplementedException(); }
		}

		#endregion
	}
}