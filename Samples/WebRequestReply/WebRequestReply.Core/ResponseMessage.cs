using System;
using MassTransit.ServiceBus;

namespace WebRequestReply.Core
{
	[Serializable]
	public class ResponseMessage :
		IMessage
	{
		private string _text;

		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}
	}
}