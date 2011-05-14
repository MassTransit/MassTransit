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
namespace MassTransit.Util
{
	using System;
	using Context;
	using Pipeline;

    //REVIEW: move into core and mark internal?
	public class NullServiceBus :
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

		public void Publish<T>(T message, Action<IPublishContext<T>> contextCallback) where T : class
		{
			throw new NotImplementedException();
		}

		public TService GetService<TService>() where TService : IBusService
		{
			throw new NotImplementedException();
		}

		IOutboundMessagePipeline IServiceBus.OutboundPipeline
		{
			get { throw new NotImplementedException(); }
		}

		IInboundMessagePipeline IServiceBus.InboundPipeline
		{
			get { throw new NotImplementedException(); }
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

		public TContext ReceiveContext<TContext>(Action<TContext> contextAction) where TContext : IReceiveContext
		{
			throw new NotImplementedException();
		}

		public TContext SendContext<TContext, TMessage>(TMessage message, Action<TContext> contextAction)
			where TContext : ISendContext<TMessage> where TMessage : class
		{
			throw new NotImplementedException();
		}

		public void Publish<T>(T message) where T : class
		{
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