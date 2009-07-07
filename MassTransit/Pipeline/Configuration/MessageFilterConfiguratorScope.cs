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
	    public Func<IPipelineSink<TMessage>, IPipelineSink<TMessage>> InsertAfter { get; private set; }

        protected bool Inspect(MessagePipeline element) 
        {
            if(typeof(TMessage) == typeof(object))
            {
                InsertAfter = (sink =>
                {
                    return element
                        .ReplaceOutputSink(sink.TranslateTo<IPipelineSink<object>>())
                        .TranslateTo<IPipelineSink<TMessage>>();
                });

                return false;
            }

            return true;
        }

	    protected bool Inspect<TInput, TOutput>(MessageTranslator<TInput, TOutput> element)
			where TOutput : class, TInput
			where TInput : class
		{
			if (typeof (TOutput) == typeof (TMessage))
			{
				InsertAfter = (sink =>
					{
						return element
							.ReplaceOutputSink(sink.TranslateTo<IPipelineSink<TOutput>>())
							.TranslateTo<IPipelineSink<TMessage>>();
					});

				return false;
			}

			return true;
		}
	}
}