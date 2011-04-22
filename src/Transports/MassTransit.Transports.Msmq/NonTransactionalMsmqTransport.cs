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

	public class NonTransactionalMsmqTransport :
		ILoopbackTransport
	{
		private readonly IInboundTransport _inbound;
		private readonly IOutboundTransport _outbound;

		public NonTransactionalMsmqTransport(IMsmqEndpointAddress address)
		{
			_inbound = new NonTransactionalInboundMsmqTransport(address);
			_outbound = new NonTransactionalOutboundMsmqTransport(address);
		}

		public void Dispose()
		{
			_inbound.Dispose();
			_outbound.Dispose();
		}

		public IEndpointAddress Address
		{
			get { return _inbound.Address; }
		}

		public void Send(Action<ISendContext> callback)
		{
			_outbound.Send(callback);
		}

		public void Receive(Func<IReceiveContext, Action<IReceiveContext>> callback, TimeSpan timeout)
		{
			_inbound.Receive(callback, timeout);
		}

		public IOutboundTransport OutboundTransport
		{
			get { return _outbound; }
		}

		public IInboundTransport InboundTransport
		{
			get { return _inbound; }
		}
	}
}