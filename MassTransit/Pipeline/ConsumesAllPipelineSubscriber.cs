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

	public class ConsumesAllPipelineSubscriber :
		ConsumesPipelineSubscriberBase
	{
		protected override Type InterfaceType
		{
			get { return typeof (Consumes<>.All); }
		}

		protected virtual Func<bool> Connect<TMessage>(IInboundContext context, Consumes<TMessage>.All consumer) where TMessage : class
		{
			var sink = new MessageSink<TMessage>(message => consumer);

			return context.Connect(sink);
		}

		protected virtual Func<bool> Connect<TComponent, TMessage>(IInboundContext context)
			where TMessage : class
			where TComponent : class, Consumes<TMessage>.All
		{
			var sink = new ComponentMessageSink<TComponent, TMessage>(context);

			return context.Connect(sink);
		}
	}
}