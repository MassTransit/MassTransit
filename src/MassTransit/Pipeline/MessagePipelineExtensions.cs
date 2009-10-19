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
	using Configuration;
	using Configuration.Subscribers;
	using Sinks;

	public static class MessagePipelineExtensions
	{
		/// <summary>
		/// Dispatch a message through the pipeline
		/// </summary>
		/// <param name="pipeline">The pipeline instance</param>
		/// <param name="message">The message to dispatch</param>
		public static bool Dispatch(this IMessagePipeline pipeline, object message)
		{
			return pipeline.Dispatch(message, x => true);
		}

		/// <summary>
		/// Dispatch a message through the pipeline. If the message will be consumed, the accept function
		/// is called to allow the endpoint to acknowledge the message reception if applicable
		/// </summary>
		/// <param name="pipeline">The pipeline instance</param>
		/// <param name="message">The message to dispatch</param>
		/// <param name="acknowledge">The function to call if the message will be consumed by the pipeline</param>
		public static bool Dispatch(this IMessagePipeline pipeline, object message, Func<object, bool> acknowledge)
		{
			bool consumed = false;

			foreach (Action<object> consumer in pipeline.Enumerate(message))
			{
				if (!acknowledge(message))
					return false;

				acknowledge = x => true;

				consumed = true;

				consumer(message);

			}

			return consumed;
		}

		/// <summary>
		/// Subscribe a component type to the pipeline that is resolved from the container for each message
		/// </summary>
		/// <typeparam name="TComponent"></typeparam>
		/// <param name="pipeline">The pipeline to configure</param>
		/// <returns></returns>
		public static UnsubscribeAction Subscribe<TComponent>(this IMessagePipeline pipeline) where TComponent : class
		{
			return pipeline.Configure(x => x.Subscribe<TComponent>());
		}

		/// <summary>
		/// Subscribe a component to the pipeline that handles every message
		/// </summary>
		/// <typeparam name="TComponent"></typeparam>
		/// <param name="pipeline">The pipeline to configure</param>
		/// <param name="instance">The instance that will handle the messages</param>
		/// <returns></returns>
		public static UnsubscribeAction Subscribe<TComponent>(this IMessagePipeline pipeline, TComponent instance)
			where TComponent : class
		{
			return pipeline.Configure(x => x.Subscribe(instance));
		}

		public static UnsubscribeAction Subscribe<TMessage>(this IMessagePipeline pipeline, Action<TMessage> handler, Predicate<TMessage> acceptor) 
			where TMessage : class
		{
			return pipeline.Configure(x => x.Subscribe(handler, acceptor));
		}

		public static UnsubscribeAction Subscribe<TMessage>(this IMessagePipeline pipeline, Func<TMessage, Action<TMessage>> getHandler) 
			where TMessage : class
		{
			return pipeline.Configure(x => x.Subscribe<TMessage>(getHandler));
		}

		public static UnsubscribeAction Subscribe<TMessage>(this IMessagePipeline pipeline, IEndpoint endpoint) where TMessage : class
		{
			MessageRouterConfigurator routerConfigurator = MessageRouterConfigurator.For(pipeline);

			return routerConfigurator.FindOrCreate<TMessage>().Connect(new EndpointMessageSink<TMessage>(endpoint));
		}

		public static UnsubscribeAction Subscribe<TMessage,TKey>(this IMessagePipeline pipeline, TKey correlationId, IEndpoint endpoint) 
			where TMessage : class, CorrelatedBy<TKey>
		{
			var correlatedConfigurator = CorrelatedMessageRouterConfigurator.For(pipeline);

			var router = correlatedConfigurator.FindOrCreate<TMessage, TKey>();

			UnsubscribeAction result = router.Connect(correlationId, new EndpointMessageSink<TMessage>(endpoint));

			return result;
		}

		public static UnregisterAction Filter<TMessage>(this IMessagePipeline pipeline, Func<TMessage, bool> allow)
			where TMessage : class
		{
			return Filter(pipeline, "", allow);
		}

		public static UnregisterAction Filter<TMessage>(this IMessagePipeline pipeline, string description, Func<TMessage, bool> allow)
			where TMessage : class
		{
			MessageFilterConfigurator configurator = MessageFilterConfigurator.For(pipeline);

			var filter = configurator.Create(description, allow);

			UnregisterAction result = () => { throw new NotSupportedException("Removal of filters not yet supported"); };

			return result;
		}

		public static UnregisterAction Register(this IPipelineConfigurator context, IPipelineSubscriber subscriber)
		{
			return context.Register(subscriber);
		}
	}
}