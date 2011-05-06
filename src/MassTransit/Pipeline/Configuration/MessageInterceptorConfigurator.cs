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
	using Exceptions;
	using Sinks;

	public class MessageInterceptorConfigurator
	{
		readonly IPipelineSink<object> _sink;

		public MessageInterceptorConfigurator(IPipelineSink<object> sink)
		{
			_sink = sink;
		}

		public MessageInterceptor Create(IMessageInterceptor messageInterceptor)
		{
			var scope = new MessageInterceptorConfiguratorScope();
			_sink.Inspect(scope);

			return ConfigureInterceptor(scope.InsertAfter, messageInterceptor);
		}

		static MessageInterceptor ConfigureInterceptor(Func<IPipelineSink<object>, IPipelineSink<object>> insertAfter,
		                                               IMessageInterceptor messageInterceptor)
		{
			if (insertAfter == null)
				throw new PipelineException("Unable to insert filter into pipeline for message type " + typeof (object).FullName);

			var interceptor = new MessageInterceptor(messageInterceptor);

			interceptor.ReplaceOutputSink(insertAfter(interceptor));

			return interceptor;
		}
	}
}