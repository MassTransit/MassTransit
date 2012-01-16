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
namespace MassTransit.Pipeline
{
	using System;
	using System.IO;
	using Configuration;
	using Context;
	using MassTransit.Configuration;
	using Sinks;
	using SubscriptionConnectors;

	public static class MessagePipelineExtensions
	{
		/// <summary>
		/// Dispatch a message through the pipeline
		/// </summary>
		/// <param name="pipeline">The pipeline instance</param>
		/// <param name="message">The message to dispatch</param>
		public static bool Dispatch<T>(this IInboundMessagePipeline pipeline, T message)
			where T : class
		{
			return pipeline.Dispatch(message, x => true);
		}

		public static bool Dispatch<T>(this IOutboundMessagePipeline pipeline, T message)
			where T : class
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
		public static bool Dispatch<T>(this IInboundMessagePipeline pipeline, T message, Func<T, bool> acknowledge)
			where T : class
		{
			bool consumed = false;

			using (var bodyStream = new MemoryStream())
			{
				ReceiveContext receiveContext = ReceiveContext.FromBodyStream(bodyStream);
				pipeline.Configure(x => receiveContext.SetBus(x.Bus));
				var context = new ConsumeContext<T>(receiveContext, message);

				using (context.CreateScope())
				{
					foreach (var consumer in pipeline.Enumerate(context))
					{
						if (!acknowledge(message))
							return false;

						acknowledge = x => true;

						consumed = true;

						consumer(context);
					}
				}
			}

			return consumed;
		}

		public static bool Dispatch<T>(this IOutboundMessagePipeline pipeline, T message, Func<T, bool> acknowledge)
			where T : class
		{
			bool consumed = false;

			var context = new SendContext<T>(message);

			foreach (var consumer in pipeline.Enumerate(context))
			{
				if (!acknowledge(message))
					return false;

				acknowledge = x => true;

				consumed = true;

				consumer(context);
			}

			return consumed;
		}

		/// <summary>
		/// Subscribe a component type to the pipeline that is resolved from the container for each message
		/// </summary>
		/// <typeparam name="TComponent"></typeparam>
		/// <param name="pipeline">The pipeline to configure</param>
		/// <returns></returns>
		public static UnsubscribeAction ConnectConsumer<TComponent>(this IInboundMessagePipeline pipeline)
			where TComponent : class, new()
		{
			return pipeline.Configure(x =>
				{
					var consumerFactory = new DelegateConsumerFactory<TComponent>(() => new TComponent());

					ConsumerConnector connector = ConsumerConnectorCache.GetConsumerConnector<TComponent>();
					return connector.Connect(x, consumerFactory);
				});
		}

		/// <summary>
		/// Subscribe a component type to the pipeline that is resolved from the container for each message
		/// </summary>
		/// <typeparam name="TConsumer"></typeparam>
		/// <param name="pipeline">The pipeline to configure</param>
		/// <param name="consumerFactory"></param>
		/// <returns></returns>
		public static UnsubscribeAction ConnectConsumer<TConsumer>(this IInboundMessagePipeline pipeline,
		                                                           Func<TConsumer> consumerFactory)
			where TConsumer : class
		{
			return pipeline.Configure(x =>
				{
					var factory = new DelegateConsumerFactory<TConsumer>(consumerFactory);

					ConsumerConnector connector = ConsumerConnectorCache.GetConsumerConnector<TConsumer>();
				    return connector.Connect(x, factory);
				});
		}

		/// <summary>
		/// Subscribe a component to the pipeline that handles every message
		/// </summary>
		/// <typeparam name="TComponent"></typeparam>
		/// <param name="pipeline">The pipeline to configure</param>
		/// <param name="instance">The instance that will handle the messages</param>
		/// <returns></returns>
		public static UnsubscribeAction ConnectInstance<TComponent>(this IInboundMessagePipeline pipeline, TComponent instance)
			where TComponent : class
		{
			return pipeline.Configure(x =>
				{
					InstanceConnector connector = InstanceConnectorCache.GetInstanceConnector<TComponent>();
					return connector.Connect(x, instance);
				});
		}

		public static UnsubscribeAction ConnectHandler<TMessage>(this IInboundMessagePipeline pipeline,
		                                                         Action<TMessage> handler,
		                                                         Predicate<TMessage> condition)
			where TMessage : class
		{
			return pipeline.Configure(x =>
				{
					var connector = new HandlerSubscriptionConnector<TMessage>();

					return connector.Connect(x, HandlerSelector.ForSelectiveHandler(condition, handler));
				});
		}

		public static UnsubscribeAction ConnectEndpoint<TMessage>(this IOutboundMessagePipeline pipeline, IEndpoint endpoint)
			where TMessage : class
		{
			var sink = new EndpointMessageSink<TMessage>(endpoint);

			return pipeline.ConnectToRouter(sink);
		}

	    public static UnsubscribeAction ConnectEndpoint<TMessage, TKey>(this IOutboundMessagePipeline pipeline,
		                                                                TKey correlationId,
		                                                                IEndpoint endpoint)
			where TMessage : class, CorrelatedBy<TKey>
		{
			var correlatedConfigurator = new OutboundCorrelatedMessageRouterConfigurator(pipeline);

			CorrelatedMessageRouter<IBusPublishContext<TMessage>, TMessage, TKey> router =
				correlatedConfigurator.FindOrCreate<TMessage, TKey>();

			UnsubscribeAction result = router.Connect(correlationId, new EndpointMessageSink<TMessage>(endpoint));

			return result;
		}
	}
}