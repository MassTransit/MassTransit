// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Transports
{
	using System;
	using System.IO;
	using log4net;
	using Serialization;

	public class LoopbackMessageSelector :
		IMessageSelector
	{
		private static readonly ILog _messageLog = LogManager.GetLogger("MassTransit.Messages");

		private readonly byte[] _data;
		private readonly LoopbackEndpoint _endpoint;
		private readonly IMessageSerializer _serializer;
		private bool _accepted;
		private object _message;

		public LoopbackMessageSelector(LoopbackEndpoint endpoint, byte[] data, IMessageSerializer serializer)
		{
			_endpoint = endpoint;
			_data = data;
			_serializer = serializer;
		}

		public void Dispose()
		{
			if (_accepted) return;

			if (_messageLog.IsInfoEnabled)
				_messageLog.InfoFormat("SKIP:{0}:{1}", _endpoint.Uri, _message != null ? _message.GetType().Name : "(Unknown)");
		}

		public bool AcceptMessage()
		{
			if (_messageLog.IsInfoEnabled)
				_messageLog.InfoFormat("RECV:{0}:{1}", _endpoint.Uri, _message != null ? _message.GetType().Name : "(Unknown)");

			_accepted = true;

			// we were able to get the message without conflict, because we don't support peeking with loopback
			return true;
		}

		public void MoveMessageTo(IEndpoint endpoint)
		{
			throw new NotImplementedException();
		}

		public object DeserializeMessage()
		{
			using (MemoryStream mstream = new MemoryStream(_data))
			{
				_message = _serializer.Deserialize(mstream);

				return _message;
			}
		}
	}
}