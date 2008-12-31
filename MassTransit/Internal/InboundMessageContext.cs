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
namespace MassTransit.Internal
{
	using System;

	public class InboundMessageContext :
		IInboundMessageContext
	{
		private ServiceBus _bus;
		private Type _messageType;
		private IEndpoint _replyTo;

		public Type MessageType
		{
			get { return _messageType; }
		}

		public IEndpoint ReplyTo
		{
			get { return _replyTo; }
		}

		public IServiceBus Bus
		{
			get { return _bus; }
		}

		public void Clear()
		{
			_bus = null;
			_messageType = null;
			_replyTo = null;
		}

		public void SetReplyTo(Uri uri)
		{
			_replyTo = _bus.EndpointFactory.GetEndpoint(uri);
		}

		public void Initialize(ServiceBus bus)
		{
			Clear();

			_bus = bus;
		}
	}
}