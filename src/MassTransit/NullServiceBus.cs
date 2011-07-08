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
namespace MassTransit
{
	using System;
	using Exceptions;
	using Pipeline;

	class NullServiceBus :
		IServiceBus
	{
		bool _disposed;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public IEndpoint Endpoint
		{
			get { return Transports.Endpoint.Null; }
		}

		public UnsubscribeAction Configure(Func<IInboundPipelineConfigurator, UnsubscribeAction> configure)
		{
			return () => true;
		}

		public void Publish<T>(T message, Action<IPublishContext<T>> contextCallback) 
			where T : class
		{
			throw new NotImplementedByDesignException();
		}

		public TService GetService<TService>() where TService : IBusService
		{
			throw new NotImplementedByDesignException();
		}

		IOutboundMessagePipeline IServiceBus.OutboundPipeline
		{
			get { throw new NotImplementedByDesignException(); }
		}

		IInboundMessagePipeline IServiceBus.InboundPipeline
		{
			get { throw new NotImplementedByDesignException(); }
		}

		public IServiceBus ControlBus
		{
			get { return this; }
		}

		public IEndpointCache EndpointCache
		{
			get { return null; }
		}

		public IEndpoint GetEndpoint(Uri address)
		{
			return Transports.Endpoint.Null;
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
			}

			_disposed = true;
		}

		~NullServiceBus()
		{
			Dispose(false);
		}
	}
}