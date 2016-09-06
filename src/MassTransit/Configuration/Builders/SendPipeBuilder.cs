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
    using GreenPipes;
    using GreenPipes.Builders;
    using Pipeline;
    using Pipeline.Filters;
    using Pipeline.Pipes;


    public class SendPipeBuilder :
        ISendPipeBuilder
    {
        readonly MessageTypeSendFilter _messageTypeSendFilter;
        readonly PipeBuilder<SendContext> _pipeBuilder;

        public SendPipeBuilder()
        {
            _messageTypeSendFilter = new MessageTypeSendFilter();
            _pipeBuilder = new PipeBuilder<SendContext>();
        }

        void IPipeBuilder<SendContext>.AddFilter(IFilter<SendContext> filter)
        {
            _pipeBuilder.AddFilter(filter);
        }

        void ISendPipeBuilder.AddFilter<T>(IFilter<SendContext<T>> filter)
        {
            _messageTypeSendFilter.AddFilter(filter);
        }

        public ISendPipe Build()
        {
            _pipeBuilder.AddFilter(_messageTypeSendFilter);

            return new SendPipe(_messageTypeSendFilter, _pipeBuilder.Build());
        }
    }
}