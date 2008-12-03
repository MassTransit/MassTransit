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
	using Exceptions;
	using Inspectors;
	using Sinks;

	public class MessageRouterConfigurator :
		PipelineInspectorBase
	{
		private readonly IMessageSink<object> _sink;

		private MessageRouterConfigurator(IMessageSink<object> sink)
		{
			_sink = sink;
		}

		public MessageRouter<TMessage> FindOrCreate<TMessage>()
			where TMessage : class
		{
			var scope = new MessageRouterConfiguratorScope<TMessage>();

			_sink.Inspect(scope);

			return scope.Router ?? ConfigureRouter<TMessage>(scope.ObjectRouter);
		}

		private static MessageRouter<TMessage> ConfigureRouter<TMessage>(MessageRouter<object> objectRouter)
			where TMessage : class
		{
			if (objectRouter == null)
				throw new PipelineException("The base object router was not found");

			MessageRouter<TMessage> router = new MessageRouter<TMessage>();

			MessageTranslator<object, TMessage> translator = new MessageTranslator<object, TMessage>(router);

			objectRouter.Connect(translator);

			return router;
		}

		public Func<bool> Connect<TMessage>(MessagePipeline pipeline, IMessageSink<TMessage> sink)
			where TMessage : class
		{
			MessageRouterConfigurator routerConfigurator = For(pipeline);

			return routerConfigurator.FindOrCreate<TMessage>().Connect(sink);
		}

		public static MessageRouterConfigurator For<TMessage>(IMessageSink<TMessage> sink)
			where TMessage : class
		{
			return new MessageRouterConfigurator(TranslateTo<IMessageSink<object>>.From(sink));
		}
	}

	public class MessageRouterConfiguratorScope<TMessage> :
		PipelineInspectorBase
		where TMessage : class
	{
		public MessageRouter<object> ObjectRouter { get; private set; }
		public MessageRouter<TMessage> Router { get; private set; }

		public override bool Inspect<TRoutedMessage>(MessageRouter<TRoutedMessage> element)
		{
			if (typeof (TRoutedMessage) == typeof (TMessage))
			{
				Router = TranslateTo<MessageRouter<TMessage>>.From(element);

				return false;
			}

			if (typeof (TRoutedMessage) == typeof (object))
			{
				ObjectRouter = TranslateTo<MessageRouter<object>>.From(element);
			}

			return true;
		}
	}
}