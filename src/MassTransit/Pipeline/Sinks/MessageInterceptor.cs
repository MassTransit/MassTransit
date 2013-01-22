// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Pipeline.Sinks
{
    using System;
    using System.Collections.Generic;


    public class InboundMessageInterceptor :
        IPipelineSink<IConsumeContext>
    {
        readonly IInboundMessageInterceptor _interceptor;
        readonly IPipelineSink<IConsumeContext> _output;

        public InboundMessageInterceptor(
            Func<IPipelineSink<IConsumeContext>, IPipelineSink<IConsumeContext>> insertAfter,
            IInboundMessageInterceptor interceptor)
        {
            _interceptor = interceptor;

            _output = insertAfter(this);
        }

        public IEnumerable<Action<IConsumeContext>> Enumerate(IConsumeContext context)
        {
            _interceptor.PreDispatch(context);

            foreach (var consumer in _output.Enumerate(context))
                yield return consumer;

            _interceptor.PostDispatch(context);
        }

        public bool Inspect(IPipelineInspector inspector)
        {
            return inspector.Inspect(this) && _output.Inspect(inspector);
        }
    }
}