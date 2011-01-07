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
	using System.IO;
	using System.Text;
	using Apache.NMS;

	public class NmsReceiveContext :
		IReceiveContext
	{
		private Stream _body;
		private readonly ITextMessage _message;

		public NmsReceiveContext(ITextMessage message)
		{
			_message = message;
			_body = new MemoryStream(Encoding.UTF8.GetBytes(message.Text), false);
		}

		public string MessageId
		{
			get { return _message.NMSMessageId; }
		}

		public Stream Body
		{
			get { return _body; }
		}

		public void Dispose()
		{
			if (_body != null)
			{
				_body.Dispose();
				_body = null;
			}
		}
	}
}