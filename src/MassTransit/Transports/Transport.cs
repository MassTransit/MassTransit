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
namespace MassTransit.Transports
{
	using System;

	public class Transport :
		IDuplexTransport
	{
		readonly Func<IInboundTransport> _inboundFactory;
		readonly object _lock = new object();
		readonly Func<IOutboundTransport> _outboundFactory;
		bool _disposed;
		IInboundTransport _inbound;
		IOutboundTransport _outbound;
		readonly IEndpointAddress _address;

		public Transport(IEndpointAddress address, Func<IInboundTransport> inboundFactory, Func<IOutboundTransport> outboundFactory)
		{
			_inboundFactory = inboundFactory;
			_outboundFactory = outboundFactory;
			_address = address;
		}

		public void Dispose()
		{
			Dispose(true);
		}

		public IEndpointAddress Address
		{
			get { return _address; }
		}

		public void Send(ISendContext context)
		{
			if (_disposed)
				throw new ObjectDisposedException("The transport has already been disposed: " + Address);

			OutboundTransport.Send(context);
		}

		public void Receive(Func<IReceiveContext, Action<IReceiveContext>> callback, TimeSpan timeout)
		{
			if (_disposed)
				throw new ObjectDisposedException("The transport has already been disposed: " + Address);

			InboundTransport.Receive(callback, timeout);
		}

		public IOutboundTransport OutboundTransport
		{
			get
			{
				lock (_lock)
				{
					return _outbound ?? (_outbound = _outboundFactory());
				}
			}
		}

		public IInboundTransport InboundTransport
		{
			get
			{
				lock (_lock)
				{
					return _inbound ?? (_inbound = _inboundFactory());
				}
			}
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				if (_inbound != null)
				{
					_inbound.Dispose();
					_inbound = null;
				}

				if (_outbound != null)
				{
					_outbound.Dispose();
					_outbound = null;
				}
			}

			_disposed = true;
		}
	}
}