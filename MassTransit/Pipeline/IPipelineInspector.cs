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
	using Sinks;

	public interface IPipelineInspector
	{
		bool Inspect(MessagePipeline element);
		bool Inspect<TMessage>(MessageRouter<TMessage> element) where TMessage : class;
		bool Inspect<TMessage>(MessageSink<TMessage> sink) where TMessage : class;
		bool Inspect<TInput, TOutput>(MessageTranslator<TInput, TOutput> translator) where TInput : class where TOutput : class, TInput;

		bool Inspect<TMessage>(IMessageSink<TMessage> element) where TMessage : class;
	}
}