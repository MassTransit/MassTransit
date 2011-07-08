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
namespace MassTransit.Pipeline.Configuration
{
	using System;
	using Context;
	using Exceptions;
	using Sinks;

	public class InboundMessageInterceptorConfigurator
	{
		readonly IInboundMessagePipeline _sink;

		public InboundMessageInterceptorConfigurator(IInboundMessagePipeline sink)
		{
			_sink = sink;
		}

		public InboundMessageInterceptor Create(IInboundMessageInterceptor messageInterceptor)
		{
			var scope = new InboundMessageInterceptorConfiguratorScope();
			_sink.Inspect(scope);

			return ConfigureInterceptor(scope.InsertAfter, messageInterceptor);
		}

		static InboundMessageInterceptor ConfigureInterceptor(
			Func<IPipelineSink<IConsumeContext>, IPipelineSink<IConsumeContext>> insertAfter,
			IInboundMessageInterceptor messageInterceptor)
		{
			if (insertAfter == null)
				throw new PipelineException("Unable to insert filter into pipeline for message type " + typeof (object).FullName);

			var interceptor = new InboundMessageInterceptor(insertAfter, messageInterceptor);

			return interceptor;
		}
	}
}