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
namespace MassTransit.Transports.Loopback
{
	using System;
	using System.IO;

	public class LoopbackSendContext :
		ISendContext
	{
		private LoopbackMessage _message;

		public LoopbackSendContext()
		{
			_message = new LoopbackMessage();
		}

		public Stream Body
		{
			get { return _message.Body; }
		}

		public void MarkRecoverable()
		{
		}

		public void SetLabel(string label)
		{
		}

		public void SetMessageExpiration(DateTime d)
		{
		}


		public void Dispose()
		{
			if (_message != null)
			{
				_message.Body.Dispose();
				_message = null;
			}
		}

		public LoopbackMessage GetMessage()
		{
			_message.Body.Seek(0, SeekOrigin.Begin);

			LoopbackMessage message = _message;

			_message = null;

			return message;
		}
	}
}