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
	using Sinks;

	public static class MessagePipelineExtensions
	{
		/// <summary>
		/// Dispatch a message through the pipeline
		/// </summary>
		/// <param name="pipeline">The pipeline instance</param>
		/// <param name="message">The message to dispatch</param>
		public static void Dispatch(this MessagePipeline pipeline, object message)
		{
			pipeline.Dispatch(message, x => true);
		}

		/// <summary>
		/// Dispatch a message through the pipeline. If the message will be consumed, the accept function
		/// is called to allow the endpoint to acknowledge the message reception if applicable
		/// </summary>
		/// <param name="pipeline">The pipeline instance</param>
		/// <param name="message">The message to dispatch</param>
		/// <param name="acknowledge">The function to call if the message will be consumed by the pipeline</param>
		public static void Dispatch(this MessagePipeline pipeline, object message, Func<object, bool> acknowledge)
		{
			foreach (Consumes<object>.All consumer in pipeline.Enumerate(message))
			{
				if (!acknowledge(message))
					break;

				acknowledge = x => true;

				consumer.Consume(message);
			}
		}

		/// <summary>
		/// Subscribe a component type to the pipeline that is resolved from the container for each message
		/// </summary>
		/// <typeparam name="TComponent"></typeparam>
		/// <param name="pipeline">The pipeline to configure</param>
		/// <returns></returns>
		public static Func<bool> Subscribe<TComponent>(this MessagePipeline pipeline) where TComponent : class
		{
			return MessagePipelineConfigurator.For(pipeline).Configure(x => x.Subscribe<TComponent>());
		}

		/// <summary>
		/// Subscribe a component to the pipeline that handles every message
		/// </summary>
		/// <typeparam name="TComponent"></typeparam>
		/// <param name="pipeline">The pipeline to configure</param>
		/// <param name="instance">The instance that will handle the messages</param>
		/// <returns></returns>
		public static Func<bool> Subscribe<TComponent>(this MessagePipeline pipeline, TComponent instance)
			where TComponent : class
		{
			return MessagePipelineConfigurator.For(pipeline).Configure(x => x.Subscribe(instance));
		}

		public static Func<bool> Subscribe<TMessage>(this MessagePipeline pipeline, IEndpoint endpoint) where TMessage : class
		{
			MessageRouterConfigurator routerConfigurator = MessageRouterConfigurator.For(pipeline);

			return routerConfigurator.FindOrCreate<TMessage>().Connect(new EndpointMessageSink<TMessage>(endpoint));
		}

		public static Func<bool> Filter<TMessage>(this MessagePipeline pipeline, Func<TMessage, bool> allow)
			where TMessage : class
		{
			return Filter(pipeline, "", allow);
		}

		public static Func<bool> Filter<TMessage>(this MessagePipeline pipeline, string description, Func<TMessage, bool> allow)
			where TMessage : class
		{
			MessageFilterConfigurator configurator = MessageFilterConfigurator.For(pipeline);

			var filter = configurator.Create(description, allow);

			Func<bool> result = () => { throw new NotSupportedException("Removal of filters not yet supported"); };

			return result;
		}
	}
}