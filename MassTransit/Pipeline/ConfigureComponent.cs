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
namespace MassTransit.Pipeline
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Handles the configuration of components into the MessagePipeline and provides
	/// an extensible model for adding new configuration types without modifying the 
	/// base support.
	/// </summary>
	public class ConfigureComponent :
		IDisposable
	{
		private readonly MessagePipeline _pipeline;
		private volatile bool _disposed;
		private InterceptorList<ISubscribeInterceptor> _subscribers;

		public ConfigureComponent(MessagePipeline pipeline)
		{
			_pipeline = pipeline;

			_subscribers = new InterceptorList<ISubscribeInterceptor>();

			_subscribers.Register(new ConsumesSelectedPipelineSubscriber());
			_subscribers.Register(new ConsumesAllPipelineSubscriber());
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
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

		private Func<bool> Subscribe(Func<ISubscribeContext, ISubscribeInterceptor, IEnumerable<Func<bool>>> subscriber)
		{
			var context = new ConfigureComponentContext(_pipeline);

			Func<bool> result = null;

			_subscribers.ForEach(interceptor =>
				{
					foreach (Func<bool> token in subscriber(context, interceptor))
					{
						if (result == null)
							result = token;
						else
							result += token;
					}
				});

			return result;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_subscribers.Dispose();
				_subscribers = null;
			}

			_disposed = true;
		}

		~ConfigureComponent()
		{
			Dispose(false);
		}
	}
}