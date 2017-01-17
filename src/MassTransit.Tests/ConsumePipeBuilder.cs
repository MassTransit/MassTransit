// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests
{
    using System;
    using Context;
    using GreenPipes;
    using GreenPipes.Builders;
    using GreenPipes.Filters;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Pipes;


    public class ConsumePipeBuilder :
        IConsumePipeBuilder
    {
        readonly DynamicFilter<ConsumeContext, Guid> _filter;
        readonly PipeBuilder<ConsumeContext> _pipeBuilder;

        public ConsumePipeBuilder()
        {
            _filter = new DynamicFilter<ConsumeContext, Guid>(new ConsumeContextConverterFactory(), GetRequestId);
            _pipeBuilder = new PipeBuilder<ConsumeContext>();
        }

        void IPipeBuilder<ConsumeContext>.AddFilter(IFilter<ConsumeContext> filter)
        {
            _pipeBuilder.AddFilter(filter);
        }

        void IConsumePipeBuilder.AddFilter<T>(IFilter<ConsumeContext<T>> filter)
        {
            _filter.AddFilter(filter);
        }

        public IConsumePipe Build()
        {
            _pipeBuilder.AddFilter(_filter);

            return new ConsumePipe(_filter, _pipeBuilder.Build());
        }

        static Guid GetRequestId(ConsumeContext context)
        {
            return context.RequestId ?? Guid.Empty;
        }
    }
}