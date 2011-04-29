// Copyright 2007-2011 The Apache Software Foundation.
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
namespace MassTransit.Transports.Nms
{
	using System;
	using System.IO;
	using System.Text;
	using Apache.NMS;
	using Magnum;
	using MessageHeaders;

	public class NmsSendContext :
		ISendContext
	{
		private readonly ITextMessage _message;
		private readonly IMessageProducer _producer;
		private MemoryStream _body;

		public NmsSendContext(IMessageProducer producer)
		{
			_producer = producer;
			_message = _producer.CreateTextMessage();
			_body = new MemoryStream();
		}

		public ITextMessage Message
		{
			get { return _message; }
		}

		public Stream Body
		{
			get { return _body; }
		}

		public void MarkRecoverable()
		{
			_producer.DeliveryMode = MsgDeliveryMode.Persistent;
			_message.NMSDeliveryMode = MsgDeliveryMode.Persistent;
		}

		public void SetLabel(string label)
		{
			_message.Properties["label"] = label;
		}

		public void SetMessageExpiration(DateTime d)
		{
			if (OutboundMessage.Headers.ExpirationTime.HasValue)
			{
				_message.NMSTimeToLive = d.Kind == DateTimeKind.Utc ? d - SystemUtil.UtcNow : d - SystemUtil.Now;
			}
		}

		public void Send()
		{
			_message.Text = Encoding.UTF8.GetString(_body.ToArray());
			_producer.Send(_message);
		}

		public void Dispose()
		{
			if(_body != null)
			{
				_body.Dispose();
				_body = null;
			}
		}
	}
}