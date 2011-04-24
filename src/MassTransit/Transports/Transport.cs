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
namespace MassTransit.Transports
{
	using System;

	public class Transport :
		IDuplexTransport
	{
		private bool _disposed;
		private IInboundTransport _inbound;
		private IOutboundTransport _outbound;

		public Transport(IInboundTransport inbound, IOutboundTransport outbound)
		{
			_inbound = inbound;
			_outbound = outbound;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public IEndpointAddress Address
		{
			get { return _inbound.Address; }
		}

		public void Send(Action<ISendContext> callback)
		{
			GuardAgainstDisposed();

			_outbound.Send(callback);
		}

		public void Receive(Func<IReceiveContext, Action<IReceiveContext>> callback, TimeSpan timeout)
		{
			GuardAgainstDisposed();

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

		private void GuardAgainstDisposed()
		{
			if (_disposed)
				throw new ObjectDisposedException("The transport has already been disposed: " + Address);
		}

		private void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_inbound.Dispose();
				_inbound = null;

				_outbound.Dispose();
				_outbound = null;
			}

			_disposed = true;
		}

		~Transport()
		{
			Dispose(false);
		}
	}
}