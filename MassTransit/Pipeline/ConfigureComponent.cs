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

	public class ConfigureComponent<TComponent>
	{
		private static List<ISubscribeInterceptor> _subscribers;

		static ConfigureComponent()
		{
			_subscribers = new List<ISubscribeInterceptor>();

			_subscribers.Add(new ConsumesAllPipelineSubscriber());
		}

		public static Func<bool> Subscribe(MessagePipeline pipeline)
		{
			var context = new ConfigureComponentContext(pipeline);

			Func<bool> result = null;

			foreach (ISubscribeInterceptor interceptor in _subscribers)
			{
				foreach (Func<bool> token in interceptor.Subscribe<TComponent>(context))
				{
					if (result == null)
						result = token;
					else
						result += token;
				}
			}

			return result;
		}
	}

	public class ConfigureComponentContext :
		ISubscribeContext
	{
		private readonly MessagePipeline _pipeline;
		private readonly HashSet<Type> _used = new HashSet<Type>();

		public ConfigureComponentContext(MessagePipeline pipeline)
		{
			_pipeline = pipeline;
		}

		public bool HasMessageTypeBeenDefined(Type messageType)
		{
			return _used.Contains(messageType);
		}

		public Func<bool> Connect<TMessage>(IMessageSink<TMessage> sink) where TMessage : class
		{
			return _pipeline.Configure(x => ConfigureMessageRouter<TMessage>.Connect(x, sink));
		}

		public void MessageTypeWasDefined(Type messageType)
		{
			_used.Add(messageType);
		}

		public IObjectBuilder Builder
		{
			get { return _pipeline.Builder; }
		}
	}
}