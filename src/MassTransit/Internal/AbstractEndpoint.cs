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
	using Serialization;

	public abstract class AbstractEndpoint :
		IEndpoint
	{
		private readonly IEndpointAddress _address;
		private bool _disposed;
		private string _disposedMessage;

		protected AbstractEndpoint(IEndpointAddress address, IMessageSerializer serializer)
		{
			_address = address;
			Serializer = serializer;
		}

		protected IMessageSerializer Serializer { get; set; }

		public IEndpointAddress Address
		{
			get { return _address; }
		}

		public Uri Uri
		{
			get { return Address.Uri; }
		}

		public abstract void Send<T>(T message) where T : class;
		public abstract void Receive(Func<object, Action<object>> receiver);
		public abstract void Receive(Func<object, Action<object>> receiver, TimeSpan timeout);

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void SetDisposedMessage()
		{
			_disposedMessage = "The endpoint has already been disposed: " + _address;
		}

		protected ObjectDisposedException NewDisposedException()
		{
			return new ObjectDisposedException(_disposedMessage);
		}

		protected void SetOutboundMessageHeaders<T>()
		{
			OutboundMessage.Set(headers =>
				{
					headers.SetMessageType(typeof (T));
					headers.SetDestinationAddress(Uri);
				});
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				Serializer = null;
			}

			_disposed = true;
		}

		~AbstractEndpoint()
		{
			Dispose(false);
		}
	}
}