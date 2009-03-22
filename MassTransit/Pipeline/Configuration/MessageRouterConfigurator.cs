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
	using Exceptions;
	using Sinks;

	public class MessageRouterConfigurator
	{
		private readonly IPipelineSink<object> _sink;
		private readonly object _typedSink;

		private MessageRouterConfigurator(IPipelineSink<object> sink)
		{
			_sink = sink;
		}

		private MessageRouterConfigurator(object typedSink)
		{
			_typedSink = typedSink;
		}

		public MessageRouter<TMessage> FindOrCreate<TMessage>()
			where TMessage : class
		{
			var scope = new MessageRouterConfiguratorScope<TMessage>();

			if (_sink != null)
				_sink.Inspect(scope);
			else if (_typedSink != null)
				_typedSink.TranslateTo<IPipelineSink<TMessage>>().Inspect(scope);

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

		public UnsubscribeAction Connect<TMessage>(IMessagePipeline pipeline, IPipelineSink<TMessage> sink)
			where TMessage : class
		{
			MessageRouterConfigurator routerConfigurator = For(pipeline);

			return routerConfigurator.FindOrCreate<TMessage>().Connect(sink);
		}

		public static MessageRouterConfigurator For(IPipelineSink<object> sink)
		{
			return new MessageRouterConfigurator(sink);
		}

		public static MessageRouterConfigurator For<TMessage>(IPipelineSink<TMessage> sink)
			where TMessage : class
		{
			return new MessageRouterConfigurator(sink);
		}
	}
}