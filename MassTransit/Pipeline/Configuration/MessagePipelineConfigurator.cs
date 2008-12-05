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
	using Interceptors;
	using Sinks;

	public class MessagePipelineConfigurator :
		MessagePipelineConfiguratorBase,
		IConfigurePipeline
	{
		private readonly Func<bool> _emptyToken = () => false;
		private readonly MessagePipeline _pipeline;

		public MessagePipelineConfigurator(MessagePipeline pipeline)
		{
			_pipeline = pipeline;

			// interceptors are inserted at the front of the list, so do them from least to most specific
			_interceptors.Register(new ConsumesAllInterceptor());
			_interceptors.Register(new ConsumesSelectedInterceptor());
			_interceptors.Register(new ConsumesForInterceptor());
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

		public V Configure<V>(Func<IConfigurePipeline, V> action)
		{
			V result = action(this);

			return result;
		}

		private Func<bool> Subscribe(Func<IInterceptorContext, IPipelineInterceptor, IEnumerable<Func<bool>>> subscriber)
		{
			var context = new InterceptorContext(_pipeline, _pipeline.Builder);

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

		public static MessagePipelineConfigurator CreateDefault(IObjectBuilder builder)
		{
			MessageRouter<object> router = new MessageRouter<object>();

			MessagePipeline pipeline = new MessagePipeline(router, builder);

			return new MessagePipelineConfigurator(pipeline);
		}

		public static MessagePipelineConfigurator For(MessagePipeline pipeline)
		{
			return new MessagePipelineConfigurator(pipeline);
		}
	}
}