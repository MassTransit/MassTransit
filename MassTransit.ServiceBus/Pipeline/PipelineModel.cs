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
namespace MassTransit.ServiceBus.Pipeline
{
	using System;
	using System.Collections.Generic;

	public class PipelineModel :
		IPipelineModel
	{
		private readonly Func<bool> _emptyToken = () => false;

		private volatile bool _disposed;

		private InterceptorList<IPublishInterceptor> _publishInterceptorList = new InterceptorList<IPublishInterceptor>();
		private InterceptorList<ISubscribeInterceptor> _subscribeInterceptorList = new InterceptorList<ISubscribeInterceptor>();

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public Func<bool> RegisterSubscribeInterceptor(ISubscribeInterceptor interceptor)
		{
			return _subscribeInterceptorList.Register(interceptor);
		}

		public Func<bool> RegisterPublishInterceptor(IPublishInterceptor interceptor)
		{
			return _publishInterceptorList.Register(interceptor);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing || _disposed) return;

			if (_subscribeInterceptorList != null)
				_subscribeInterceptorList.Dispose();

			_subscribeInterceptorList = null;

			if (_publishInterceptorList != null)
				_publishInterceptorList.Dispose();

			_publishInterceptorList = null;
			_disposed = true;
		}

		~PipelineModel()
		{
			Dispose(false);
		}

		public Func<bool> Subscribe<TComponent>(ISubscribeContext context, TComponent instance)
		{
			Func<bool> result = null;

			_subscribeInterceptorList.ForEach(interceptor =>
				{
					foreach (Func<bool> token in interceptor.Subscribe(context, instance))
						result += token;
				});

			return result ?? _emptyToken;
		}

		public IEnumerable<IEndpoint> Publish<TMessage>(TMessage message) where TMessage : class
		{
			HashSet<IEndpoint> endpoints = new HashSet<IEndpoint>();

			_publishInterceptorList.ForEach(interceptor =>
				{
					foreach (IEndpoint endpoint in interceptor.Publish(message))
					{
						if (!endpoints.Contains(endpoint))
							endpoints.Add(endpoint);
					}
				});

			return endpoints;
		}
	}
}