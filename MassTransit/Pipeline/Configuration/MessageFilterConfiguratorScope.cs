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
	using Inspectors;
	using Sinks;

	public class MessageFilterConfiguratorScope<TMessage> :
		PipelineInspectorBase<MessageFilterConfiguratorScope<TMessage>>
		where TMessage : class
	{
		private Func<IPipelineSink<TMessage>, IPipelineSink<TMessage>> _insertAfter;

		public Func<IPipelineSink<TMessage>, IPipelineSink<TMessage>> InsertAfter
		{
			get { return _insertAfter; }
		}

		protected bool Inspect<TInput, TOutput>(MessageTranslator<TInput, TOutput> element)
			where TOutput : class, TInput
			where TInput : class
		{
			if (typeof (TOutput) == typeof (TMessage))
			{
				_insertAfter = (sink =>
					{
						return TranslateTo<IPipelineSink<TMessage>>
							.From(element.ReplaceOutputSink(TranslateTo<IPipelineSink<TOutput>>.From(sink)));
					});

				return false;
			}

			return true;
		}
	}
}