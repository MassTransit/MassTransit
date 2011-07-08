// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.RabbitMq
{
	using System;
	using Context;

	public class LoopbackRabbitMqTransport :
		IDuplexTransport
	{
		readonly IRabbitMqEndpointAddress _address;
		readonly IInboundTransport _inbound;
		readonly IOutboundTransport _outbound;
		bool _disposed;

		public LoopbackRabbitMqTransport(IRabbitMqEndpointAddress address, IInboundTransport inbound,
		                                 IOutboundTransport outbound)
		{
			_inbound = inbound;
			_outbound = outbound;
			_address = address;
		}

		public IEndpointAddress Address
		{
			get { return _address; }
		}

		public void Receive(Func<IReceiveContext, Action<IReceiveContext>> callback, TimeSpan timeout)
		{
			_inbound.Receive(callback, timeout);
		}

		public void Send(ISendContext context)
		{
			_outbound.Send(context);
		}

		public IOutboundTransport OutboundTransport
		{
			get { return _outbound; }
		}

		public IInboundTransport InboundTransport
		{
			get { return _inbound; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_inbound.Dispose();
				_outbound.Dispose();
			}

			_disposed = true;
		}

		~LoopbackRabbitMqTransport()
		{
			Dispose(false);
		}
	}
}