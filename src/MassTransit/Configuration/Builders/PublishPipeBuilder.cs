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
    using PipeBuilders;
    using Pipeline;
    using Pipeline.Filters;
    using Pipeline.Pipes;


    public class PublishPipeBuilder :
        IPublishPipeBuilder
    {
        readonly MessageTypePublishFilter _messageTypePublishFilter;
        readonly PipeBuilder<PublishContext> _pipeBuilder;

        public PublishPipeBuilder()
        {
            _messageTypePublishFilter = new MessageTypePublishFilter();
            _pipeBuilder = new PipeBuilder<PublishContext>();
        }

        void IPipeBuilder<PublishContext>.AddFilter(IFilter<PublishContext> filter)
        {
            _pipeBuilder.AddFilter(filter);
        }

        void IPublishPipeBuilder.AddFilter<T>(IFilter<PublishContext<T>> filter)
        {
            _messageTypePublishFilter.AddFilter(filter);
        }

        public IPublishPipe Build()
        {
            _pipeBuilder.AddFilter(_messageTypePublishFilter);

            return new PublishPipe(_messageTypePublishFilter, _pipeBuilder.Build());
        }
    }
}