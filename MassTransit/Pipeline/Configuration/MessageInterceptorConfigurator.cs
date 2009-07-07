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

    public class MessageInterceptorConfigurator
    {
        private readonly IPipelineSink<object> _sink;

        private MessageInterceptorConfigurator(IPipelineSink<object> sink)
        {
            _sink = sink;
        }

        public MessageInterceptor Create(Action beforeConsume, Action afterConsume)
        {
            var scope = new MessageInterceptorConfiguratorScope();
            _sink.Inspect(scope);

            return ConfigureFilter(scope.InsertAfter, beforeConsume, afterConsume);
        }

        private static MessageInterceptor ConfigureFilter(Func<IPipelineSink<object>, IPipelineSink<object>> insertAfter, Action beforeConsume, Action afterConsume)
        {
            if (insertAfter == null)
                throw new PipelineException("Unable to insert filter into pipeline for message type " + typeof (object).FullName);

            return new MessageInterceptor(insertAfter, beforeConsume, afterConsume);
        }

        public static MessageInterceptorConfigurator For(IMessagePipeline sink)
        {
            return new MessageInterceptorConfigurator(sink);
        }
    }
}