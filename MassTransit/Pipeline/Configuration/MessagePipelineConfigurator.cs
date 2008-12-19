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
namespace MassTransit.Pipeline.Configuration
{
	using System;
	using System.Collections.Generic;
	using Batch.Pipeline;
	using Interceptors;
	using Saga.Pipeline;
	using Sinks;
	using Subscriptions;

	public class MessagePipelineConfigurator :
		IConfigurePipeline,
		IDisposable
	{
		private readonly IObjectBuilder _builder;
		private readonly Func<bool> _emptyToken = () => false;
		private volatile bool _disposed;

		protected InterceptorList<IPipelineInterceptor> _interceptors = new InterceptorList<IPipelineInterceptor>();
		private MessagePipeline _pipeline;
		private ISubscriptionEvent _subscriptionEvent;

		public MessagePipelineConfigurator(IObjectBuilder builder, ISubscriptionEvent subscriptionEvent)
		{
			_builder = builder;
			_subscriptionEvent = subscriptionEvent;

			MessageRouter<object> router = new MessageRouter<object>();

			_pipeline = new MessagePipeline(router, this);

			// interceptors are inserted at the front of the list, so do them from least to most specific
			_interceptors.Register(new ConsumesAllInterceptor());
			_interceptors.Register(new ConsumesSelectedInterceptor());
			_interceptors.Register(new ConsumesForInterceptor());
			_interceptors.Register(new BatchInterceptor());
			_interceptors.Register(new OrchestratesInterceptor());
			_interceptors.Register(new InitiatesInterceptor());
		}

		public Func<bool> Register(IPipelineInterceptor interceptor)
		{
			return _interceptors.Register(interceptor);
		}

		public Func<bool> Subscribe<TComponent>()
			where TComponent : class
		{
			return Subscribe((context, interceptor) => interceptor.Subscribe<TComponent>(context));
		}

		public Func<bool> Subscribe<TComponent>(TComponent instance)
			where TComponent : class
		{
			return Subscribe((context, interceptor) => interceptor.Subscribe(context, instance));
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing || _disposed) return;

			if (_interceptors != null)
				_interceptors.Dispose();

			_pipeline = null;
			_interceptors = null;

			_disposed = true;
		}

		~MessagePipelineConfigurator()
		{
			Dispose(false);
		}

		public V Configure<V>(Func<IConfigurePipeline, V> action)
		{
			V result = action(this);

			return result;
		}

		private Func<bool> Subscribe(Func<IInterceptorContext, IPipelineInterceptor, IEnumerable<Func<bool>>> subscriber)
		{
			var context = new InterceptorContext(_pipeline, _builder, _subscriptionEvent);

			Func<bool> result = null;

			_interceptors.ForEach(interceptor =>
				{
					foreach (Func<bool> token in subscriber(context, interceptor))
					{
						if (result == null)
							result = token;
						else
							result += token;
					}
				});

			return result ?? _emptyToken;
		}

		public static implicit operator MessagePipeline(MessagePipelineConfigurator configurator)
		{
			return configurator._pipeline;
		}

		public static MessagePipelineConfigurator CreateDefault(IObjectBuilder builder, ISubscriptionEvent subscriptionEvent)
		{
			return new MessagePipelineConfigurator(builder, subscriptionEvent);
		}
	}
}