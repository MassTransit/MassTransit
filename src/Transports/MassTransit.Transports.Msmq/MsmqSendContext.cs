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
namespace MassTransit.Transports.Msmq
{
	using System;
	using System.IO;
	using System.Messaging;
	using Magnum;

	public class MsmqSendContext :
		ISendContext
	{
		private Message _message;

		public MsmqSendContext()
		{
			_message = new Message();
		}

		public Message Message
		{
			get { return _message; }
		}

		public Stream Body
		{
			get { return _message.BodyStream; }
		}

		public void MarkRecoverable()
		{
			_message.Recoverable = true;
		}

		public void SetLabel(string label)
		{
			_message.Label = label;
		}

		public void SetMessageExpiration(DateTime d)
		{
			_message.TimeToBeReceived = d.Kind == DateTimeKind.Utc ? d - SystemUtil.UtcNow : d - SystemUtil.Now;
		}

		public void Dispose()
		{
			if (_message != null)
			{
				_message.Dispose();
				_message = null;
			}
		}
	}
}