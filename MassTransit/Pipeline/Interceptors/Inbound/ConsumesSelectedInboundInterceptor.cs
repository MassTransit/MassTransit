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
namespace MassTransit.Pipeline.Interceptors.Inbound
{
	using System;
	using Sinks;

	public class ConsumesSelectedInboundInterceptor :
		ConsumesInboundInterceptorBase
	{
		protected override Type InterfaceType
		{
			get { return typeof (Consumes<>.Selected); }
		}

		protected virtual Func<bool> Connect<TMessage>(IInboundContext context, Consumes<TMessage>.Selected consumer) where TMessage : class
		{
			MessageRouterConfigurator routerConfigurator = MessageRouterConfigurator.For(context.Pipeline);

			return routerConfigurator.FindOrCreate<TMessage>().Connect(new MessageSink<TMessage>(message =>
												 consumer.Accept(message) ? consumer : Consumes<TMessage>.Null));
		}

		protected virtual Func<bool> Connect<TComponent, TMessage>(IInboundContext context)
			where TMessage : class
			where TComponent : class, Consumes<TMessage>.Selected
		{
			MessageRouterConfigurator routerConfigurator = MessageRouterConfigurator.For(context.Pipeline);

			return routerConfigurator.FindOrCreate<TMessage>().Connect(new ComponentMessageSink<TComponent, TMessage>(context));
		}
	}
}