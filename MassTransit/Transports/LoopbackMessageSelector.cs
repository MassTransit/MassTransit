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
	using Internal;
	using log4net;
	using Magnum.Reflection;
	using Serialization;

	public class LoopbackMessageSelector :
		IMessageSelector
	{
		private static readonly ILog _messageLog = SpecialLoggers.Messages;

		private readonly LoopbackEndpoint _endpoint;
		private readonly Action _acceptor;
		private readonly IMessageSerializer _serializer;
		private byte[] _data;
		private object _message;

		public LoopbackMessageSelector(LoopbackEndpoint endpoint, byte[] data, Action acceptor, IMessageSerializer serializer)
		{
			_endpoint = endpoint;
			_data = data;
			_acceptor = acceptor;
			_serializer = serializer;
		}

		public bool AcceptedMessage { get; private set; }

		public void Dispose()
		{
			if (AcceptedMessage) return;

			if (_messageLog.IsInfoEnabled)
				_messageLog.InfoFormat("SKIP:{0}:{1}", _endpoint.Uri, _message != null ? _message.GetType().Name : "(Unknown)");
		}

		public bool AcceptMessage()
		{
			if (AcceptedMessage)
				return true;

			if (_messageLog.IsInfoEnabled)
				_messageLog.InfoFormat("RECV:{0}:{1}", _endpoint.Uri, _message != null ? _message.GetType().Name : "(Unknown)");

			try
			{
				_acceptor();
				AcceptedMessage = true;
				return true;
			}
			catch (Exception ex)
			{
				_messageLog.WarnFormat("FAIL:{0}:{1} - {2}", _endpoint.Uri, _message != null ? _message.GetType().Name : "(Unknown)", ex.Message);
				return false;
			}
		}

		public void MoveMessageTo(IEndpoint endpoint)
		{
			endpoint.Call("Send", _message);
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