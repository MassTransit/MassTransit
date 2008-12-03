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
	using Inspectors;
	using Sinks;

	/// <summary>
	/// Performs the pipeline configuration for the CorrelatedMessageRouter
	/// </summary>
	/// <typeparam name="TMessage"></typeparam>
	public class CorrelatedMessageRouterConfigurator<TMessage, TKey> :
		PipelineInspectorBase
		where TMessage : class, CorrelatedBy<TKey>
	{
		private bool _found;
		private MessageRouter<object> _objectRouter;
		private MessageRouter<TMessage> _router;

		private MessageRouter<TMessage> Router
		{
			get { return _router; }
		}

		private bool Find(IMessageSink<object> pipeline, bool addIfNotFound)
		{
			pipeline.Inspect(this);

			if (_found == false && addIfNotFound)
			{
				AddRouter();
			}

			return _found;
		}

		private void AddRouter()
		{
			if (_objectRouter == null)
				return;

			MessageRouter<TMessage> router = new MessageRouter<TMessage>();

			MessageTranslator<object, TMessage> translator = new MessageTranslator<object, TMessage>(router);

			_objectRouter.Connect(translator);

			_router = router;

			_found = true;
		}

		public override bool Inspect<TRoutedMessage>(MessageRouter<TRoutedMessage> element)
		{
			if (typeof (TRoutedMessage) == typeof (TMessage))
			{
				_router = TranslateTo<MessageRouter<TMessage>>.From(element);

				_found = true;

				return false;
			}

			if (typeof (TRoutedMessage) == typeof (object))
			{
				_objectRouter = TranslateTo<MessageRouter<object>>.From(element);
			}

			return true;
		}

		public static Func<bool> Connect(MessagePipeline pipeline, IMessageSink<TMessage> sink)
		{
			MessageRouterConfigurator routerConfigurator = MessageRouterConfigurator.For(pipeline);

			return routerConfigurator.FindOrCreate<TMessage>().Connect(sink);
		}
	}
}