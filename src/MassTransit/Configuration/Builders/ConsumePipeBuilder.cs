// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Builders
{
    using BusConfigurators;
    using PipeBuilders;
    using Pipeline;
    using Pipeline.Filters;
    using Pipeline.Pipes;


    public class ConsumePipeBuilder :
        IConsumePipeBuilder
    {
        readonly MessageTypeConsumeFilter _messageTypeConsumeFilter;
        readonly PipeBuilder<ConsumeContext> _pipeBuilder;

        public ConsumePipeBuilder()
        {
            _messageTypeConsumeFilter = new MessageTypeConsumeFilter();
            _pipeBuilder = new PipeBuilder<ConsumeContext>();
        }

        void IPipeBuilder<ConsumeContext>.AddFilter(IFilter<ConsumeContext> filter)
        {
            _pipeBuilder.AddFilter(filter);
        }

        void IConsumePipeBuilder.AddFilter<T>(IFilter<ConsumeContext<T>> filter)
        {
            _messageTypeConsumeFilter.AddFilter(filter);
        }

        public IConsumePipe Build()
        {
            _pipeBuilder.AddFilter(_messageTypeConsumeFilter);

            return new ConsumePipe(_messageTypeConsumeFilter, _pipeBuilder.Build());
        }
    }
}